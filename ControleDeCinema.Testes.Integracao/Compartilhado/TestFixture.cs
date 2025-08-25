using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using ControleDeCinema.Infraestrutura.Orm.ModuloFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;
using ControleDeCinema.Infraestrutura.Orm.ModuloSala;
using ControleDeCinema.Infraestrutura.Orm.ModuloSessao;
using DotNet.Testcontainers.Containers;
using FizzWare.NBuilder;
using Testcontainers.PostgreSql;

namespace ControleDeCinema.Testes.Integracao.Compartilhado;

[TestClass]
public abstract class TestFixture
{
    protected ControleDeCinemaDbContext? _dbContext;

    protected RepositorioFilmeEmOrm? _repositorioFilme;
    protected RepositorioGeneroFilmeEmOrm? _repositorioGenero;
    protected RepositorioSalaEmOrm? _repositorioSala;
    protected RepositorioSessaoEmOrm? _repositorioSessao;
    protected RepositorioIngressoEmOrm? _repositorioIngresso;

    private static IDatabaseContainer? _container;

    [AssemblyInitialize]
    public static async Task Setup(TestContext _)
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithName("controle-de-cinema-testes")
            .WithDatabase("controle-de-cinema-testes")
            .WithUsername("postgres")
            .WithPassword("YourStrongPassword")
            .WithCleanUp(true)
            .Build();

        await InicializarBancoDadosAsync(_container);
    }

    [AssemblyCleanup]
    public static async Task Teardown()
    {
        await EncerrarBancoDadosAsync();
    }

    [TestInitialize]
    public void ConfigurarTestes()
    {
        if (_container is null)
            throw new ArgumentNullException("O banco de dados não foi inicializado.");

        _dbContext = ControleDeCinemaDbContextFactory.CriarDbContext(_container.GetConnectionString());

        ConfigurarTabelas(_dbContext);

        _repositorioFilme = new RepositorioFilmeEmOrm(_dbContext);
        _repositorioGenero = new RepositorioGeneroFilmeEmOrm(_dbContext);
        _repositorioSala = new RepositorioSalaEmOrm(_dbContext);
        _repositorioSessao = new RepositorioSessaoEmOrm(_dbContext);
        _repositorioIngresso = new RepositorioIngressoEmOrm(_dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Filme>(_repositorioFilme.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<Filme>>(_repositorioFilme.CadastrarEntidades);

        BuilderSetup.SetCreatePersistenceMethod<GeneroFilme>(_repositorioGenero.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<GeneroFilme>>(_repositorioGenero.CadastrarEntidades);

        BuilderSetup.SetCreatePersistenceMethod<Sala>(_repositorioSala.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<Sala>>(_repositorioSala.CadastrarEntidades);

        BuilderSetup.SetCreatePersistenceMethod<Sessao>(_repositorioSessao.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<Sessao>>(_repositorioSessao.CadastrarEntidades);
    }

    private static void ConfigurarTabelas(ControleDeCinemaDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();

        dbContext.Filmes.RemoveRange(dbContext.Filmes);
        dbContext.GenerosFilme.RemoveRange(dbContext.GenerosFilme);
        dbContext.Salas.RemoveRange(dbContext.Salas);
        dbContext.Sessoes.RemoveRange(dbContext.Sessoes);
        dbContext.Ingressos.RemoveRange(dbContext.Ingressos);

        dbContext.SaveChanges();
    }

    private static async Task InicializarBancoDadosAsync(IDatabaseContainer container)
    {
        await container.StartAsync();
    }

    private static async Task EncerrarBancoDadosAsync()
    {
        if (_container is null)
            throw new ArgumentNullException("O Banco de dados não foi inicializado.");

        await _container.StopAsync();
        await _container.DisposeAsync();
    }
}