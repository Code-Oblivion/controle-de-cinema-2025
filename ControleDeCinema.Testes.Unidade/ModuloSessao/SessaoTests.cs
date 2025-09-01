
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;

namespace ControleDeCinema.Testes.Unidade.ModuloSessao;
[TestClass]
[TestCategory("Teste de Unidade de Entidade: Sessao")]
public sealed class SessaoTests
{
    private static (GeneroFilme genero, Filme filme, Sala sala) Dados()
    {
        var genero = new GeneroFilme("Ação");
        var filme = new Filme("Inception", 120, false, genero);
        var sala = new Sala(1, 5);

        return (genero, filme, sala);
    }

    private static Sessao NovaSessao(
        DateTime? inicio = null,
        int? max = null,
        Filme? filme = null,
        Sala? sala = null)
    {
        var (g, f, s) = Dados();

        return new Sessao(
            inicio ?? DateTime.UtcNow,
            max ?? 5,
            filme ?? f,
            sala ?? s
        );
    }

    [TestMethod]
    public void GerarIngresso_Deve_Adicionar_Ao_Agregado_E_Retornar_Instancia_Com_Referencia()
    {
        // Arrange
        var sessao = NovaSessao();

        // Act
        var ing = sessao.GerarIngresso(3, true);

        // Assert
        Assert.AreEqual(1, sessao.Ingressos.Count);
        Assert.AreSame(ing, sessao.Ingressos.Single());
        Assert.AreEqual(3, ing.NumeroAssento);
        Assert.IsTrue(ing.MeiaEntrada);
        Assert.AreSame(sessao, ing.Sessao);
    }

    [TestMethod]
    public void ObterAssentosDisponiveis_Deve_Excluir_Assentos_Ja_Ocupados()
    {
        // Arrange
        var sessao = NovaSessao(max: 5);

        sessao.GerarIngresso(2, false);
        sessao.GerarIngresso(4, true);

        // Act
        var disponiveis = sessao.ObterAssentosDisponiveis();
        var qtdDisp = sessao.ObterQuantidadeIngressosDisponiveis();

        // Assert
        CollectionAssert.AreEqual(new[] { 1, 3, 5 }, disponiveis);
        Assert.AreEqual(3, qtdDisp);
    }

    [TestMethod]
    public void ObterAssentosDisponiveis_Deve_Retornar_Vazio_Quando_Todos_Ocupados()
    {
        // Arrange
        var sessao = NovaSessao(max: 3);

        sessao.GerarIngresso(1, false);
        sessao.GerarIngresso(2, false);
        sessao.GerarIngresso(3, false);

        // Act
        var disponiveis = sessao.ObterAssentosDisponiveis();
        var qtdDisp = sessao.ObterQuantidadeIngressosDisponiveis();

        // Assert
        Assert.AreEqual(0, disponiveis.Length);
        Assert.AreEqual(0, qtdDisp);
    }

    [TestMethod]
    public void Encerrar_Deve_Marcar_Sessao_Como_Encerrada()
    {
        // Arrange
        var sessao = NovaSessao();
        Assert.IsFalse(sessao.Encerrada);

        // Act
        sessao.Encerrar();

        // Assert
        Assert.IsTrue(sessao.Encerrada);
    }

    [TestMethod]
    public void AtualizarRegistro_Deve_Alterar_Inicio_E_NumeroMaximo_Sem_Mexer_Nos_Demais()
    {
        // Arrange
        var (genero, filme, sala) = Dados();
        var sessao = new Sessao(new DateTime(2025, 8, 30, 14, 0, 0, DateTimeKind.Utc), 5, filme, sala);

        var idOriginal = sessao.Id;
        var filmeOriginal = sessao.Filme;
        var salaOriginal = sessao.Sala;

        var ingOriginal = sessao.GerarIngresso(1, false);

        var novaData = new DateTime(2025, 9, 1, 10, 0, 0, DateTimeKind.Utc);
        var sessaoEditada = new Sessao(novaData, 7, filme, sala);

        // Act
        sessao.AtualizarRegistro(sessaoEditada);

        // Assert
        Assert.AreEqual(novaData, sessao.Inicio);
        Assert.AreEqual(7, sessao.NumeroMaximoIngressos);

        Assert.AreEqual(idOriginal, sessao.Id, "Id não deve ser alterado por AtualizarRegistro.");
        Assert.AreSame(filmeOriginal, sessao.Filme, "Filme não deve ser alterado por AtualizarRegistro.");
        Assert.AreSame(salaOriginal, sessao.Sala, "Sala não deve ser alterada por AtualizarRegistro.");
        Assert.IsFalse(sessao.Encerrada, "Encerrada não deve ser alterada por AtualizarRegistro.");
        Assert.AreEqual(1, sessao.Ingressos.Count, "Lista de ingressos não deve ser substituída.");
        Assert.AreSame(ingOriginal, sessao.Ingressos.Single());
    }
}