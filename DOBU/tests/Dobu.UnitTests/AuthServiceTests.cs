using Dobu.Application.DTOs;
using Dobu.Application.Services;
using Dobu.Domain.Entities;
using Moq;
using Xunit;

namespace Dobu.UnitTests;

public sealed class AuthServiceFixture
{
    public string SigningKey { get; } = "chave-de-teste-com-mais-de-32-caracteres";
}

public class AuthServiceTests(AuthServiceFixture fixture) : IClassFixture<AuthServiceFixture>
{
    [Fact]
    public async Task RegisterAsync_NovoUsuario_RetornaTokenSemExporSenha()
    {
        // Arrange
        var repository = new Mock<IUserStore>();
        repository.Setup(x => x.FindByEmailAsync("ana@dobu.com", It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);
        var service = new AuthService(repository.Object, fixture.SigningKey);

        // Act
        var result = await service.RegisterAsync(new RegisterRequest("Ana", "ana@dobu.com", "Senha123", "Responsavel"));

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.Equal("ana@dobu.com", result.Email);
        repository.Verify(x => x.AddAsync(It.Is<Usuario>(u => u.Senha != "Senha123"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_SenhaIncorreta_LancaUnauthorizedAccessException()
    {
        // Arrange
        var user = new Usuario("Ana", "ana@dobu.com", "Senha123", "Responsavel");
        var repository = new Mock<IUserStore>();
        repository.Setup(x => x.FindByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        var service = new AuthService(repository.Object, fixture.SigningKey);

        // Act
        var action = () => service.LoginAsync(new LoginRequest(user.Email, "errada"));

        // Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(action);
    }

    [Fact]
    public async Task RegisterAsync_SenhaCurta_LancaArgumentException()
    {
        // Arrange
        var repository = new Mock<IUserStore>();
        var service = new AuthService(repository.Object, fixture.SigningKey);

        // Act
        var action = () => service.RegisterAsync(new RegisterRequest("Ana", "ana@dobu.com", "123", "Responsavel"));

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(action);
    }
}
