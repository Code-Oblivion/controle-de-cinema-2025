using ControleDeCinema.Dominio.ModuloGeneroFilme;
using System.ComponentModel.DataAnnotations;

namespace ControleDeCinema.WebApp.Models;

public abstract class FormularioGeneroFilmeViewModel
{
    [Required(ErrorMessage = "O campo \"Descrição\" é obrigatório.")]
    [MinLength(2, ErrorMessage = "O campo \"Descrição\" precisa conter ao menos 2 caracteres.")]
    [MaxLength(100, ErrorMessage = "O campo \"Descrição\" precisa conter no máximo 50 caracteres.")]
    public string? Descricao { get; set; }

    public static GeneroFilme ParaEntidade(FormularioGeneroFilmeViewModel viewModel)
    {
        return new GeneroFilme(viewModel.Descricao ?? string.Empty);
    }
}

public class CadastrarGeneroFilmeViewModel : FormularioGeneroFilmeViewModel
{
    public CadastrarGeneroFilmeViewModel() { }
}

public class EditarGeneroFilmeViewModel : FormularioGeneroFilmeViewModel
{
    public Guid Id { get; set; }

    public EditarGeneroFilmeViewModel() { }

    public EditarGeneroFilmeViewModel(Guid id, string descricao) : this()
    {
        Id = id;
        Descricao = descricao;
    }
}

public class ExcluirGeneroFilmeViewModel
{
    public Guid Id { get; set; }
    public string Descricao { get; set; }

    public ExcluirGeneroFilmeViewModel() { }
    public ExcluirGeneroFilmeViewModel(Guid id, string descricao)
    {
        Id = id;
        Descricao = descricao;
    }
}

public class VisualizarGenerosFilmeViewModel
{
    public List<DetalhesGeneroFilmeViewModel> Registros { get; set; }

    public VisualizarGenerosFilmeViewModel(List<GeneroFilme> GeneroFilmes)
    {
        Registros = new List<DetalhesGeneroFilmeViewModel>();

        foreach (var d in GeneroFilmes)
        {
            var detalhesVm = DetalhesGeneroFilmeViewModel.ParaDetalhesVm(d);

            Registros.Add(detalhesVm);
        }
    }
}

public class DetalhesGeneroFilmeViewModel
{
    public Guid Id { get; set; }
    public string Descricao { get; set; }

    public DetalhesGeneroFilmeViewModel(Guid id, string nome)
    {
        Id = id;
        Descricao = nome;
    }

    public static DetalhesGeneroFilmeViewModel ParaDetalhesVm(GeneroFilme generoFilme)
    {
        return new DetalhesGeneroFilmeViewModel(generoFilme.Id, generoFilme.Descricao);
    }
}