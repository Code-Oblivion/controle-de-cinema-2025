using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.ModuloSessao;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidade.ModuloSessao;
[TestClass]
[TestCategory("Teste de Unidade de Sessão")]
public sealed class SessaoAppServiceTests
{
    private Mock<ITenantProvider>? _tenantProviderMock;
    private Mock<IRepositorioSessao>? _repositorioSessaoMock;
    private Mock<IUnitOfWork>? _unitOfWorkMock;
    private Mock<ILogger<SessaoAppService>>? _loggerMock;

    private SessaoAppService? _sessaoAppService;

    private static (GeneroFilme genero, Filme filme, Sala sala) DadosBasicos()
    {
        var genero = new GeneroFilme("Ação");
        var filme = new Filme("Inception", 120, false, genero);
        var sala = new Sala(1, 100);
        return (genero, filme, sala);
    }

    private static Sessao NovaSessao(DateTime inicio, int maxIngressos, Filme filme, Sala sala)
        => new Sessao(inicio, maxIngressos, filme, sala);

    [TestInitialize]
    public void Setup()
    {
        _tenantProviderMock = new Mock<ITenantProvider>();
        _repositorioSessaoMock = new Mock<IRepositorioSessao>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<SessaoAppService>>();

        _sessaoAppService = new SessaoAppService(
            _tenantProviderMock.Object,
            _repositorioSessaoMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object
        );
    }

    [TestMethod]
    public void Cadastrar_Deve_Retornar_Ok_Quando_Sessao_For_Valida()
    {
        // Arrange
        var (_, filme, sala) = DadosBasicos();
        var sessao = NovaSessao(DateTime.UtcNow.AddHours(1), 50, filme, sala);

        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>());

        _tenantProviderMock?
            .Setup(tp => tp.UsuarioId)
            .Returns(Guid.NewGuid());

        // Act
        var resultado = _sessaoAppService!.Cadastrar(sessao);

        // Assert
        _repositorioSessaoMock?.Verify(r => r.Cadastrar(sessao), Times.Once);
        _unitOfWorkMock!.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Cadastrar_Deve_Retornar_Falha_Quando_NumeroMaximoIngressos_Exceder_CapacidadeDaSala()
    {
        // Arrange
        var (_, filme, sala) = DadosBasicos();
        var sessao = NovaSessao(DateTime.UtcNow.AddHours(1), 101, filme, sala);

        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>());

        // Act
        var resultado = _sessaoAppService?.Cadastrar(sessao);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        _repositorioSessaoMock?.Verify(r => r.Cadastrar(It.IsAny<Sessao>()), Times.Never);
        _unitOfWorkMock!.Verify(u => u.Commit(), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_Deve_Retornar_Falha_Quando_Houver_Duplicidade_Por_Sala_E_Horario()
    {
        // Arrange
        var (_, filme, sala) = DadosBasicos();
        var inicio = DateTime.UtcNow.AddHours(1);

        var existente = NovaSessao(inicio, 50, filme, sala);
        var nova = NovaSessao(inicio, 40, filme, sala);

        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao> { existente });

        // Act
        var resultado = _sessaoAppService?.Cadastrar(nova);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        _repositorioSessaoMock?.Verify(r => r.Cadastrar(It.IsAny<Sessao>()), Times.Never);
        _unitOfWorkMock!.Verify(u => u.Commit(), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        var (_, filme, sala) = DadosBasicos();
        var sessao = NovaSessao(DateTime.UtcNow.AddHours(1), 50, filme, sala);

        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>());

        _tenantProviderMock?
            .Setup(tp => tp.UsuarioId)
            .Returns(Guid.NewGuid());

        _unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _sessaoAppService?.Cadastrar(sessao);

        // Assert
        _repositorioSessaoMock?.Verify(r => r.Cadastrar(sessao), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Rollback(), Times.Once);

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Ocorreu um erro interno do servidor", resultado.Errors.First().Message);
    }

