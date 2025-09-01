using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Aplicacao.ModuloSala;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidade.ModuloSala
{
    [TestClass]
    [TestCategory("Teste de unidade de Sala")]

    public sealed class SalaAppServiceTests
    {
        private Mock<IRepositorioSala>? _repositorioSalaMock;
        private Mock<ITenantProvider>? _tenantProviderMock;
        private Mock<IUnitOfWork>? _unitOfWorkMock;
        private Mock<ILogger<SalaAppService>>? _loggerMock;

        private SalaAppService? _salaAppService;

        [TestInitialize]
        public void Setup()
        {
            _repositorioSalaMock = new Mock<IRepositorioSala>();
            _tenantProviderMock = new Mock<ITenantProvider>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<SalaAppService>>();


            _salaAppService = new SalaAppService(
                _tenantProviderMock.Object,
                _repositorioSalaMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object
                );

        }
        [TestMethod]
        public void Cadastrar_DeveRetornarOk_QuandoSalaForValida()
        {
            // Arrange
            var sala = new Sala(01, 100);

            var salaTeste = new Sala(02, 100);

            _repositorioSalaMock?
                .Setup(r => r.SelecionarRegistros())
                .Returns(new List<Sala>() { salaTeste });

            // Act
            var resultado = _salaAppService?.Cadastrar(sala);

            // Assert
            _repositorioSalaMock?.Verify(r => r.Cadastrar(sala), Times.Once);
            _unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.IsSuccess);
        }

        [TestMethod]
        public void Cadastrar_DeveRetornarErro_QuandoSalaJaExistir()
        {
            // Arrange
            var sala = new Sala(01, 100);

            _repositorioSalaMock?
                .Setup(r => r.SelecionarRegistros())
                .Returns(new List<Sala>() { sala });

            // Act
            var resultado = _salaAppService?.Cadastrar(sala);

            // Assert
            _repositorioSalaMock?.Verify(r => r.Cadastrar(It.IsAny<Sala>()), Times.Never);
            _unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.IsSuccess);
        }
        [TestMethod]

        public void Cadastrar_DeveRetornarFalha_QuandoSalaForDuplicado() 
        {
            // Arrange
            var sala = new Sala(10, 100);
            var salaTeste = new Sala(10, 100);

            _repositorioSalaMock?
                .Setup(r=> r.SelecionarRegistros())
                .Returns(new List<Sala>() {sala});
            // Act

            var resultado = _salaAppService? .Cadastrar(sala);

            // Assert
            Assert.IsTrue(resultado.IsFailed);
            _repositorioSalaMock.Verify(r => r.Cadastrar(It.IsAny<Sala>()), Times.Never);
            _unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);
        }
       

    }

}


