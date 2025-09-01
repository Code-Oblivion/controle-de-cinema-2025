using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloFilme;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidade.ModuloFilme
{
    [TestClass]
    [TestCategory("Teste de Unidade de Filme")]
    public sealed class FilmeAppServiceTests
    {
        private Mock<IRepositorioFilme>? _repositorioFilmeMock;       
        private Mock<IUnitOfWork>? _unitOfWorkMock;
        private Mock<ILogger<FilmeAppService>>? _loggerMock;
        private Mock<ITenantProvider>? _tenantProviderMock;

        private FilmeAppService? _filmeAppServiceTests;

        [TestInitialize]
        public void Setup()
        {
            _repositorioFilmeMock = new Mock<IRepositorioFilme>();            
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<FilmeAppService>>();
            _tenantProviderMock = new Mock<ITenantProvider>();

            _filmeAppServiceTests = new FilmeAppService(
                _tenantProviderMock.Object,
                _repositorioFilmeMock.Object,              
                _unitOfWorkMock.Object,
                _loggerMock.Object
            );
        }

        [TestMethod]
        public void Cadastrar_DeveRetornarOk_QuandoFilmeForValido()
        {
            // Arrange
           var filme = new Filme("Inception", 148, false, null);
           var filmeteste = new Filme("Avatar", 162, true, null);

            _repositorioFilmeMock!
                .Setup(r => r.SelecionarRegistros())
                .Returns(new List<Filme>() { filmeteste});

            _tenantProviderMock!
                .Setup(tp => tp.UsuarioId)
                .Returns(Guid.NewGuid());

            // Act
            var resultado = _filmeAppServiceTests!.Cadastrar(filme);

            // Assert
            Assert.IsTrue(resultado.IsSuccess);

            _repositorioFilmeMock.Verify(r => r.Cadastrar(filme), Times.Once);
            _unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.IsSuccess);
        }

        [TestMethod]
        public void Cadastrar_DeveRetornarFalha_QuandoFilmeForDuplicado()
        {
            // Arrange
            var filme = new Filme("Inception", 148, false, null);
            var filmeTeste = new Filme("Inception", 150, true, null);

            _repositorioFilmeMock?
                .Setup(r => r.SelecionarRegistros())
                .Returns(new List<Filme> { filmeTeste });

            // Act
            var resultado = _filmeAppServiceTests?.Cadastrar(filme);

            // Assert
            _repositorioFilmeMock?.Verify(r => r.Cadastrar(filme), Times.Never);
           
            _unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);        

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.IsFailed);           


        }            

        [TestMethod]
        public void Cadastrar_DeveRetornarFalha_QuandoExcecaoForLancada()
        {
            // Arrange
            var generoFilme = new GeneroFilme("Ação");
            var filme = new Filme("Titanic", 120, false, generoFilme);

            _repositorioFilmeMock?
                .Setup(r => r.SelecionarRegistros())
                .Returns(new List<Filme>());

            _unitOfWorkMock?
                .Setup(r => r.Commit())
                .Throws(new Exception("Erro Esperado"));

            // Act
            var resultado = _filmeAppServiceTests?.Cadastrar(filme);

            // Assert
            _repositorioFilmeMock?.Verify(r => r.Cadastrar(filme), Times.Once);

            _unitOfWorkMock?.Verify(u => u.Rollback(), Times.Once);

            Assert.IsNotNull(resultado);


            var mensagemErro = resultado.Errors.First().Message;

            Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
            Assert.IsTrue(resultado.IsFailed);
        }
    }
}
