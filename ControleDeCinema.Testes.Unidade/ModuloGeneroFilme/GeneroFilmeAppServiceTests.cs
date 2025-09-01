using ControleDeCinema.Aplicacao.ModuloGeneroFilme;
using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using Moq;
using Microsoft.Extensions.Logging;

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
    public void Cadastrar_Deve_Retornar_Ok_Quando_Genero_For_Valido()
    {
        // Arrange
        var generoFilme = new GeneroFilme("Ação");
        var generoTeste = new GeneroFilme("Comédia");
        _repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>() { generoTeste });
        _tenantProviderMock?
            .Setup(tp => tp.UsuarioId)
            .Returns(Guid.NewGuid());
        // Act
        var resultado = _generoFilmeApp?.Cadastrar(generoFilme);
        // Assert
        Assert.IsTrue(resultado.IsSuccess);

        _repositorioGeneroFilmeMock?.Verify(r => r.Cadastrar(generoFilme), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_Deve_Retornar_Falha_Quando_Genero_For_Duplicado()
    {
        // Arrange
        var generoFilme = new GeneroFilme("Ação");
        var generoTeste = new GeneroFilme("Ação");

        _repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>() { generoTeste });

        // Act
        var resultado = _generoFilmeApp?.Cadastrar(generoFilme);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        _repositorioGeneroFilmeMock?.Verify(r => r.Cadastrar(It.IsAny<GeneroFilme>()), Times.Never);
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        var genero = new GeneroFilme("Ação");

        _repositorioGeneroFilmeMock!
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>());

        _unitOfWorkMock!
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _generoFilmeApp?.Cadastrar(genero);

        // Assert
        _repositorioGeneroFilmeMock.Verify(r => r.Cadastrar(genero), Times.Once);
        _unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);

        Assert.IsNotNull(resultado);

        var mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Editar_Deve_Retornar_Ok_Quando_O_Genero_For_Valido()
    {
        // Arrange
        var generoExistente = new GeneroFilme("Ação");
        var generoEditado = new GeneroFilme("Aventura");

        _repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(generoExistente.Id))
            .Returns(generoExistente);

        _repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>());

        // Act
        var resultado = _generoFilmeApp!.Editar(generoExistente.Id, generoEditado);

        // Assert
        _repositorioGeneroFilmeMock?.Verify(r => r.Editar(generoExistente.Id, generoEditado), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Editar_Deve_Retornar_Falha_Quando_O_Genero_For_Duplicado()
    {
        // Arrange
        var generoExistente = new GeneroFilme("Ação");
        var generoDuplicado = new GeneroFilme("Romance");
        var generoEditado = new GeneroFilme("Romance");

        _repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(generoExistente.Id))
            .Returns(generoExistente);

        _repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme> { generoDuplicado });

        // Act
        var resultado = _generoFilmeApp!.Editar(generoExistente.Id, generoEditado);

        // Assert
        _repositorioGeneroFilmeMock?.Verify(r => r.Editar(generoExistente.Id, generoEditado), Times.Never);
        _unitOfWorkMock!.Verify(u => u.Commit(), Times.Never);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Editar_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        var generoExistente = new GeneroFilme("Ação");
        var generoEditado = new GeneroFilme("Aventura");

        _repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(generoExistente.Id))
            .Returns(generoExistente);

        _repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<GeneroFilme>());

        _unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _generoFilmeApp?.Editar(generoExistente.Id, generoEditado);

        // Assert
        _repositorioGeneroFilmeMock?.Verify(r => r.Editar(generoExistente.Id, generoEditado), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Rollback(), Times.Once);

        Assert.IsNotNull(resultado);

        var mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Excluir_Deve_Retornar_Ok_Quando_O_Genero_Existir()
    {
        // Arrange
        var generoExistente = new GeneroFilme("Ação");

        _repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(generoExistente.Id))
            .Returns(generoExistente);

        // Act
        var resultado = _generoFilmeApp?.Excluir(generoExistente.Id);

        // Assert
        _repositorioGeneroFilmeMock?.Verify(r => r.Excluir(generoExistente.Id), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Excluir_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        var generoId = Guid.NewGuid();

        _repositorioGeneroFilmeMock?
            .Setup(r => r.Excluir(generoId));

        _unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _generoFilmeApp!.Excluir(generoId);

        // Assert
        _repositorioGeneroFilmeMock?.Verify(r => r.Excluir(generoId), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Rollback(), Times.Once);

        Assert.IsNotNull(resultado);

        var mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void SelecionarTodos_Deve_Retornar_Ok()
    {
        // Arrange
        var generos = new List<GeneroFilme>
        {
            new GeneroFilme("Ação"),
            new GeneroFilme("Comédia")
        };

        _repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(generos);

        // Act
        var resultado = _generoFilmeApp!.SelecionarTodos();
        var generosEsperados = generos;

        // Assert
        _repositorioGeneroFilmeMock?.Verify(r => r.SelecionarRegistros(), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(generosEsperados.Count, resultado.Value.Count);
    }

    [TestMethod]
    public void SelecionarTodos_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        _repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _generoFilmeApp?.SelecionarTodos();

        // Assert
        _repositorioGeneroFilmeMock?.Verify(r => r.SelecionarRegistros(), Times.Once);

        Assert.IsNotNull(resultado);
        var mensagemErro = resultado.Errors.First().Message;
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void SelecionarPorId_Deve_Retornar_Ok_Quando_O_Genero_Existir()
    {
        // Arrange
        var generoExistente = new GeneroFilme("Ação");

        _repositorioGeneroFilmeMock!
            .Setup(r => r.SelecionarRegistroPorId(generoExistente.Id))
            .Returns(generoExistente);

        // Act
        var resultado = _generoFilmeApp!.SelecionarPorId(generoExistente.Id);

        // Assert
        _repositorioGeneroFilmeMock.Verify(r => r.SelecionarRegistroPorId(generoExistente.Id), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(generoExistente, resultado.Value);
    }

    [TestMethod]
    public void SelecionarPorId_Deve_Retornar_Falha_Quando_O_Genero_Nao_Existir()
    {
        // Arrange
        var generoId = Guid.NewGuid();

        _repositorioGeneroFilmeMock!
            .Setup(r => r.SelecionarRegistroPorId(generoId))
            .Returns((GeneroFilme?)null);

        // Act
        var resultado = _generoFilmeApp!.SelecionarPorId(generoId);

        // Assert
        _repositorioGeneroFilmeMock.Verify(r => r.SelecionarRegistroPorId(generoId), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void SelecionarPorId_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        var generoId = Guid.NewGuid();

        _repositorioGeneroFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(generoId))
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _generoFilmeApp?.SelecionarPorId(generoId);

        // Assert
        _repositorioGeneroFilmeMock?.Verify(r => r.SelecionarRegistroPorId(generoId), Times.Once);

        Assert.IsNotNull(resultado);
        var mensagemErro = resultado.Errors.First().Message;
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }
}