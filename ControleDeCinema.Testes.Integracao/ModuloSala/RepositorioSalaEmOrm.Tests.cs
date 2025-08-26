using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Testes.Integracao.Compartilhado;

namespace ControleDeCinema.Testes.Integracao.ModuloSala;

[TestClass]
[TestCategory("Testes de Integração de Sala")]
public sealed class RepositorioSalaEmOrm : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Sala_Corretamente()
    {
        // Arrange
        var sala = new Sala(1, 10);

        // Act
        _repositorioSala?.Cadastrar(sala);
        _dbContext?.SaveChanges();

        // Assert
        var salaSelecionada = _repositorioSala?.SelecionarRegistroPorId(sala.Id);

        Assert.AreEqual(sala, salaSelecionada);
    }

    [TestMethod]
    public void Deve_Editar_Sala_Corretamente()
    {
        // Arrange
        var sala = new Sala(1, 10);
        _repositorioSala?.Cadastrar(sala);
        _dbContext?.SaveChanges();

        var salaEditada = new Sala(2, 20);

        // Act
        var conseguiuEditar = _repositorioSala?.Editar(sala.Id, salaEditada);
        _dbContext?.SaveChanges();

        // Assert
        var salaSelecionada = _repositorioSala?.SelecionarRegistroPorId(sala.Id);
       
        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual(sala, salaSelecionada);
    }

    [TestMethod]
    public void Deve_Excluir_Sala_Corretamente()
    {
        // Arrange
        var sala = new Sala(1, 10);
        _repositorioSala?.Cadastrar(sala);
        _dbContext?.SaveChanges();

        // Act
        var conseguiuExcluir = _repositorioSala?.Excluir(sala.Id);
        _dbContext?.SaveChanges();

        // Assert
        var salaSelecionada = _repositorioSala?.SelecionarRegistroPorId(sala.Id);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(salaSelecionada);
    }

    [TestMethod]
    public void Deve_Selecionar_Todos_As_Salas_Corretamente()
    {
        // Arrange
        var sala1 = new Sala(1, 10);
        var sala2 = new Sala(2, 20);
        var sala3 = new Sala(3, 30);

        List<Sala> salasEsperadas = [sala1, sala2, sala3];

        _repositorioSala?.CadastrarEntidades(salasEsperadas);
        _dbContext?.SaveChanges();

        var salaEsperadasOrdenadas = salasEsperadas
            .OrderBy(x => x.Numero)
            .ToList();

        // Act
        var salaRecebidas = _repositorioSala?.SelecionarRegistros();

        // Assert
        CollectionAssert.AreEqual(salaEsperadasOrdenadas, salaRecebidas);
    }
}
