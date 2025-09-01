using ControleDeCinema.Dominio.ModuloFilme;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ControleDeCinema.WebApp.Models;

public abstract class FormularioFilmeViewModel
{
    [Required(ErrorMessage = "O campo \"Título\" é obrigatório.")]
    [MinLength(2, ErrorMessage = "O campo \"Título\" precisa conter ao menos 2 caracteres.")]
    [MaxLength(100, ErrorMessage = "O campo \"Título\" precisa conter no máximo 50 caracteres.")]
    public string? Titulo { get; set; }

    [Required(ErrorMessage = "O campo \"Duração\" é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "O campo \"Duração\" precisa conter um valor acima de 0.")]
    public int Duracao { get; set; }

    public bool Lancamento { get; set; }

    [Required(ErrorMessage = "O campo \"Gênero\" é obrigatório.")] //corrigido de "lançamento" para "genero"
    public Guid? GeneroId { get; set; }
    public List<SelectListItem>? GenerosDisponiveis { get; set; }

    public static Filme ParaEntidade(FormularioFilmeViewModel viewModel, List<GeneroFilme> generosDisponiveis)
    {
        var generoFilmeSelecionado = generosDisponiveis.Find(g => g.Id.Equals(viewModel.GeneroId));

        if (generoFilmeSelecionado is null)
            throw new ArgumentOutOfRangeException(nameof(viewModel));

        return new Filme(
            viewModel.Titulo ?? string.Empty,
            viewModel.Duracao,
            viewModel.Lancamento,
            generoFilmeSelecionado
        );
    }
}

public class CadastrarFilmeViewModel : FormularioFilmeViewModel
{
    public CadastrarFilmeViewModel() { }

    public CadastrarFilmeViewModel(List<GeneroFilme> generosDisponiveis)
    {
        GenerosDisponiveis = generosDisponiveis
            .Select(g => new SelectListItem(g.Descricao, g.Id.ToString()))
            .ToList();
    }
}

public class EditarFilmeViewModel : FormularioFilmeViewModel
{
    public Guid Id { get; set; }

    public EditarFilmeViewModel() { }

    public EditarFilmeViewModel(Guid id, string titulo, int duracao, bool lancamento, Guid generoId, List<GeneroFilme> generosDisponiveis) : this()
    {
        Id = id;
        Titulo = titulo;
        Duracao = duracao;
        Lancamento = lancamento;
        GeneroId = generoId;

        GenerosDisponiveis = generosDisponiveis
            .Select(g => new SelectListItem(g.Descricao, g.Id.ToString()))
            .ToList();
    }
}

public class ExcluirFilmeViewModel
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }

    public ExcluirFilmeViewModel() { }

    public ExcluirFilmeViewModel(Guid id, string titulo)
    {
        Id = id;
        Titulo = titulo;
    }
}

public class VisualizarFilmesViewModel
{
    public List<DetalhesFilmeViewModel> Registros { get; set; }

    public VisualizarFilmesViewModel(List<Filme> Filmes)
    {
        Registros = Filmes
            .Select(DetalhesFilmeViewModel.ParaDetalhesVm)
            .ToList();
    }
}

public class DetalhesFilmeViewModel
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }
    public int Duracao { get; set; }
    public bool Lancamento { get; set; }
    public string Genero { get; set; }

    public DetalhesFilmeViewModel(Guid id, string titulo, int duracao, bool lancamento, string genero)
    {
        Id = id;
        Titulo = titulo;
        Duracao = duracao;
        Lancamento = lancamento;
        Genero = genero;
    }

    public static DetalhesFilmeViewModel ParaDetalhesVm(Filme filme)
    {
        return new DetalhesFilmeViewModel(
            filme.Id,
            filme.Titulo,
            filme.Duracao,
            filme.Lancamento,
            filme.Genero.Descricao
        );
    }
}