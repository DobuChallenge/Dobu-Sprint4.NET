using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Dobu.Application.DTOs;
using Xunit;

namespace Dobu.IntegrationTests;

public sealed class MongoFactAttribute : FactAttribute
{
    public MongoFactAttribute()
    {
        if (Environment.GetEnvironmentVariable("DOBU_TEST_MONGO_URL") is null)
            Skip = "Configure DOBU_TEST_MONGO_URL para testar com MongoDB real.";
    }
}

[Collection(ApiCollection.Name)]
public class Sprint4FlowTests(DobuApiFactory factory)
{
    private async Task<(HttpClient client, AuthResponse account)> Account(string type = "RESPONSAVEL")
    {
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/register", new { nome = "Conta Sprint", email = $"{Guid.NewGuid():N}@example.test", senha = "Teste12345", tipoUsuario = type });
        response.EnsureSuccessStatusCode();
        var account = (await response.Content.ReadFromJsonAsync<AuthResponse>())!;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", account.Token);
        return (client, account);
    }

    private static async Task<Guid> Create(HttpClient client, string path, object body)
    {
        var response = await client.PostAsJsonAsync(path, body);
        response.EnsureSuccessStatusCode();
        var record = await response.Content.ReadFromJsonAsync<JsonElement>();
        return record.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> Pet(HttpClient client, Guid owner)
    {
        var species = await Create(client, "/api/especies", new { nome = "Gato Sprint", descricao = "Espécie de teste" });
        var breed = await Create(client, "/api/racas", new { nome = "Sem raça", porte = "Pequeno", expectativaVida = 15, descricao = "Cadastro de teste", cuidados = "Rotina", especieId = species });
        return await Create(client, "/api/pets", new { nome = "Lua Sprint", idade = 2, racaId = breed, responsavelId = owner });
    }

    [Fact]
    public async Task Consultas_PaginadasOrdenadasFiltradas_RetornamLinksETotalSemMudarLista()
    {
        // Arrange
        var (client, _) = await Account();
        var term = Guid.NewGuid().ToString("N");
        foreach (var name in new[] { "C", "A", "B" }) await Create(client, "/api/especies", new { nome = name + term, descricao = "Catálogo de teste" });
        // Act
        var response = await client.GetAsync($"/api/especies?page=1&pageSize=2&sort=nome&search={term}");
        var records = await response.Content.ReadFromJsonAsync<JsonElement>();
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, records.GetArrayLength());
        Assert.StartsWith("A", records[0].GetProperty("nome").GetString());
        Assert.Equal("3", response.Headers.GetValues("X-Total-Count").Single());
        Assert.Contains("rel=\"next\"", response.Headers.GetValues("Link").Single());
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/especies?pageSize=101")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/especies?sort=Senha")).StatusCode);
    }

    [Fact]
    public async Task Conversa_DuasConfirmacoes_ConfirmaConsultaERejeitaTerceiro()
    {
        // Arrange
        var (owner, account) = await Account();
        var (vet, vetAccount) = await Account("VETERINARIO");
        var (stranger, _) = await Account();
        var pet = await Pet(owner, account.UsuarioId);
        var appointment = await Create(owner, "/api/agendamentos", new { dataAgendamento = DateTime.UtcNow.AddDays(1), status = "AGENDADO", petId = pet, veterinarioId = vetAccount.UsuarioId });
        var path = $"/api/agendamentos/{appointment}/chat";
        // Act
        var first = await owner.PostAsJsonAsync(path, new { texto = "", tipo = "CONFIRMACAO" });
        var second = await vet.PostAsJsonAsync(path, new { texto = "", tipo = "CONFIRMACAO" });
        var duplicate = await owner.PostAsJsonAsync(path, new { texto = "", tipo = "CONFIRMACAO" });
        var messages = await owner.GetFromJsonAsync<JsonElement>(path);
        var updated = await owner.GetFromJsonAsync<JsonElement>($"/api/agendamentos/{appointment}");
        // Assert
        Assert.True(first.IsSuccessStatusCode && second.IsSuccessStatusCode && duplicate.IsSuccessStatusCode);
        Assert.Equal(2, messages.GetArrayLength());
        Assert.Equal("CONFIRMADO", updated.GetProperty("status").GetString());
        Assert.Equal(HttpStatusCode.NotFound, (await stranger.GetAsync(path)).StatusCode);
    }

    [MongoFact]
    public async Task Observacoes_CameraDoTutor_PersistemNoMongoComConsultaEExclusao()
    {
        // Arrange
        var (owner, account) = await Account();
        var (stranger, _) = await Account();
        var pet = await Pet(owner, account.UsuarioId);
        var camera = await Create(owner, "/api/dobucams", new { localizacao = "Sala", statusCamera = "ATIVA", dataUltimaMovimentacao = DateTime.UtcNow, petId = pet });
        var path = $"/api/dobucams/{camera}/eventos";
        // Act
        var observation = await Create(owner, path, new { tipo = "HIDRATACAO", descricao = "Possível interação junto ao pote de água", data = DateTime.UtcNow, confianca = .8 });
        var response = await owner.GetAsync(path + "?page=1&pageSize=10&tipo=HIDRATACAO");
        var records = await response.Content.ReadFromJsonAsync<JsonElement>();
        var forbidden = await stranger.GetAsync(path);
        var deleted = await owner.DeleteAsync($"{path}/{observation}");
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, records.GetArrayLength());
        Assert.Equal(observation, records[0].GetProperty("id").GetGuid());
        Assert.Equal(HttpStatusCode.NotFound, forbidden.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await owner.GetAsync($"{path}/{observation}")).StatusCode);
    }
}