    [TestMethod]
    public void Editar_Deve_Retornar_Ok_Quando_Sessao_For_Valida()
    {
        // Arrange
        var (_, filme, sala) = DadosBasicos();
        var existente = NovaSessao(DateTime.UtcNow.AddHours(1), 50, filme, sala);
        var editada = NovaSessao(existente.Inicio.AddHours(2), 60, filme, sala);

        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>());

        _repositorioSessaoMock?
            .Setup(r => r.Editar(existente.Id, editada))
            .Returns(true);

        // Act
        var resultado = _sessaoAppService?.Editar(existente.Id, editada);

        // Assert
        _repositorioSessaoMock?.Verify(r => r.Editar(existente.Id, editada), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Editar_Deve_Retornar_Falha_Quando_Sessao_Duplicar_Sala_E_Horario()
    {
        // Arrange
        var (_, filme, sala) = DadosBasicos();

        var existente = NovaSessao(DateTime.UtcNow.AddHours(1), 50, filme, sala);

        var outraComMesmoHorario = NovaSessao(existente.Inicio, 70, filme, sala);
        
        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao> { outraComMesmoHorario });

        // Act
        var resultado = _sessaoAppService!.Editar(existente.Id, outraComMesmoHorario);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        _repositorioSessaoMock?.Verify(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Sessao>()), Times.Never);
        _unitOfWorkMock!.Verify(u => u.Commit(), Times.Never);
    }

    [TestMethod]
    public void Editar_Deve_Retornar_Falha_Quando_Registro_Nao_Existir()
    {
        // Arrange
        var (_, filme, sala) = DadosBasicos();
        var alvoId = Guid.NewGuid();
        var editada = NovaSessao(DateTime.UtcNow.AddHours(1), 40, filme, sala);

        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>());

        _repositorioSessaoMock?
            .Setup(r => r.Editar(alvoId, editada))
            .Returns(false);

        // Act
        var resultado = _sessaoAppService?.Editar(alvoId, editada);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        _unitOfWorkMock!.Verify(u => u.Commit(), Times.Never);
    }

    [TestMethod]
    public void Editar_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        var (_, filme, sala) = DadosBasicos();
        var existente = NovaSessao(DateTime.UtcNow.AddHours(1), 50, filme, sala);
        var editada = NovaSessao(DateTime.UtcNow.AddHours(3), 60, filme, sala);

        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(new List<Sessao>());

        _repositorioSessaoMock?
            .Setup(r => r.Editar(existente.Id, editada))
            .Returns(true);

        _unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _sessaoAppService?.Editar(existente.Id, editada);

        // Assert
        _repositorioSessaoMock?.Verify(r => r.Editar(existente.Id, editada), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Rollback(), Times.Once);

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Ocorreu um erro interno do servidor", resultado.Errors.First().Message);
    }

    [TestMethod]
    public void Excluir_Deve_Retornar_Ok_Quando_Exclusao_For_Concluida()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repositorioSessaoMock?
            .Setup(r => r.Excluir(id))
            .Returns(true);

        // Act
        var resultado = _sessaoAppService?.Excluir(id);

        // Assert
        _repositorioSessaoMock?.Verify(r => r.Excluir(id), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public void Excluir_Deve_Retornar_Falha_Quando_Registro_Nao_Existir()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repositorioSessaoMock?
            .Setup(r => r.Excluir(id))
            .Returns(false);

        // Act
        var resultado = _sessaoAppService?.Excluir(id);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);
    }

    [TestMethod]
    public void Excluir_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repositorioSessaoMock?
            .Setup(r => r.Excluir(id))
            .Returns(true);

        _unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _sessaoAppService!.Excluir(id);

        // Assert
        _repositorioSessaoMock?.Verify(r => r.Excluir(id), Times.Once);
        _unitOfWorkMock?.Verify(u => u.Rollback(), Times.Once);

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Ocorreu um erro interno do servidor", resultado.Errors.First().Message);
    }

    [TestMethod]
    public void SelecionarPorId_Deve_Retornar_Ok_Quando_Existe()
    {
        // Arrange
        var (_, filme, sala) = DadosBasicos();
        var sessao = NovaSessao(DateTime.UtcNow, 50, filme, sala);

        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistroPorId(sessao.Id))
            .Returns(sessao);

        // Act
        var resultado = _sessaoAppService?.SelecionarPorId(sessao.Id);

        // Assert
        _repositorioSessaoMock?.Verify(r => r.SelecionarRegistroPorId(sessao.Id), Times.Once);
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(sessao, resultado.Value);
    }

    [TestMethod]
    public void SelecionarPorId_Deve_Retornar_Falha_Quando_Nao_Existir()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistroPorId(id))
            .Returns((Sessao?)null);

        // Act
        var resultado = _sessaoAppService?.SelecionarPorId(id);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
    }

    [TestMethod]
    public void SelecionarPorId_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistroPorId(id))
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _sessaoAppService?.SelecionarPorId(id);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Ocorreu um erro interno do servidor", resultado.Errors.First().Message);
    }

    [TestMethod]
    public void SelecionarTodos_Deve_Trazer_Apenas_Do_Usuario_Quando_Role_Empresa()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        _tenantProviderMock?
            .Setup(tp => tp.IsInRole("Empresa"))
            .Returns(true);

        _tenantProviderMock?
            .Setup(tp => tp.UsuarioId)
            .Returns(usuarioId);

        var listaUsuario = new List<Sessao> { };
        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistrosDoUsuario(usuarioId))
            .Returns(listaUsuario);

        // Act
        var resultado = _sessaoAppService?.SelecionarTodos();

        // Assert
        _repositorioSessaoMock?.Verify(r => r.SelecionarRegistrosDoUsuario(usuarioId), Times.Once);
        _repositorioSessaoMock?.Verify(r => r.SelecionarRegistros(), Times.Never);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(listaUsuario, resultado.Value);
    }

    [TestMethod]
    public void SelecionarTodos_Deve_Trazer_Todos_Quando_Role_Cliente()
    {
        // Arrange
        _tenantProviderMock?
            .Setup(tp => tp.IsInRole("Empresa"))
            .Returns(false);

        _tenantProviderMock?
            .Setup(tp => tp.IsInRole("Cliente"))
            .Returns(true);

        var todas = new List<Sessao> { };
        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistros())
            .Returns(todas);

        // Act
        var resultado = _sessaoAppService!.SelecionarTodos();

        // Assert
        _repositorioSessaoMock?.Verify(r => r.SelecionarRegistros(), Times.Once);
        _repositorioSessaoMock?.Verify(r => r.SelecionarRegistrosDoUsuario(It.IsAny<Guid>()), Times.Never);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(todas, resultado.Value);
    }

     [TestMethod]
    public void Encerrar_Deve_Retornar_Ok_Quando_Sessao_Existir()
    {
        // Arrange
        var (_, filme, sala) = DadosBasicos();
        var sessao = NovaSessao(DateTime.UtcNow, 50, filme, sala);

        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistroPorId(sessao.Id))
            .Returns(sessao);

        // Act
        var resultado = _sessaoAppService?.Encerrar(sessao.Id);

        // Assert
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsTrue(sessao.Encerrada, "A sessão deveria estar marcada como encerrada.");
    }

    [TestMethod]
    public void Encerrar_Deve_Retornar_Falha_Quando_Sessao_Nao_Existir()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistroPorId(id))
            .Returns((Sessao?)null);

        // Act
        var resultado = _sessaoAppService?.Encerrar(id);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        _unitOfWorkMock!.Verify(u => u.Commit(), Times.Never);
    }

    [TestMethod]
    public void Encerrar_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        var (_, filme, sala) = DadosBasicos();
        var sessao = NovaSessao(DateTime.UtcNow, 50, filme, sala);

        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistroPorId(sessao.Id))
            .Returns(sessao);

        _unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _sessaoAppService?.Encerrar(sessao.Id);

        // Assert
        _unitOfWorkMock?.Verify(u => u.Rollback(), Times.Once);

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Ocorreu um erro interno do servidor", resultado.Errors.First().Message);
    }

    [TestMethod]
    public void VenderIngresso_Deve_Retornar_Ok_Quando_Valido()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var (_, filme, sala) = DadosBasicos();
        var sessao = NovaSessao(DateTime.UtcNow.AddHours(1), 3, filme, sala);

        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistroPorId(sessao.Id))
            .Returns(sessao);

        _tenantProviderMock?
            .Setup(tp => tp.UsuarioId)
            .Returns(usuarioId);

        // Act
        var resultado = _sessaoAppService?.VenderIngresso(sessao.Id, assento: 2, meiaEntrada: true);

        // Assert
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(2, resultado.Value.NumeroAssento);
        Assert.AreEqual(usuarioId, resultado.Value.UsuarioId);
    }

    [TestMethod]
    public void VenderIngresso_Deve_Falhar_Quando_Sessao_Nao_Existir()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistroPorId(id))
            .Returns((Sessao?)null);

        // Act
        var resultado = _sessaoAppService?.VenderIngresso(id, 1, false);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);
    }

    [TestMethod]
    public void VenderIngresso_Deve_Falhar_Quando_Sessao_Estiver_Encerrada()
    {
        // Arrange
        var (_, filme, sala) = DadosBasicos();
        var sessao = NovaSessao(DateTime.UtcNow, 10, filme, sala);
        sessao.Encerrar();

        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistroPorId(sessao.Id))
            .Returns(sessao);

        // Act
        var resultado = _sessaoAppService?.VenderIngresso(sessao.Id, 1, false);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);
    }

    [TestMethod]
    public void VenderIngresso_Deve_Falhar_Quando_Assento_For_Invalido()
    {
        // Arrange
        var (_, filme, sala) = DadosBasicos();
        var sessao = NovaSessao(DateTime.UtcNow, 10, filme, sala);

        _repositorioSessaoMock!
            .Setup(r => r.SelecionarRegistroPorId(sessao.Id))
            .Returns(sessao);

        // Act // Assert
        Assert.IsTrue(_sessaoAppService?.VenderIngresso(sessao.Id, 0, false).IsFailed);  
        Assert.IsTrue(_sessaoAppService?.VenderIngresso(sessao.Id, 11, false).IsFailed);

        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);
    }

    [TestMethod]
    public void VenderIngresso_Deve_Falhar_Quando_Assento_Ja_Estiver_Ocupado()
    {
        // Arrange
        var (_, filme, sala) = DadosBasicos();
        var sessao = NovaSessao(DateTime.UtcNow, 3, filme, sala);

        // Ocupa o assento 2 previamente
        var pre = sessao.GerarIngresso(2, false);

        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistroPorId(sessao.Id))
            .Returns(sessao);

        // Act
        var resultado = _sessaoAppService?.VenderIngresso(sessao.Id, 2, true);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);
    }

    [TestMethod]
    public void VenderIngresso_Deve_Falhar_Quando_Sessao_Estiver_Lotada()
    {
        // Arrange
        var (_, filme, sala) = DadosBasicos();
        var sessao = NovaSessao(DateTime.UtcNow, 2, filme, sala);

        // Lota a sessão
        sessao.GerarIngresso(1, false);
        sessao.GerarIngresso(2, false);
        Assert.AreEqual(0, sessao.ObterQuantidadeIngressosDisponiveis());

        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistroPorId(sessao.Id))
            .Returns(sessao);

        // Act
        var resultado = _sessaoAppService?.VenderIngresso(sessao.Id, 1, false);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        _unitOfWorkMock?.Verify(u => u.Commit(), Times.Never);
    }

    [TestMethod]
    public void VenderIngresso_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        var (_, filme, sala) = DadosBasicos();
        var sessao = NovaSessao(DateTime.UtcNow, 3, filme, sala);

        _repositorioSessaoMock?
            .Setup(r => r.SelecionarRegistroPorId(sessao.Id))
            .Returns(sessao);

        _tenantProviderMock?
            .Setup(tp => tp.UsuarioId)
            .Returns(Guid.NewGuid());

        _unitOfWorkMock?
            .Setup(u => u.Commit())
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _sessaoAppService!.VenderIngresso(sessao.Id, 1, true);

        // Assert
        _unitOfWorkMock?.Verify(u => u.Rollback(), Times.Once);
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Ocorreu um erro interno do servidor", resultado.Errors.First().Message);
    }
}