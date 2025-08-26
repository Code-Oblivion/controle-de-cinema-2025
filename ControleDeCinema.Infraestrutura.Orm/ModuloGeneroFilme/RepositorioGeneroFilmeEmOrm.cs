using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;

namespace ControleDeCinema.Infraestrutura.Orm.ModuloGeneroFilme;

public class RepositorioGeneroFilmeEmOrm : RepositorioBaseEmOrm<GeneroFilme>, IRepositorioGeneroFilme
{
    public RepositorioGeneroFilmeEmOrm(ControleDeCinemaDbContext contexto) : base(contexto) { }

    public override List<GeneroFilme> SelecionarRegistros() 
    {
        return registros
            .OrderBy(g => g.Descricao)
            .ToList();
    }
}
