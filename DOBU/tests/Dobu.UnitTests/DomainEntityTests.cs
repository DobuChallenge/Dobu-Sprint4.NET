using Dobu.Domain.Entities;
using Xunit;

namespace Dobu.UnitTests;

public class DomainEntityTests
{
    [Fact]
    public void Usuario_CredenciaisInvalidas_LancaArgumentException()
    {
        // Arrange

        // Act
        var action = () => new Usuario("A", "email-invalido", "123", "Responsavel");

        // Assert
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Usuario_TipoUsuarioNumerico_LancaArgumentException()
    {
        // Arrange

        // Act
        var action = () => new Usuario("Ana", "ana@dobu.com", "Senha123", "1");

        // Assert
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Pet_IdadeNegativa_LancaArgumentException()
    {
        // Arrange

        // Act
        var action = () => new Pet("Mel", -1, Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void InformacaoCuidado_DescricaoValida_CriaEntidade()
    {
        // Arrange
        var petId = Guid.NewGuid();

        // Act
        var information = new InformacaoCuidado("Alimentação", "Oferecer ração duas vezes ao dia", petId);

        // Assert
        Assert.Equal(petId, information.PetId);
        Assert.Equal("Alimentação", information.Titulo);
    }
}
