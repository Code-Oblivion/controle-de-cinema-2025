using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Testes.Integracao.Compartilhado;

namespace ControleDeCinema.Testes.Integracao.ModuloGenero;

[TestClass]
[TestCategory("Teste de Integração de Gênero")]
public sealed class RepositorioGeneroEmOrm : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Genero_Corretamente() 
    {
        // Arrange
        var genero = new GeneroFilme("Terror");

        // Act
        _repositorioGenero?.Cadastrar(genero);
        _dbContext?.SaveChanges();

        // Assert
        var generoSelecionado = _repositorioGenero?.SelecionarRegistroPorId(genero.Id);

        Assert.AreEqual(genero, generoSelecionado);
    }

    [TestMethod]
    public void Deve_Editar_Genero_Corretamente()
    {
        // Arrange
        var genero = new GeneroFilme("Terror");
        _repositorioGenero?.Cadastrar(genero);
        _dbContext?.SaveChanges();

        var generoEditado = new GeneroFilme("Comédia");

        // Act
        var conseguiuEditar = _repositorioGenero?.Editar(genero.Id, generoEditado);
        _dbContext?.SaveChanges();

        // Assert
        var generoSelecionado = _repositorioGenero?.SelecionarRegistroPorId(genero.Id);
        
        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual(genero, generoSelecionado);
    }

    [TestMethod]
    public void Deve_Excluir_Genero_Corretamente()
    {
        // Arrange
        var genero = new GeneroFilme("Terror");
        _repositorioGenero?.Cadastrar(genero);
        _dbContext?.SaveChanges();

        // Act
        var conseguiuExcluir = _repositorioGenero?.Excluir(genero.Id);
        _dbContext?.SaveChanges();

        // Assert
        var generoSelecionado = _repositorioGenero?.SelecionarRegistroPorId(genero.Id);
        
        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(generoSelecionado);
    }

    [TestMethod]
    public void Deve_Selecionar_Genero_Por_Id_Corretamente() 
    {
        // Arrange
        var genero = new GeneroFilme("Terror");
        _repositorioGenero?.Cadastrar(genero);
        _dbContext?.SaveChanges();

        // Act
        var generoSelecionado = _repositorioGenero?.SelecionarRegistroPorId(genero.Id);

        // Assert
        Assert.AreEqual(genero, generoSelecionado);
    }

    [TestMethod]
    public void Deve_Retornar_Null_Ao_Selecionar_Genero_Por_Id_Inexistente() 
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        // Act
        var generoSelecionado = _repositorioGenero?.SelecionarRegistroPorId(idInexistente);

        // Assert
        Assert.IsNull(generoSelecionado);
    }

    [TestMethod]
    public void Deve_Selecionar_Todos_os_Generos_Corretamente()
    {
        // Arrange
        var genero1 = new GeneroFilme("Terror");
        var genero2 = new GeneroFilme("Comédia");
        var genero3 = new GeneroFilme("Ação");

        List<GeneroFilme> generosEsperados = [genero1, genero2, genero3];
        _repositorioGenero?.CadastrarEntidades(generosEsperados);
        _dbContext?.SaveChanges();

        var generosEsperadosOrdenadas = generosEsperados
            .OrderBy(g => g.Descricao)
            .ToList();

        // Act
        var generosRecebidos = _repositorioGenero?.SelecionarRegistros();

        // Assert
        CollectionAssert.AreEqual(generosEsperadosOrdenadas, generosRecebidos);
    }
}
