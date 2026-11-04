using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Dobu.Application.DTOs;
using Xunit;

namespace Dobu.IntegrationTests;

[Collection(ApiCollection.Name)]
public class AuthFlowTests(DobuApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task RegisterELogin_CredenciaisValidas_RetornaTokenEPermiteEndpointProtegido()
    {
        // Arrange
        var email = $"usuario-{Guid.NewGuid():N}@dobu.com";

        // Act
        var register = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest("Ana", email, "Senha123", "Responsavel"));
        var login = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, "Senha123"));
        var auth = await login.Content.ReadFromJsonAsync<AuthResponse>();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth!.Token);
        var pets = await _client.GetAsync("/api/pets");

        // Assert
        Assert.Equal(HttpStatusCode.OK, register.StatusCode);
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        Assert.Equal(HttpStatusCode.OK, pets.StatusCode);
    }

    [Theory]
    [InlineData("Responsavel")]
    [InlineData("Veterinario")]
    public async Task Login_PerfilValido_AcessaEndpointProtegido(string tipoUsuario)
    {
        // Arrange
        var email = $"usuario-{Guid.NewGuid():N}@dobu.com";
        await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest("Ana", email, "Senha123", tipoUsuario));
        var login = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, "Senha123"));
        var auth = await login.Content.ReadFromJsonAsync<AuthResponse>();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/pets");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(tipoUsuario.ToUpperInvariant(), auth.TipoUsuario);
    }

    [Fact]
    public async Task Login_SenhaIncorreta_RetornaNaoAutorizado()
    {
        // Arrange
        var email = $"usuario-{Guid.NewGuid():N}@dobu.com";
        await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest("Ana", email, "Senha123", "Responsavel"));

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, "SenhaErrada"));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_DadosInvalidos_RetornaBadRequest()
    {
        // Arrange
        var request = new RegisterRequest("A", "invalido", "123", "Responsavel");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_UsuarioCriadoPeloCrud_RetornaToken()
    {
        // Arrange
        var email = $"crud-{Guid.NewGuid():N}@dobu.com";
        await _client.PostAsJsonAsync("/api/usuarios", new
        {
            Nome = "Ana",
            Email = email,
            Senha = "Senha123",
            TipoUsuario = "Responsavel"
        });

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, "Senha123"));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PutUsuario_OutroUsuarioAutenticado_RetornaForbidden()
    {
        // Arrange
        var dono = await RegistrarAsync("dono");
        var outro = await RegistrarAsync("outro");
        using var request = new HttpRequestMessage(HttpMethod.Put, $"/api/usuarios/{outro.UsuarioId}")
        {
            Content = JsonContent.Create(new
            {
                Nome = "Outro Atualizado",
                Email = $"outro-atualizado-{Guid.NewGuid():N}@dobu.com",
                Senha = "Senha123",
                TipoUsuario = "Responsavel"
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", dono.Token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUsuario_ProprioUsuarioAutenticado_RetornaNoContent()
    {
        // Arrange
        var usuario = await RegistrarAsync("remover");
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/usuarios/{usuario.UsuarioId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", usuario.Token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task<AuthResponse> RegistrarAsync(string prefixo)
    {
        var email = $"{prefixo}-{Guid.NewGuid():N}@dobu.com";
        var response = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest("Ana", email, "Senha123", "Responsavel"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthResponse>())!;
    }
}
