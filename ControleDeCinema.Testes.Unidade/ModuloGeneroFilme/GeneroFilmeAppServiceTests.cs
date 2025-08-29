using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidade.ModuloGenero
{
    [TestClass]
    [TestCategory("Teste de Unidade de GêneroFilme")]
    public sealed class GeneroFilmeAppServiceTests
    {
        private Mock<IRepositorioGeneroFilme>? _repositorioGeneroFilmeMock; 
        private Mock<ITenantProvider>? _tenantProviderMock;
        private Mock<IUnitOfWork>? _unitOfWorkMock;
        private Mock<ILogger<GeneroFilmeAppService>>? _loggerMock;
       

        private GeneroFilmeAppService? _generoFilmeApp;

        [TestInitialize]

        public void Setup() 
        {
            _repositorioGeneroFilmeMock = new Mock<IRepositorioGeneroFilme>();
            _tenantProviderMock = new Mock<ITenantProvider>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<GeneroFilmeAppService>>();
            

            _generoFilmeApp = new GeneroFilmeAppService(
                _tenantProviderMock.Object,
                _repositorioGeneroFilmeMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object
                );

        }
        
        [TestMethod]
        public void Cadastrar_DeveRetornarOk_QuandoGeneroForValido()
        {
            // Arrange
            var generoFilme = new GeneroFilme("Ação");
            var generoTeste = new GeneroFilme("Comédia");
            _repositorioGeneroFilmeMock!
                .Setup(r => r.SelecionarRegistros())
                .Returns(new List<GeneroFilme>() { generoTeste });
            _tenantProviderMock!
                .Setup(tp => tp.UsuarioId)
                .Returns(Guid.NewGuid());
            // Act
            var resultado = _generoFilmeApp!.Cadastrar(generoFilme);
            // Assert
            Assert.IsTrue(resultado.IsSuccess);
            _repositorioGeneroFilmeMock.Verify(r => r.Cadastrar(generoFilme), Times.Once);
            _unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);
        }
        [TestMethod]
        public void Cadastrar_DeveRetornarFalha_QuandoGeneroForDuplicado()
        {
            // Arrange
            var generoFilme = new GeneroFilme("Ação");
            var generoTeste = new GeneroFilme("Ação");

            _repositorioGeneroFilmeMock!
                .Setup(r => r.SelecionarRegistros())
                .Returns(new List<GeneroFilme>() { generoTeste });

            // Act
            var resultado = _generoFilmeApp!.Cadastrar(generoFilme);

            // Assert
            Assert.IsTrue(resultado.IsFailed);
            _repositorioGeneroFilmeMock.Verify(r => r.Cadastrar(It.IsAny<GeneroFilme>()), Times.Never);
            _unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);
        }
        
              

    }
}
