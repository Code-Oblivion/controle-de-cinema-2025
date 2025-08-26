using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace ControleDeCinema.Infraestrutura.Orm.ModuloFilme;

public class RepositorioFilmeEmOrm : RepositorioBaseEmOrm<Filme>, IRepositorioFilme
{
    public RepositorioFilmeEmOrm(ControleDeCinemaDbContext contexto) : base(contexto) { }

    public override Filme? SelecionarRegistroPorId(Guid idRegistro)
    {
        return registros
            .Include(f => f.Genero)
            .FirstOrDefault(f => f.Id.Equals(idRegistro));
    }

    public override List<Filme> SelecionarRegistros()
    {
        return registros
            .Include(f => f.Genero)
            .OrderBy(f => f.Titulo)
            .ToList();
    }
}