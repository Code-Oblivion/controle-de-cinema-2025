
using ControleDeCinema.Aplicacao.ModuloSessao;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloSessao;
using Microsoft.Extensions.Logging;
using Moq;

namespace ControleDeCinema.Testes.Unidade.ModuloIngresso;
[TestClass]
[TestCategory("Teste de Unidade de Ingresso")]
public sealed class IngressoAppServiceTests
{
    private Mock<ITenantProvider>? _tenantProviderMock;
    private Mock<IRepositorioIngresso>? _repositorioIngressoMock;
    private Mock<ILogger<IngressoAppService>>? _loggerMock;

    private IngressoAppService? _service;

    [TestInitialize]
    public void Setup()
    {
        _tenantProviderMock = new Mock<ITenantProvider>();
        _repositorioIngressoMock = new Mock<IRepositorioIngresso>();
        _loggerMock = new Mock<ILogger<IngressoAppService>>();

        _service = new IngressoAppService(
            _tenantProviderMock.Object,
            _repositorioIngressoMock.Object,
            _loggerMock.Object
        );
    }

    [TestMethod]
    public void SelecionarTodos_Deve_Retornar_Ok_Com_Lista_Do_Usuario()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        _tenantProviderMock?
            .Setup(tp => tp.UsuarioId)
            .Returns(usuarioId);

        var ingressos = new List<Ingresso>
            {
                new Ingresso(1, false, null!),
                new Ingresso(2, true, null!)
            };

        _repositorioIngressoMock?
            .Setup(r => r.SelecionarRegistros(usuarioId))
            .Returns(ingressos);

        // Act
        var resultado = _service?.SelecionarTodos();

        // Assert
        _repositorioIngressoMock?.Verify(r => r.SelecionarRegistros(usuarioId), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsSuccess);
        CollectionAssert.AreEqual(ingressos, resultado.Value);
    }

    [TestMethod]
    public void SelecionarTodos_Deve_Retornar_Ok_Com_Lista_Vazia()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        _tenantProviderMock?
            .Setup(tp => tp.UsuarioId)
            .Returns(usuarioId);

        _repositorioIngressoMock?
            .Setup(r => r.SelecionarRegistros(usuarioId))
            .Returns(new List<Ingresso>());

        // Act
        var resultado = _service?.SelecionarTodos();

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(resultado.Value);
        Assert.AreEqual(0, resultado.Value.Count);
    }

    [TestMethod]
    public void SelecionarTodos_Deve_Retornar_Falha_Quando_Excecao_For_Lancada()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        _tenantProviderMock?
            .Setup(tp => tp.UsuarioId)
            .Returns(usuarioId);

        _repositorioIngressoMock?
            .Setup(r => r.SelecionarRegistros(usuarioId))
            .Throws(new Exception("Erro Esperado"));

        // Act
        var resultado = _service?.SelecionarTodos();

        // Assert
        _repositorioIngressoMock?.Verify(r => r.SelecionarRegistros(usuarioId), Times.Once);

        Assert.IsNotNull(resultado);
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Ocorreu um erro interno do servidor", resultado.Errors.First().Message);
    }
}