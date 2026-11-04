using System.Linq.Expressions;
using Dobu.Application.Interfaces.Repositories;
using Dobu.Application.Queries;
using Dobu.Application.Services;
using Dobu.Domain.Entities;
using Moq;
using Xunit;

namespace Dobu.UnitTests;

public class ObservationServiceTests
{
    private static (CameraObservationService service, Mock<ICameraObservationRepository> observations) Service(bool allowed = true)
    {
        var cameras = new Mock<IEntityRepository<DobuCam>>();
        cameras.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<DobuCam, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<DobuCam, object>>[]?>(), true))
            .ReturnsAsync(allowed ? new DobuCam("Sala", "ATIVA", null, Guid.NewGuid()) : null);
        var work = new Mock<IDobuUnitOfWork>();
        work.SetupGet(x => x.DobuCams).Returns(cameras.Object);
        var observations = new Mock<ICameraObservationRepository>();
        return (new CameraObservationService(work.Object, observations.Object), observations);
    }

    [Fact]
    public async Task CriarObservacao_ForaDoAcesso_NaoEscreveNoMongo()
    {
        // Arrange
        var (service, repository) = Service(false);
        var request = new CameraObservationRequest("PRESENCA", "Animal identificado", DateTime.UtcNow, .8);
        // Act
        var create = () => service.CreateAsync(Guid.NewGuid(), Guid.NewGuid(), request, default);
        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(create);
        repository.Verify(x => x.AddAsync(It.IsAny<CameraObservation>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CriarObservacao_Futura_NaoEscreveNoMongo()
    {
        // Arrange
        var (service, repository) = Service();
        var request = new CameraObservationRequest("PRESENCA", "Animal identificado", DateTime.UtcNow.AddHours(1), .8);
        // Act
        var create = () => service.CreateAsync(Guid.NewGuid(), Guid.NewGuid(), request, default);
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(create);
        repository.Verify(x => x.AddAsync(It.IsAny<CameraObservation>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegistroAusente_ConsultaEExclusaoRetornamNaoEncontrado()
    {
        // Arrange
        var (service, _) = Service();
        var camera = Guid.NewGuid(); var owner = Guid.NewGuid(); var id = Guid.NewGuid();
        // Act / Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetAsync(camera, owner, id, default));
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteAsync(camera, owner, id, default));
    }
}
