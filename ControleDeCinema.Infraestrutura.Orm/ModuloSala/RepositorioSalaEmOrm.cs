using ControleDeCinema.Dominio.ModuloGeneroFilme;
using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Infraestrutura.Orm.Compartilhado;

namespace ControleDeCinema.Infraestrutura.Orm.ModuloSala;

public class RepositorioSalaEmOrm : RepositorioBaseEmOrm<Sala>, IRepositorioSala
{
    public RepositorioSalaEmOrm(ControleDeCinemaDbContext contexto) : base(contexto) { }

    public override List<Sala> SelecionarRegistros()
    {
        return registros
            .OrderBy(s => s.Numero)
            .ToList();
    }
}