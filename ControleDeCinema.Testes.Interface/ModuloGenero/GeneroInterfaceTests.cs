using ControleDeCinema.Testes.Interface.Compartilhado;
using OpenQA.Selenium;

namespace ControleDeCinema.Testes.Interface.ModuloGenero;

[TestClass]
[TestCategory("Teste de Interface de Genero")]
public sealed class GeneroInterfaceTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Genero_Corretamente()
    {
        // Arrange
        var IndexPage = new GeneroIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        // Act
        IndexPage
            .ClickCadastrar()
            .PreencherDescricao("Ação")
            .Confirmar();
        // Assert
        Assert.IsTrue(IndexPage.ContemGenero("Ação"));
    }

    [TestMethod]
    public void Deve_Editar_Genero_Corretamente()
    {
        // Arrange
        var IndexPage = new GeneroIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        IndexPage
          .ClickCadastrar()
          .PreencherDescricao("Comédia")
          .Confirmar();

        // Act
        IndexPage
            .ClickEditar()
            .PreencherDescricao("Comédia Editada")
            .Confirmar();

        // Assert
        Assert.IsTrue(IndexPage.ContemGenero("Comédia Editada"));
    }

    [TestMethod]
    public void Deve_Excluir_Genero_Corretamente()
    {
        // Arrange
        var IndexPage = new GeneroIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        IndexPage
          .ClickCadastrar()
          .PreencherDescricao("Terror")
          .Confirmar();

        // Act
        IndexPage
            .ClickExcluir()
            .Confirmar();

        // Assert
        Assert.IsFalse(IndexPage.ContemGenero("Terror"));
    }

    [TestMethod]
    public void Deve_Visualizar_Genero_Corretamente()
    {
        // Arrange
        var IndexPage = new GeneroIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        IndexPage
          .ClickCadastrar()
          .PreencherDescricao("Drama")
          .Confirmar();

        // Act
        var descricao = driver?.FindElement(By.CssSelector("h5[data-se='Descricao']"))?.Text;

        // Assert
        Assert.AreEqual("Drama", descricao);
    }

    [TestMethod]
    public void Deve_Validar_Campos_Obrigatorios_ao_Cadastrar_Genero()
    {
        // Arrange
        var IndexPage = new GeneroIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        // Act
        IndexPage
            .ClickCadastrar()
            .PreencherDescricao(string.Empty)
            .ClicarEmConfirmar();
        var mensagemDeErro = driver?.FindElement(By.CssSelector("span[data-se='Descricao-error']"))?.Text;

        // Assert
        Assert.AreEqual("O campo \"Descrição\" é obrigatório.", mensagemDeErro);
    }

    [TestMethod]
    public void Deve_Validar_Campos_Obrigatorios_ao_Editar_Genero()
    {
        // Arrange
        var IndexPage = new GeneroIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        IndexPage
          .ClickCadastrar()
          .PreencherDescricao("Ficção")
          .Confirmar();

        // Act
        IndexPage
            .ClickEditar()
            .PreencherDescricao(string.Empty)
            .ClicarEmConfirmar();

        var mensagemDeErro = driver?.FindElement(By.CssSelector("span[data-se='Descricao-error']"))?.Text;
        // Assert
        Assert.AreEqual("O campo \"Descrição\" é obrigatório.", mensagemDeErro);
    }

    //deve validar Gênero Duplicado:

    [TestMethod]
    public void Deve_Validar_Genero_Duplicado_Ao_Cadastrar()
    {
        // Arrange
        var IndexPage = new GeneroIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        IndexPage
          .ClickCadastrar()
          .PreencherDescricao("Aventura")
          .Confirmar();

        // Act
        IndexPage
            .ClickCadastrar()
            .PreencherDescricao("Aventura")
            .ClicarEmConfirmar();
        var mensagemDeErro = driver?.FindElement(By.CssSelector("div[role='alert']"))?.Text;

        // Assert
        Assert.AreEqual("Já existe um gênero de filme registrado com esta descrição.", mensagemDeErro);
    }

    [TestMethod]
    public void Deve_Validar_Genero_Duplicado_Ao_Editar()
    {
        // Arrange
        var IndexPage = new GeneroIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        IndexPage
          .ClickCadastrar()
          .PreencherDescricao("Musical")
          .Confirmar();

        IndexPage
          .ClickCadastrar()
          .PreencherDescricao("Documentário")
          .Confirmar();

        // Act
        IndexPage
            .ClickEditar()
            .PreencherDescricao("Musical")
            .ClicarEmConfirmar();

        var mensagemDeErro = driver?.FindElement(By.CssSelector("div[role='alert']"))?.Text;
        // Assert
        Assert.AreEqual("Já existe um gênero de filme registrado com esta descrição.", mensagemDeErro);
    }
}