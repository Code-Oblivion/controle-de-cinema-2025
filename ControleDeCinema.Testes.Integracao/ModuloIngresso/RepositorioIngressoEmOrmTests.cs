using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Testes.Integracao.Compartilhado;
using FizzWare.NBuilder;

namespace ControleDeCinema.Testes.Integracao.ModuloIngresso;

[TestClass]
[TestCategory("Testes de Integração de Ingresso")]
public sealed class RepositorioIngressoEmOrmTests : TestFixture
{
    [TestMethod]
    public void Deve_Selecionar_Todos_Os_Ingressos_Do_Usuario_Corretamente()
    {
        // Arrange
        var usuarioA = Guid.NewGuid();
        var usuarioB = Guid.NewGuid();

        var genero = Builder<GeneroFilme>.CreateNew().Persist();
        var filme = Builder<Filme>.CreateNew().With(f => f.Genero = genero).Persist();
        var salas = Builder<Sala>.CreateListOfSize(2).Persist();

        var sessao1 = new Sessao(DateTime.UtcNow.AddHours(1), 50, filme, salas[0]);
        var sessao2 = new Sessao(DateTime.UtcNow.AddHours(2), 50, filme, salas[1]);

        _dbContext!.Sessoes.AddRange(sessao1, sessao2);
        _dbContext.SaveChanges();

        var ingressoA1 = new Ingresso(5, false, sessao1);
        var ingressoA2 = new Ingresso(2, true, sessao2);
        var ingressoB1 = new Ingresso(9, false, sessao1);

        ingressoA1.UsuarioId = usuarioA;
        ingressoA2.UsuarioId = usuarioA;
        ingressoB1.UsuarioId = usuarioB;

        _dbContext.Ingressos.AddRange(ingressoA1, ingressoA2, ingressoB1);
        _dbContext.SaveChanges();

        var ingressosEsperadosOrdenados = new List<Ingresso> { ingressoA2, ingressoA1 }
            .OrderBy(i => i.NumeroAssento)
            .ToList();

        // Act
        var ingressosSelecionados = _repositorioIngresso?.SelecionarRegistros(usuarioA);

        // Assert
        CollectionAssert.AreEqual(ingressosEsperadosOrdenados, ingressosSelecionados);
    }
}