using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Testes.Integracao.Compartilhado;

namespace ControleDeCinema.Testes.Integracao.ModuloGenero;

[TestClass]
[TestCategory("Teste de Integração de Gênero")]
public sealed class RepositorioGeneroEmOrm : TestFixture
{
    public void Deve_Cadastrar_Genero_Corretmente() 
    {
        // Arrange
        var genero = new GeneroFilme("Terror");
        // Act
        _repositorioGenero?.Cadastrar(genero);
        _dbContext.SaveChanges();

        // Assert
        var registroSelecionado = _repositorioGenero?.SelecionarRegistroPorId(genero.Id);

        Assert.AreEqual(genero, registroSelecionado);
    }

    [TestMethod]
    public void Deve_Editar_Genero_Corretamente()
    {
        // Arrange
        var genero = new GeneroFilme("Terror");
        _repositorioGenero?.Cadastrar(genero);
        _dbContext.SaveChanges();
        var gerenoEditado = new GeneroFilme("Comédia");

        // Act
        var conseguiuEditar = _repositorioGenero?.Editar(genero.Id, gerenoEditado);
        _dbContext.SaveChanges();

        // Assert
        var registroSelecionado = _repositorioGenero?.SelecionarRegistroPorId(genero.Id);
        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual(genero, registroSelecionado);
    }

    [TestMethod]
    public void Deve_Excluir_Genero_Corretamente()
    {
        // Arrange
        var genero = new GeneroFilme("Terror");
        _repositorioGenero?.Cadastrar(genero);
        _dbContext.SaveChanges();

        // Act
        var conseguiuExcluir = _repositorioGenero?.Excluir(genero.Id);
        _dbContext.SaveChanges();

        // Assert
        var registroSelecionado = _repositorioGenero?.SelecionarRegistroPorId(genero.Id);
        
        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(registroSelecionado);
    }

    [TestMethod]
    public void Deve_Selecionar_TodosGeneros_Corretamente()
    {
        // Arrange
        var genero1 = new GeneroFilme("Terror");
        var genero2 = new GeneroFilme("Comédia");
        var genero3 = new GeneroFilme("Ação");

        List<GeneroFilme> generosEsperados = [genero1, genero2, genero3];
        _repositorioGenero?.CadastrarEntidades(generosEsperados);
        _dbContext.SaveChanges();

        var generosEsperadosOrdenadas = generosEsperados
            .OrderBy(g => g.Descricao)
            .ToList();

        // Act
        var registrosSelecionados = _repositorioGenero?.SelecionarRegistros();
        // Assert
        CollectionAssert.AreEqual(generosEsperadosOrdenadas, registrosSelecionados);
    }



}
