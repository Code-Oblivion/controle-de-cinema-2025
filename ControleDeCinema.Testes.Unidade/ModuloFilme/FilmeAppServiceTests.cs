using ControleDeCinema.Aplicacao.ModuloFilme;
using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Logging;

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
    public void Cadastrar_Deve_Retornar_Ok_Quando_O_Filme_For_Valido()
    {
        // Arrange
        var genero = new GeneroFilme("Ação");

        var filme = new Filme("Inception", 148, false, genero);
        var filmeteste = new Filme("Avatar", 162, true, genero);

        _repositorioFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>() { filmeteste });

        // Act
        var resultado = _filmeAppServiceTests!.Cadastrar(filme);

        // Assert
        _repositorioFilmeMock?.Verify(r => r.Cadastrar(filme), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Cadastrar_Deve_Retornar_Falha_Quando_O_Filme_For_Duplicado()
    {
        // Arrange
        var genero = new GeneroFilme("Ação");

        var filme = new Filme("Inception", 148, false, genero);
        var filmeTeste = new Filme("Inception", 150, true, genero);

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
    public void Cadastrar_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
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

    [TestMethod]
    public void Editar_Deve_Retornar_Ok_Quando_O_Filme_For_Valido()
    {
        // Arrange
        var genero = new GeneroFilme("Ação");
        var filmeExistente = new Filme("Inception", 148, false, genero);
        var filmeEditado = new Filme("Inception 2", 150, true, genero);

        _repositorioFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(filmeExistente.Id))
            .Returns(filmeExistente);

        _repositorioFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>());

        // Act
        var resultado = _filmeAppServiceTests?.Editar(filmeExistente.Id, filmeEditado);

        // Assert
        _repositorioFilmeMock?.Verify(r => r.Editar(filmeExistente.Id, filmeEditado), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Editar_Deve_Retornar_Falha_Quando_O_Filme_For_Duplicado()
    {
        // Arrange
        var genero = new GeneroFilme("Ação");

        var filmeExistente = new Filme("Inception", 148, false, genero);
        var filmeDuplicado = new Filme("Avatar", 162, true, genero);
        var filmeEditado = new Filme("Avatar", 150, true, genero);

        _repositorioFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(filmeExistente.Id))
            .Returns(filmeExistente);

        _repositorioFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme> { filmeDuplicado });
        // Act
        var resultado = _filmeAppServiceTests?.Editar(filmeExistente.Id, filmeEditado);

        // Assert
        _repositorioFilmeMock?.Verify(r => r.Editar(filmeExistente.Id, filmeEditado), Times.Never);
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Editar_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        var genero = new GeneroFilme("Ação");

        var filmeExistente = new Filme("Inception", 148, false, genero);
        var filmeEditado = new Filme("Inception 2", 150, true, genero);

        _repositorioFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(filmeExistente.Id))
            .Returns(filmeExistente);

        _repositorioFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Filme>());

        _unitOfWorkMock?
            .Setup(r => r.Commit())
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _filmeAppServiceTests?.Editar(filmeExistente.Id, filmeEditado);

        // Assert
        _repositorioFilmeMock?.Verify(r => r.Editar(filmeExistente.Id, filmeEditado), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Rollback(), Times.Once);

        Assert.IsNotNull(resultado);

        var mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Excluir_Deve_Retornar_Ok_Quando_O_Filme_Existir()
    {
        // Arrange
        var genero = new GeneroFilme("Ação");
        var filmeExistente = new Filme("Inception", 148, false, genero);

        _repositorioFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(filmeExistente.Id))
            .Returns(filmeExistente);

        // Act
        var resultado = _filmeAppServiceTests?.Excluir(filmeExistente.Id);

        // Assert
        _repositorioFilmeMock?.Verify(r => r.Excluir(filmeExistente.Id), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Excluir_Deve_Retornar_Falha_Quando_Banco_Impedir_Exclusao_Por_Relacionamento()
    {
        // Arrange
        var filmeId = Guid.NewGuid();

        _repositorioFilmeMock?
            .Setup(r => r.Excluir(filmeId));

        _unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new DbUpdateException("Violação de chave estrangeira (Sessao->Filme).", new Exception()));

        // Act
        var resultado = _filmeAppServiceTests?.Excluir(filmeId);

        // Assert
        _repositorioFilmeMock?.Verify(r => r.Excluir(filmeId), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Rollback(), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Excluir_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        var filmeId = Guid.NewGuid();

        _repositorioFilmeMock?
            .Setup(r => r.Excluir(filmeId));

        _unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _filmeAppServiceTests?.Excluir(filmeId);

        // Assert
        _repositorioFilmeMock?.Verify(r => r.Excluir(filmeId), Times.Once);
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
        var genero = new GeneroFilme("Ação");
        var filmes = new List<Filme>
        {
            new Filme("Inception", 148, false, genero),
            new Filme("Avatar", 162, true, genero)
        };

        _repositorioFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(filmes);

        // Act
        var resultado = _filmeAppServiceTests?.SelecionarTodos();
        List<Filme> filmesEsperados = filmes;

        // Assert
        _repositorioFilmeMock?.Verify(r => r.SelecionarRegistros(), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(filmesEsperados.Count, resultado.Value.Count);
    }

    [TestMethod]
    public void SelecionarTodos_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        _repositorioFilmeMock?
            .Setup(r => r.SelecionarRegistros())
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _filmeAppServiceTests?.SelecionarTodos();

        // Assert
        _repositorioFilmeMock?.Verify(r => r.SelecionarRegistros(), Times.Once);

        Assert.IsNotNull(resultado);

        var mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void SelecionarPorId_Deve_Retornar_Ok_Quando_O_Filme_Existir()
    {
        // Arrange
        var genero = new GeneroFilme("Ação");
        var filmeExistente = new Filme("Inception", 148, false, genero);

        _repositorioFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(filmeExistente.Id))
            .Returns(filmeExistente);

        // Act
        var resultado = _filmeAppServiceTests?.SelecionarPorId(filmeExistente.Id);

        // Assert
        _repositorioFilmeMock?.Verify(r => r.SelecionarRegistroPorId(filmeExistente.Id), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(filmeExistente, resultado.Value);
    }

    [TestMethod]
    public void SelecionarPorId_Deve_Retornar_Falha_Quando_O_Filme_Nao_Existir()
    {
        // Arrange
        var filmeId = Guid.NewGuid();

        _repositorioFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(filmeId))
            .Returns((Filme?)null);

        // Act
        var resultado = _filmeAppServiceTests?.SelecionarPorId(filmeId);

        // Assert
        _repositorioFilmeMock?.Verify(r => r.SelecionarRegistroPorId(filmeId), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void SelecionarPorId_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        var filmeId = Guid.NewGuid();

        _repositorioFilmeMock?
            .Setup(r => r.SelecionarRegistroPorId(filmeId))
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _filmeAppServiceTests?.SelecionarPorId(filmeId);

        // Assert
        _repositorioFilmeMock?.Verify(r => r.SelecionarRegistroPorId(filmeId), Times.Once);

        Assert.IsNotNull(resultado);
        var mensagemErro = resultado.Errors.First().Message;
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }
}