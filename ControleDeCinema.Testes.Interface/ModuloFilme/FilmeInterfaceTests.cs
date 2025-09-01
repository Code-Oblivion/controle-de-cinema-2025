using ControleDeCinema.Testes.Interface.Compartilhado;
using ControleDeCinema.Testes.Interface.ModuloGenero;
using OpenQA.Selenium;

namespace ControleDeCinema.Testes.Interface.ModuloFilme;

[TestClass]
[TestCategory("Teste de Interface de Filme")]
public sealed class FilmeInterfaceTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Filme_Corretamente()
    {
        // Arrange
        var GeneroIndexPage = new GeneroIndexPageObject(driver!)
             .IrPara(enderecoBase!);

        GeneroIndexPage
            .ClickCadastrar()
            .PreencherDescricao("Ação")
            .Confirmar();

        var IndexPage = new FilmeIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        // Act
        IndexPage
            .ClickCadastrar()
            .PreencherTitulo("Inception")
            .PreencherDuracao(148)
            .MarcarLancamento(false)
            .SelecionarGenero("Ação")
            .Confirmar();

        // Assert
        Assert.IsTrue(IndexPage.ContemFilme("Inception"));
    }

    [TestMethod]
    public void Deve_Editar_Filme_Corretamente()
    {
        // Arrange
        var GeneroIndexPage = new GeneroIndexPageObject(driver!)
         .IrPara(enderecoBase!);

        GeneroIndexPage
            .ClickCadastrar()
            .PreencherDescricao("Ação")
            .Confirmar();

        var IndexPage = new FilmeIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        IndexPage
          .ClickCadastrar()
          .PreencherTitulo("Avatar")
          .PreencherDuracao(162)
          .MarcarLancamento(false)
          .SelecionarGenero("Ação")
          .Confirmar();

        // Act
        IndexPage
            .ClickEditar()
            .PreencherTitulo("Avatar: The Way of Water")
            .PreencherDuracao(192)
            .MarcarLancamento(true)
            .SelecionarGenero("Ação")
            .Confirmar();

        // Assert
        Assert.IsTrue(IndexPage.ContemFilme("Avatar: The Way of Water"));
    }

    [TestMethod]
    public void Deve_Excluir_Filme_Corretamente()
    {
        // Arrange
        var GeneroIndexPage = new GeneroIndexPageObject(driver!)
         .IrPara(enderecoBase!);

        GeneroIndexPage
            .ClickCadastrar()
            .PreencherDescricao("Romance")
            .Confirmar();

        var IndexPage = new FilmeIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        IndexPage
          .ClickCadastrar()
          .PreencherTitulo("Titanic")
          .PreencherDuracao(195)
          .MarcarLancamento(false)
          .SelecionarGenero("Romance")
          .Confirmar();

        // Act
        IndexPage
            .ClickExcluir()
            .Confirmar();

        // Assert
        Assert.IsFalse(IndexPage.ContemFilme("Titanic"));
    }

    [TestMethod]
    public void Deve_Visualizar_Filme_Corretamente()
    {
        // Arrange
        var GeneroIndexPage = new GeneroIndexPageObject(driver!)
         .IrPara(enderecoBase!);

        GeneroIndexPage
            .ClickCadastrar()
            .PreencherDescricao("Ficção Científica")
            .Confirmar();

        var IndexPage = new FilmeIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        IndexPage
          .ClickCadastrar()
          .PreencherTitulo("Interstellar")
          .PreencherDuracao(169)
          .MarcarLancamento(false)
          .SelecionarGenero("Ficção Científica")
          .Confirmar();

        // Act
        var titulo = driver?.FindElement(By.CssSelector("h5[data-se='Titulo']"))?.Text;

        // Assert
        Assert.AreEqual("Interstellar", titulo);
    }

    [TestMethod]
    public void Deve_Validar_Campos_Obrigatorios_ao_Cadastrar_Filme()
    {
        // Arrange
        var IndexPage = new FilmeIndexPageObject(driver!)
        .IrPara(enderecoBase!);
        // Act
        IndexPage
            .ClickCadastrar()
            .ClicarEmConfirmar();

        var erroTitulo = driver?.FindElement(By.CssSelector("span[data-se='Titulo-erro']"))?.Text;
        var erroDuracao = driver?.FindElement(By.CssSelector("span[data-se='Duracao-erro']"))?.Text;
        var erroGenero = driver?.FindElement(By.CssSelector("span[data-se='Genero-erro']"))?.Text;

        // Assert
        Assert.AreEqual("O campo \"Título\" é obrigatório.", erroTitulo);
        Assert.AreEqual("O campo \"Duração\" precisa conter um valor acima de 0.", erroDuracao);
        Assert.AreEqual("O campo \"Gênero\" é obrigatório.", erroGenero);
    }

    [TestMethod]
    public void Deve_Validar_Campos_Obrigatorios_Ao_Editar_Filme()
    {
        // Arrange
        var GeneroIndexPage = new GeneroIndexPageObject(driver!)
         .IrPara(enderecoBase!);

        GeneroIndexPage
            .ClickCadastrar()
            .PreencherDescricao("Comédia")
            .Confirmar();

        var IndexPage = new FilmeIndexPageObject(driver!)

        .IrPara(enderecoBase!);
        IndexPage
          .ClickCadastrar()
          .PreencherTitulo("The Mask")
          .PreencherDuracao(101)
          .MarcarLancamento(false)
          .SelecionarGenero("Comédia")
          .Confirmar();

        // Act
        IndexPage
            .ClickEditar()
            .PreencherTitulo(string.Empty)
            .PreencherDuracao(0)
            .SelecionarGenero(null)
            .ClicarEmConfirmar();

        var erroTitulo = driver?.FindElement(By.CssSelector("span[data-se='Titulo-erro']"))?.Text;
        var erroDuracao = driver?.FindElement(By.CssSelector("span[data-se='Duracao-erro']"))?.Text;
        var erroGenero = driver?.FindElement(By.CssSelector("span[data-se='Genero-erro']"))?.Text;

        // Assert
        Assert.AreEqual("O campo \"Título\" é obrigatório.", erroTitulo);
        Assert.AreEqual("O campo \"Duração\" precisa conter um valor acima de 0.", erroDuracao);
        Assert.AreEqual("O campo \"Gênero\" é obrigatório.", erroGenero);
    }

    [TestMethod]
    public void Deve_Validar_Duracao_Positiva_Ao_Cadastrar_Filme()
    {
        // Arrange
        var GeneroIndexPage = new GeneroIndexPageObject(driver!)
         .IrPara(enderecoBase!);

        GeneroIndexPage
            .ClickCadastrar()
            .PreencherDescricao("Animação")
            .Confirmar();

        var IndexPage = new FilmeIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        // Act
        IndexPage
            .ClickCadastrar()
            .PreencherTitulo("Toy Story")
            .PreencherDuracao(-90)
            .MarcarLancamento(false)
            .SelecionarGenero("Animação")
            .ClicarEmConfirmar();
        var erroDuracao = driver?.FindElement(By.CssSelector("span[data-se='Duracao-erro']"))?.Text;

        // Assert
        Assert.AreEqual("O campo \"Duração\" precisa conter um valor acima de 0.", erroDuracao);
    }

    [TestMethod]
    public void Deve_Validar_Duracao_Positiva_Ao_Editar_Filme()
    {
        // Arrange
        var GeneroIndexPage = new GeneroIndexPageObject(driver!)
         .IrPara(enderecoBase!);

        GeneroIndexPage
            .ClickCadastrar()
            .PreencherDescricao("Aventura")
            .Confirmar();

        var IndexPage = new FilmeIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        IndexPage
          .ClickCadastrar()
          .PreencherTitulo("Jumanji")
          .PreencherDuracao(104)
          .MarcarLancamento(false)
          .SelecionarGenero("Aventura")
          .Confirmar();

        // Act
        IndexPage
            .ClickEditar()
            .PreencherDuracao(-150)
            .ClicarEmConfirmar();

        var erroDuracao = driver?.FindElement(By.CssSelector("span[data-se='Duracao-erro']"))?.Text;
        // Assert
        Assert.AreEqual("O campo \"Duração\" precisa conter um valor acima de 0.", erroDuracao);
    }

    [TestMethod]
    public void Deve_Validar_Titulo_Duplicado_Ao_Cadastrar_Filme()
    {
        // Arrange
        var GeneroIndexPage = new GeneroIndexPageObject(driver!)
         .IrPara(enderecoBase!);

        GeneroIndexPage
            .ClickCadastrar()
            .PreencherDescricao("Suspense")
            .Confirmar();

        var IndexPage = new FilmeIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        IndexPage
          .ClickCadastrar()
          .PreencherTitulo("Se7en")
          .PreencherDuracao(127)
          .MarcarLancamento(false)
          .SelecionarGenero("Suspense")
          .Confirmar();

        // Act
        IndexPage
            .ClickCadastrar()
            .PreencherTitulo("Se7en")
            .PreencherDuracao(130)
            .MarcarLancamento(false)
            .SelecionarGenero("Suspense")
            .ClicarEmConfirmar();

        var mensagemDeErro = driver?.FindElement(By.CssSelector("div[role='alert']"))?.Text;

        // Assert
        Assert.AreEqual("Já existe um filme registrado com este título.", mensagemDeErro);
    }

    [TestMethod]
    public void Deve_Validar_Titulo_Duplicado_Ao_Editar_Filme()
    {
        // Arrange
        var GeneroIndexPage = new GeneroIndexPageObject(driver!)
            .IrPara(enderecoBase!);

        GeneroIndexPage
            .ClickCadastrar()
            .PreencherDescricao("Suspense")
            .Confirmar();

        var IndexPage = new FilmeIndexPageObject(driver!)
       .IrPara(enderecoBase!);

        IndexPage
          .ClickCadastrar()
          .PreencherTitulo("Se7en")
          .PreencherDuracao(127)
          .MarcarLancamento(false)
          .SelecionarGenero("Suspense")
          .Confirmar();

        IndexPage
         .ClickCadastrar()
         .PreencherTitulo("Sonhos")
         .PreencherDuracao(230)
         .MarcarLancamento(true)
         .SelecionarGenero("Suspense")
         .Confirmar();

        // Act
        IndexPage
          .ClickEditar()
          .PreencherTitulo("Sonhos")
          .PreencherDuracao(100)
          .MarcarLancamento(true)
          .SelecionarGenero("Suspense")
          .ClicarEmConfirmar();

        var mensagemDeErro = driver?.FindElement(By.CssSelector("div[role='alert']"))?.Text;

        // Assert
        Assert.AreEqual("Já existe um filme registrado com este título.", mensagemDeErro);
    }
}