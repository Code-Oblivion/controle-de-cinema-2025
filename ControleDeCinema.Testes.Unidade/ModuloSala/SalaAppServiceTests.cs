using ControleDeCinema.Aplicacao.ModuloSala;
using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloSala;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Logging;

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
    public void Cadastrar_Deve_Retornar_Ok_Quando_Sala_For_Valida()
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
    public void Cadastrar_Deve_Retornar_Falha_Quando_Sala_For_Duplicado()
    {
        // Arrange
        var sala = new Sala(10, 100);
        var salaTeste = new Sala(10, 100);

        _repositorioSalaMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala>() { sala });
        // Act

        var resultado = _salaAppService?.Cadastrar(sala);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        _repositorioSalaMock?.Verify(r => r.Cadastrar(It.IsAny<Sala>()), Times.Never);
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        var sala = new Sala(5, 120);

        _repositorioSalaMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala>());

        _unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _salaAppService!.Cadastrar(sala);

        // Assert
        _repositorioSalaMock?.Verify(r => r.Cadastrar(sala), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Rollback(), Times.Once);

        Assert.IsNotNull(resultado);

        var mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Editar_Deve_Retornar_Ok_Quando_Sala_For_Valida()
    {
        // Arrange
        var existente = new Sala(1, 100);
        var editada = new Sala(1, 120);

        _repositorioSalaMock?
            .Setup(r => r.SelecionarRegistroPorId(existente.Id))
            .Returns(existente);

        _repositorioSalaMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala> { });

        // Act
        var resultado = _salaAppService?.Editar(existente.Id, editada);

        // Assert
        _repositorioSalaMock?.Verify(r => r.Editar(existente.Id, editada), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Editar_Deve_Retornar_Falha_Quando_Sala_For_Duplicado()
    {
        // Arrange
        var existente = new Sala(1, 100);
        var salaQueJaExiste = new Sala(10, 90);
        var editada = new Sala(10, 120);

        _repositorioSalaMock?
            .Setup(r => r.SelecionarRegistroPorId(existente.Id))
            .Returns(existente);

        _repositorioSalaMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala> { salaQueJaExiste });

        // Act
        var resultado = _salaAppService?.Editar(existente.Id, editada);

        // Assert
        _repositorioSalaMock?.Verify(r => r.Editar(existente.Id, editada), Times.Never);
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Editar_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        var existente = new Sala(1, 100);
        var editada = new Sala(1, 120);

        _repositorioSalaMock?
            .Setup(r => r.SelecionarRegistroPorId(existente.Id))
            .Returns(existente);

        _repositorioSalaMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sala>());

        _unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _salaAppService?.Editar(existente.Id, editada);

        // Assert
        _repositorioSalaMock?.Verify(r => r.Editar(existente.Id, editada), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Rollback(), Times.Once);

        Assert.IsNotNull(resultado);

        var mensagemErro = resultado.Errors.First().Message;

        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Excluir_Deve_Retornar_Ok_Quando_Sala_Existir()
    {
        // Arrange
        var sala = new Sala(3, 80);

        _repositorioSalaMock?
            .Setup(r => r.SelecionarRegistroPorId(sala.Id))
            .Returns(sala);

        // Act
        var resultado = _salaAppService?.Excluir(sala.Id);

        // Assert
        _repositorioSalaMock?.Verify(r => r.Excluir(sala.Id), Times.Once);
        _unitOfWorkMock!.Verify(u => u.Commit(), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Excluir_Deve_Retornar_Falha_Quando_Banco_Impedir_Exclusao_Por_Relacionamento()
    {
        // Arrange
        var salaId = Guid.NewGuid();

        _repositorioSalaMock?
            .Setup(r => r.Excluir(salaId));

        _unitOfWorkMock!
            .Setup(u => u.Commit())
            .Throws(new DbUpdateException("Violação de chave estrangeira (Sessao->Sala).", new Exception()));

        // Act
        var resultado = _salaAppService?.Excluir(salaId);

        // Assert
        _repositorioSalaMock?.Verify(r => r.Excluir(salaId), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        _unitOfWorkMock.Verify(u => u.Rollback(), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void Excluir_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        var salaId = Guid.NewGuid();

        _repositorioSalaMock?
            .Setup(r => r.Excluir(salaId));

        _unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _salaAppService?.Excluir(salaId);

        // Assert
        _repositorioSalaMock?.Verify(r => r.Excluir(salaId), Times.Once);
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
        var salas = new List<Sala>
        {
            new Sala(1, 100),
            new Sala(2, 80)
        };

        _repositorioSalaMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(salas);

        // Act
        var resultado = _salaAppService?.SelecionarTodos();
        var esperadas = salas;

        // Assert
        _repositorioSalaMock?.Verify(r => r.SelecionarRegistros(), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(esperadas.Count, resultado.Value.Count);
    }

    [TestMethod]
    public void SelecionarTodos_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        _repositorioSalaMock?
            .Setup(r => r.SelecionarRegistros())
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _salaAppService?.SelecionarTodos();

        // Assert
        _repositorioSalaMock?.Verify(r => r.SelecionarRegistros(), Times.Once);

        Assert.IsNotNull(resultado);
        var mensagemErro = resultado.Errors.First().Message;
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void SelecionarPorId_Deve_Retornar_Ok_Quando_Sala_Existir()
    {
        // Arrange
        var sala = new Sala(7, 150);

        _repositorioSalaMock?
            .Setup(r => r.SelecionarRegistroPorId(sala.Id))
            .Returns(sala);

        // Act
        var resultado = _salaAppService?.SelecionarPorId(sala.Id);

        // Assert
        _repositorioSalaMock?.Verify(r => r.SelecionarRegistroPorId(sala.Id), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(sala, resultado.Value);
    }

    [TestMethod]
    public void SelecionarPorId_Deve_Retornar_Falha_Quando_Sala_Nao_Existir()
    {
        // Arrange
        var salaId = Guid.NewGuid();

        _repositorioSalaMock?
            .Setup(r => r.SelecionarRegistroPorId(salaId))
            .Returns((Sala?)null);

        // Act
        var resultado = _salaAppService?.SelecionarPorId(salaId);

        // Assert
        _repositorioSalaMock?.Verify(r => r.SelecionarRegistroPorId(salaId), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void SelecionarPorId_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        var salaId = Guid.NewGuid();

        _repositorioSalaMock?
            .Setup(r => r.SelecionarRegistroPorId(salaId))
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _salaAppService?.SelecionarPorId(salaId);

        // Assert
        _repositorioSalaMock?.Verify(r => r.SelecionarRegistroPorId(salaId), Times.Once);

        Assert.IsNotNull(resultado);
        var mensagemErro = resultado.Errors.First().Message;
        Assert.AreEqual("Ocorreu um erro interno do servidor", mensagemErro);
        Assert.IsTrue(resultado.IsFailed);
    }
}