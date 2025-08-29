using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Aplicacao.Compartilhado;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace ControleDeCinema.Aplicacao.ModuloGeneroFilme;

public class GeneroFilmeAppService
{
    private readonly ITenantProvider tenantProvider;
    private readonly IRepositorioGeneroFilme repositorioGeneroFilme;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<GeneroFilmeAppService> logger;

    public GeneroFilmeAppService(
        ITenantProvider tenantProvider,
        IRepositorioGeneroFilme repositorioGeneroFilme,
        IUnitOfWork unitOfWork,
        ILogger<GeneroFilmeAppService> logger
    )
    {
        this.tenantProvider = tenantProvider;
        this.repositorioGeneroFilme = repositorioGeneroFilme;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public Result Cadastrar(GeneroFilme generoFilme)
    {
        var registros = repositorioGeneroFilme.SelecionarRegistros();

        if (registros.Any(i => i.Descricao.Equals(generoFilme.Descricao)))
            return Result.Fail(ResultadosErro.RegistroDuplicadoErro("Já existe um gênero de filme registrado com esta descrição."));

        try
        {
            generoFilme.UsuarioId = tenantProvider.UsuarioId.GetValueOrDefault();

            repositorioGeneroFilme.Cadastrar(generoFilme);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante o registro de {@Registro}.",
                generoFilme
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result Editar(Guid id, GeneroFilme generoFilmeEditado)
    {
        var registros = repositorioGeneroFilme.SelecionarRegistros();

        if (registros.Any(i => !i.Id.Equals(id) && i.Descricao.Equals(generoFilmeEditado.Descricao)))
            return Result.Fail(ResultadosErro.RegistroDuplicadoErro("Já existe um gênero de filme registrado com esta descrição."));

        try
        {
            repositorioGeneroFilme.Editar(id, generoFilmeEditado);

            unitOfWork.Commit();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante a edição do registro {@Registro}.",
                generoFilmeEditado
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result Excluir(Guid id)
    {
        try
        {
            repositorioGeneroFilme.Excluir(id);

            unitOfWork.Commit();

            return Result.Ok();

        }
        catch (Exception ex)
        {
            unitOfWork.Rollback();

            logger.LogError(
                ex,
                "Ocorreu um erro durante a exclusão do registro {Id}.",
                id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<GeneroFilme> SelecionarPorId(Guid id)
    {
        try
        {
            var registroSelecionado = repositorioGeneroFilme.SelecionarRegistroPorId(id);

            if (registroSelecionado is null)
                return Result.Fail(ResultadosErro.RegistroNaoEncontradoErro(id));

            return Result.Ok(registroSelecionado);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção do registro {Id}.",
                id
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }

    public Result<List<GeneroFilme>> SelecionarTodos()
    {
        try
        {
            var registros = repositorioGeneroFilme.SelecionarRegistros();

            return Result.Ok(registros);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção de registros."
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }
}
