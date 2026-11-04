using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Dobu.Application.DTOs;
using Xunit;

namespace Dobu.IntegrationTests;

[Collection(ApiCollection.Name)]
public class MainCrudFlowTests(DobuApiFactory factory)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CrudPrincipal_FluxoCompleto_CriaConsultaAtualizaERemoveRecursos()
    {
        // Arrange
        var responsavel = await RegistrarAsync("responsavel", "Responsavel");
        var veterinario = await RegistrarAsync("veterinario", "Veterinario");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", responsavel.Token);

        // Act
        var especieId = await CriarAsync("/api/especies", new { Nome = "Cachorro", Descricao = "Animal domestico" });
        var racaId = await CriarAsync("/api/racas", new { Nome = "Vira lata", Porte = "Medio", ExpectativaVida = 12, Descricao = "Raca brasileira", Cuidados = "Banho e passeio", EspecieId = especieId });
        var petId = await CriarAsync("/api/pets", new { Nome = "Mel", Idade = 4, RacaId = racaId, ResponsavelId = responsavel.UsuarioId });
        var consultaId = await CriarAsync("/api/consultas", new { DataConsulta = DateTime.UtcNow.AddDays(2), Descricao = "Consulta anual", Valor = 150m, PetId = petId, VeterinarioId = veterinario.UsuarioId });
        var agendamentoId = await CriarAsync("/api/agendamentos", new { DataAgendamento = DateTime.UtcNow.AddDays(1), Status = "AGENDADO", PetId = petId, VeterinarioId = veterinario.UsuarioId });
        var prontuarioId = await CriarAsync("/api/prontuarios", new { Diagnostico = "Paciente saudavel", Observacoes = "Retorno em seis meses", ConsultaId = consultaId });
        var vacinaId = await CriarAsync("/api/vacinas", new { Nome = "V10", DataAplicacao = DateTime.UtcNow, DataProximaDose = DateTime.UtcNow.AddMonths(12), PetId = petId });
        var pagamentoId = await CriarAsync("/api/pagamentos", new { Valor = 150m, FormaPagamento = "PIX", DataPagamento = DateTime.UtcNow, ConsultaId = consultaId });
        var lembreteId = await CriarAsync("/api/lembretes", new { Descricao = "Aplicar antipulgas", DataLembrete = DateTime.UtcNow.AddDays(30), Status = "PENDENTE", PetId = petId });
        var dobuCamId = await CriarAsync("/api/dobucams", new { Localizacao = "Sala", StatusCamera = "ATIVA", DataUltimaMovimentacao = DateTime.UtcNow, PetId = petId });
        var analiseId = await CriarAsync("/api/analises-ia", new { Descricao = "Sem risco clinico", Risco = 1, DataAnalise = DateTime.UtcNow, ProntuarioId = prontuarioId });
        var informacaoId = await CriarAsync("/api/informacoes-cuidado", new { Titulo = "Alimentacao", Descricao = "Oferecer racao duas vezes ao dia", PetId = petId });

        var getPet = await _client.GetAsync($"/api/pets/{petId}");
        var putConsulta = await _client.PutAsJsonAsync($"/api/consultas/{consultaId}", new { DataConsulta = DateTime.UtcNow.AddDays(3), Descricao = "Consulta atualizada", Valor = 180m, PetId = petId, VeterinarioId = veterinario.UsuarioId });
        var putAgendamento = await _client.PutAsJsonAsync($"/api/agendamentos/{agendamentoId}", new { DataAgendamento = DateTime.UtcNow.AddDays(4), Status = "CONFIRMADO", PetId = petId, VeterinarioId = veterinario.UsuarioId });

        var deleteInformacao = await _client.DeleteAsync($"/api/informacoes-cuidado/{informacaoId}");
        var deleteAnalise = await _client.DeleteAsync($"/api/analises-ia/{analiseId}");
        var deleteProntuario = await _client.DeleteAsync($"/api/prontuarios/{prontuarioId}");
        var deletePagamento = await _client.DeleteAsync($"/api/pagamentos/{pagamentoId}");
        var deleteVacina = await _client.DeleteAsync($"/api/vacinas/{vacinaId}");
        var deleteLembrete = await _client.DeleteAsync($"/api/lembretes/{lembreteId}");
        var deleteDobuCam = await _client.DeleteAsync($"/api/dobucams/{dobuCamId}");
        var deleteAgendamento = await _client.DeleteAsync($"/api/agendamentos/{agendamentoId}");
        var deleteConsulta = await _client.DeleteAsync($"/api/consultas/{consultaId}");
        var deletePet = await _client.DeleteAsync($"/api/pets/{petId}");
        var deleteRaca = await _client.DeleteAsync($"/api/racas/{racaId}");
        var deleteEspecie = await _client.DeleteAsync($"/api/especies/{especieId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, getPet.StatusCode);
        Assert.Equal(HttpStatusCode.OK, putConsulta.StatusCode);
        Assert.Equal(HttpStatusCode.OK, putAgendamento.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, deleteInformacao.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, deleteAnalise.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, deleteProntuario.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, deletePagamento.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, deleteVacina.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, deleteLembrete.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, deleteDobuCam.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, deleteAgendamento.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, deleteConsulta.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, deletePet.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, deleteRaca.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, deleteEspecie.StatusCode);
    }

    private async Task<AuthResponse> RegistrarAsync(string prefixo, string tipoUsuario)
    {
        var email = $"{prefixo}-{Guid.NewGuid():N}@dobu.com";
        var response = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest("Ana", email, "Senha123", tipoUsuario));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions))!;
    }

    private async Task<Guid> CriarAsync(string rota, object payload)
    {
        var response = await _client.PostAsJsonAsync(rota, payload);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using var document = JsonDocument.Parse(body);
        return document.RootElement.GetProperty("id").GetGuid();
    }
}
