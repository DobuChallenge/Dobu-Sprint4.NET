using Dobu.Domain.Entities;
using Xunit;

namespace Dobu.UnitTests;

public class CameraObservationTests
{
    [Theory]
    [InlineData("presenca")]
    [InlineData("movimento")]
    [InlineData("alimentacao")]
    [InlineData("hidratacao")]
    [InlineData("descanso")]
    public void ObservacaoValida_NormalizaTipoEPreservaIdentificacao(string type)
    {
        // Arrange
        var camera = Guid.NewGuid();
        var owner = Guid.NewGuid();
        var timestamp = DateTime.UtcNow;
        // Act
        var observation = new CameraObservation(camera, owner, type, " Possível interação ", timestamp, .8);
        // Assert
        Assert.Equal(type.ToUpperInvariant(), observation.Type);
        Assert.Equal("Possível interação", observation.Description);
        Assert.Equal(camera, observation.CameraId);
        Assert.Equal(owner, observation.OwnerId);
        Assert.Equal(timestamp, observation.Timestamp);
        Assert.NotEqual(Guid.Empty, observation.Id);
    }

    [Theory]
    [InlineData(-.1)]
    [InlineData(1.1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void ConfiancaInvalida_NaoCriaObservacao(double confidence)
    {
        // Arrange
        var camera = Guid.NewGuid();
        // Act
        var create = () => new CameraObservation(camera, Guid.NewGuid(), "HIDRATACAO", "Possível hidratação", DateTime.UtcNow, confidence);
        // Assert
        Assert.Throws<ArgumentException>(create);
    }

    [Fact]
    public void IdentificadoresTipoDescricaoOuDataInvalidos_NaoCriamObservacoes()
    {
        // Arrange
        var camera = Guid.NewGuid(); var owner = Guid.NewGuid(); var date = DateTime.UtcNow;
        var actions = new Action[] {
            () => _ = new CameraObservation(Guid.Empty, owner, "PRESENCA", "Animal observado", date, null),
            () => _ = new CameraObservation(camera, Guid.Empty, "PRESENCA", "Animal observado", date, null),
            () => _ = new CameraObservation(camera, owner, "DIAGNOSTICO", "Animal observado", date, null),
            () => _ = new CameraObservation(camera, owner, "PRESENCA", "", date, null),
            () => _ = new CameraObservation(camera, owner, "PRESENCA", new string('a', 501), date, null),
            () => _ = new CameraObservation(camera, owner, "PRESENCA", "Animal observado", default, null)
        };
        // Act / Assert
        foreach (var action in actions) Assert.Throws<ArgumentException>(action);
    }
}
