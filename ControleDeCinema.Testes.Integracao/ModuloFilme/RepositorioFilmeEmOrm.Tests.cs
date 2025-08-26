using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Testes.Integracao.Compartilhado;
using FizzWare.NBuilder;

namespace ControleDeCinema.Testes.Integracao.ModuloFilme;

[TestClass]
[TestCategory("Testes de Integração de Filme")]
public sealed class RepositorioFilmeEmOrm : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Filme_Corretamente()
    {
        // Arrange
        var genero = Builder<GeneroFilme>.CreateNew().Persist();

        var filme = new Filme("Missão Impossível", 200, true, genero);

        // Act
        _repositorioFilme?.Cadastrar(filme);
        _dbContext?.SaveChanges();

        // Assert
        var filmeSelecionado = _repositorioFilme?.SelecionarRegistroPorId(filme.Id);

        Assert.AreEqual(filme, filmeSelecionado);
    }

    [TestMethod]
    public void Deve_Editar_Filme_Corretamente()
    {
        // Arrange
        var genero = Builder<GeneroFilme>.CreateNew().Persist();

        var filme = new Filme("Missão Impossível", 200, true, genero);
        _repositorioFilme?.Cadastrar(filme);
        _dbContext?.SaveChanges();

        var filmeEditado = new Filme("O Exterminador do Futuro", 150, false, genero);

        // Act
        var conseguiuEditar = _repositorioFilme?.Editar(filme.Id, filmeEditado);
        _dbContext?.SaveChanges();

        // Assert
        var filmeSelecionado = _repositorioFilme?.SelecionarRegistroPorId(filme.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual(filme, filmeSelecionado);
    }

    [TestMethod]
    public void Deve_Excluir_Filme_Corretamente()
    {
        // Arrange
        var genero = Builder<GeneroFilme>.CreateNew().Persist();

        var filme = new Filme("Missão Impossível", 200, true, genero);
        _repositorioFilme?.Cadastrar(filme);
        _dbContext?.SaveChanges();

        // Act
        var conseguiuExcluir = _repositorioFilme?.Excluir(filme.Id);
        _dbContext?.SaveChanges();

        // Assert
        var filmeSelecionado = _repositorioFilme?.SelecionarRegistroPorId(filme.Id);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(filmeSelecionado);
    }

    [TestMethod]
    public void Deve_Selecionar_Filme_Por_Id_Corretamente()
    {
        // Arrange
        var genero = Builder<GeneroFilme>.CreateNew().Persist();
        var filme = new Filme("Missão Impossível", 200, true, genero);
        _repositorioFilme?.Cadastrar(filme);
        _dbContext?.SaveChanges();

        // Act
        var filmeSelecionado = _repositorioFilme?.SelecionarRegistroPorId(filme.Id);

        // Assert
        Assert.IsNotNull(filmeSelecionado);
        Assert.AreEqual(filme, filmeSelecionado);
    }

    [TestMethod]
    public void Deve_Retornar_Null_Ao_Selecionar_Filme_Por_Id_Inexistente()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        // Act
        var filmeSelecionado = _repositorioFilme?.SelecionarRegistroPorId(idInexistente);

        // Assert
        Assert.IsNull(filmeSelecionado);
    }

    [TestMethod]
    public void Deve_Selecionar_Todos_Os_Filmes_Corretamente()
    {
        // Arrange
        var generos = Builder<GeneroFilme>.CreateListOfSize(3).Persist().ToList();

        var filme1 = new Filme("Missão Impossível", 200, true, generos[0]);
        var filme2 = new Filme("O Exterminador do Futuro", 150, false, generos[1]);
        var filme3 = new Filme("Matrix", 180, false, generos[2]);

        List<Filme> filmesEsperados = [filme1, filme2, filme3];
        _repositorioFilme?.CadastrarEntidades(filmesEsperados);
        _dbContext?.SaveChanges();

        var filmesEsperadosOrdenadas = filmesEsperados
            .OrderBy(f => f.Titulo)
            .ToList();

        // Act
        var filmesRecebidos = _repositorioFilme?.SelecionarRegistros();

        // Assert
          CollectionAssert.AreEqual(filmesEsperadosOrdenadas, filmesRecebidos);
    }
}