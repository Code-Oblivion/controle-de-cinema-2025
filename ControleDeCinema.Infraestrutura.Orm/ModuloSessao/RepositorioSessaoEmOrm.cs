using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloSessao;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace ControleDeCinema.Infraestrutura.Orm.ModuloSessao;

public class RepositorioIngressoEmOrm : IRepositorioIngresso
{
    private readonly DbSet<Ingresso> registros;

    public RepositorioIngressoEmOrm(ControleDeCinemaDbContext contexto)
    {
        registros = contexto.Ingressos;
    }

    public List<Ingresso> SelecionarRegistros(Guid usuarioId)
    {
        return registros
            .IgnoreQueryFilters()
            .Include(x => x.Sessao)
            .ThenInclude(s => s.Filme)
            .Include(x => x.Sessao)
            .ThenInclude(s => s.Sala)
            .Where(x => x.UsuarioId.Equals(usuarioId))
            .OrderBy(x => x.NumeroAssento)
            .ToList();
    }
}

public class RepositorioSessaoEmOrm : RepositorioBaseEmOrm<Sessao>, IRepositorioSessao
{
    public RepositorioSessaoEmOrm(ControleDeCinemaDbContext contexto) : base(contexto) { }

    public override Sessao? SelecionarRegistroPorId(Guid idRegistro)
    {
        return registros
            .IgnoreQueryFilters()
            .Include(x => x.Filme)
            .Include(x => x.Sala)
            .Include(x => x.Ingressos)
            .FirstOrDefault(x => x.Id.Equals(idRegistro));
    }

    public override List<Sessao> SelecionarRegistros()
    {
        return registros
            .IgnoreQueryFilters()
            .Include(x => x.Filme)
            .Include(x => x.Sala)
            .Include(x => x.Ingressos)
            .OrderBy(x => x.NumeroMaximoIngressos)
            .ToList();
    }

    public List<Sessao> SelecionarRegistrosDoUsuario(Guid usuarioId)
    {
        return registros
            .IgnoreQueryFilters()
            .Include(x => x.Filme)
            .Include(x => x.Sala)
            .Include(x => x.Ingressos)
            .Where(x => x.UsuarioId.Equals(usuarioId))
            .OrderBy(x => x.NumeroMaximoIngressos)
            .ToList();
    }
}
