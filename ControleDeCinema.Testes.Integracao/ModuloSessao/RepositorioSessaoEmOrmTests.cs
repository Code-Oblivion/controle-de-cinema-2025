using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Testes.Integracao.Compartilhado;
using FizzWare.NBuilder;

namespace ControleDeCinema.Testes.Integracao.ModuloSessao;

[TestClass]
[TestCategory("Testes de Integração de Sessão")]
public sealed class RepositorioSessaoEmOrmTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Sessao_Corretamente() 
    {
        // Arrange
        var genero = Builder<GeneroFilme>.CreateNew()
            .Persist();

        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = genero)
            .Persist();

        var sala = Builder<Sala>.CreateNew()
            .Persist();

        var sessao = new Sessao(DateTime.UtcNow, 5, filme, sala);

        // Act
        _repositorioSessao?.Cadastrar(sessao);
        _dbContext?.SaveChanges();

        //Assert
        var sessaoSelecionada= _repositorioSessao?.SelecionarRegistroPorId(sessao.Id);

        Assert.AreEqual(sessao, sessaoSelecionada);
    }

    [TestMethod]
    public void Deve_Editar_Sessao_Corretamente() 
    {
        //Arrange
        var genero = Builder<GeneroFilme>.CreateNew()
            .Persist();

        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = genero)
            .Persist();

        var sala = Builder<Sala>.CreateNew()
            .Persist();

        var sessao = new Sessao(DateTime.UtcNow, 5, filme, sala);

        _repositorioSessao?.Cadastrar(sessao);
        _dbContext?.SaveChanges();

        var sessaoEditada = new Sessao(DateTime.UtcNow.AddHours(2), 10, filme, sala);

        //Act
        var conseguiuEditar = _repositorioSessao?.Editar(sessao.Id, sessaoEditada);
        _dbContext?.SaveChanges();

        //Assert
        var sessaoSelecionada = _repositorioSessao?.SelecionarRegistroPorId(sessao.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.AreEqual(sessao, sessaoSelecionada);
    }

    [TestMethod]
    public void Deve_Excluir_Sessao_Corretamente() 
    {
        // Arrange
        var genero = Builder<GeneroFilme>.CreateNew()
            .Persist();

        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = genero)
            .Persist();

        var sala = Builder<Sala>.CreateNew()
            .Persist();

        var sessao = new Sessao(DateTime.UtcNow, 5, filme, sala);
        _repositorioSessao?.Cadastrar(sessao);
        _dbContext?.SaveChanges();

        // Act
        var conseguiuExcluir = _repositorioSessao?.Excluir(sessao.Id);
        _dbContext?.SaveChanges();

        // Assert
        var sessaoSelecionada = _repositorioSessao?.SelecionarRegistroPorId(sessao.Id);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(sessaoSelecionada);
    }

    [TestMethod]
    public void Deve_Selecionar_Sesso_Por_Id_Corretamente()
    {
        // Arrange
        var genero = Builder<GeneroFilme>.CreateNew()
            .Persist();

        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = genero)
            .Persist();

        var sala = Builder<Sala>.CreateNew()
            .Persist();

        var sessao = new Sessao(DateTime.UtcNow, 5, filme, sala);
        _repositorioSessao?.Cadastrar(sessao);
        _dbContext?.SaveChanges();

        // Act
        var sessaoSelecionada = _repositorioSessao?.SelecionarRegistroPorId(sessao.Id);

        // Assert
        Assert.AreEqual(sessao, sessaoSelecionada);
    }

    [TestMethod]
    public void Deve_Retornar_Null_Ao_Selecionar_Sessao_Por_Id_Inexistente()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        // Act
        var sessaoSelecionada = _repositorioSessao?.SelecionarRegistroPorId(idInexistente);

        // Assert
        Assert.IsNull(sessaoSelecionada);
    }

    [TestMethod]
    public void Deve_Selecionar_Todos_As_Sessao_Corretamente() 
    {
        // Arrange
        var genero = Builder<GeneroFilme>.CreateNew()
            .Persist();

        var filme = Builder<Filme>.CreateNew()
            .With(f => f.Genero = genero)
            .Persist();

        var sala = Builder<Sala>.CreateListOfSize(3)
            .Persist();

        var sessao1 = new Sessao(DateTime.UtcNow, 5, filme, sala[0]);
        var sessao2 = new Sessao(DateTime.UtcNow, 6, filme, sala[1]);
        var sessao3 = new Sessao(DateTime.UtcNow, 7, filme, sala[2]);

        List<Sessao> sessoesEsperadas = [sessao1, sessao2, sessao3];
        _repositorioSessao?.CadastrarEntidades(sessoesEsperadas);
        _dbContext?.SaveChanges();

        var sessoesEsperadasOrdenadas = sessoesEsperadas
            .OrderBy(s => s.NumeroMaximoIngressos)
            .ToList();

        // Act
        var sessoesSelecionadas = _repositorioSessao?.SelecionarRegistros();

        // Assert
        CollectionAssert.AreEqual(sessoesEsperadasOrdenadas, sessoesSelecionadas);
    }

    [TestMethod]
    public void Deve_Selecionar_Sessoes_Do_Usuario_Corretamente()
    {
        

    }
}