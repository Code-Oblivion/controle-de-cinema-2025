using ControleDeCinema.Dominio.ModuloSala;
using ControleDeCinema.Testes.Interface.Compartilhado;
using OpenQA.Selenium;

namespace ControleDeCinema.Testes.Interface.ModuloSala;

[TestClass]
[TestCategory("Teste de Interface de Sala")]
public sealed class SalaInterfaceTests : TestFixture
{
    [TestMethod]
    public void Deve_Cadastrar_Sala_Corretamente()
    {
        // Arrange
        var IndexPage = new SalaIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        // Act
        IndexPage
            .ClickCadastrar()
            .PreencherNumero(1)
            .PreencherCapacidade(100)
            .Confirmar();

        // Assert
        Assert.IsTrue(IndexPage.ContemSala("1"));
    }

    [TestMethod]
    public void Deve_Editar_Sala_Corretamente()
    {
        // Arrange
        var IndexPage = new SalaIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        IndexPage
          .ClickCadastrar()
          .PreencherNumero(2)
          .PreencherCapacidade(150)
          .Confirmar();

        // Act
        IndexPage
            .ClickEditar()
            .PreencherNumero(3)
            .PreencherCapacidade(200)
            .Confirmar();

        // Assert
        Assert.IsTrue(IndexPage.ContemSala("3"));
    }

    [TestMethod]
    public void Deve_Excluir_Sala_Corretamente()
    {
        // Arrange
        var IndexPage = new SalaIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        IndexPage
          .ClickCadastrar()
          .PreencherNumero(4)
          .PreencherCapacidade(120)
          .Confirmar();

        // Act
        IndexPage
            .ClickExcluir()
            .Confirmar();

        // Assert
        Assert.IsFalse(IndexPage.ContemSala("4"));
    }

    [TestMethod]
    public void Deve_Visualizar_Sala_Corretamente()
    {
        // Arrange
        var IndexPage = new SalaIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        IndexPage
          .ClickCadastrar()
          .PreencherNumero(5)
          .PreencherCapacidade(130)
          .Confirmar();

        // Act  // Assert
        Assert.IsTrue(IndexPage.ContemSala("5"));
    }

    [TestMethod]
    public void Deve_Validar_Campos_Obrigatorios_ao_Cadastrar_Sala()
    {
        // Arrange
        var IndexPage = new SalaIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        // Act
        var FormPage = IndexPage.ClickCadastrar();
        FormPage.ClicarEmConfirmar();

        var mensagemErroNumero = driver!.FindElement(By.CssSelector("span[data-se='Numero-error']")).Text;
        var mensagemErroCapacidade = driver.FindElement(By.CssSelector("span[data-se='Capacidade-error']")).Text;

        // Assert
        Assert.AreEqual("O campo \"Número\" precisa conter um valor acima de 0.", mensagemErroNumero);
        Assert.AreEqual("O campo \"Capacidade\" precisa conter um valor acima de 0.", mensagemErroCapacidade);
    }

    [TestMethod]
    public void Deve_Validar_Campos_Obligatorios_ao_Editar_Sala()
    {
        // Arrange
        var IndexPage = new SalaIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        IndexPage
          .ClickCadastrar()
          .PreencherNumero(6)
          .PreencherCapacidade(140)
          .Confirmar();

        // Act
        var FormPage = IndexPage.ClickEditar();
        FormPage.PreencherNumero(0)
                .PreencherCapacidade(0)
                .ClicarEmConfirmar();

        var mensagemErroNumero = driver!.FindElement(By.CssSelector("span[data-se='Numero-error']")).Text;
        var mensagemErroCapacidade = driver.FindElement(By.CssSelector("span[data-se='Capacidade-error']")).Text;
        // Assert
        Assert.AreEqual("O campo \"Número\" precisa conter um valor acima de 0.", mensagemErroNumero);
        Assert.AreEqual("O campo \"Capacidade\" precisa conter um valor acima de 0.", mensagemErroCapacidade);
    }

    [TestMethod]
    public void Deve_Validar_Capacidade_Positiva_ao_Cadastrar_Sala()
    {
        // Arrange
        var IndexPage = new SalaIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        // Act
        var FormPage = IndexPage.ClickCadastrar();
        FormPage.PreencherNumero(7)
                .PreencherCapacidade(-10)
                .ClicarEmConfirmar();
        var mensagemErroCapacidade = driver!.FindElement(By.CssSelector("span[data-se='Capacidade-error']")).Text;
        
        // Assert
        Assert.AreEqual("O campo \"Capacidade\" precisa conter um valor acima de 0.", mensagemErroCapacidade);
    }

    [TestMethod]
    public void Deve_Validar_Capacidade_Positiva_ao_Editar_Sala()
    {
        // Arrange
        var IndexPage = new SalaIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        IndexPage
          .ClickCadastrar()
          .PreencherNumero(8)
          .PreencherCapacidade(160)
          .Confirmar();

        // Act
        var FormPage = IndexPage.ClickEditar();
        FormPage.PreencherNumero(8)
                .PreencherCapacidade(-20)
                .ClicarEmConfirmar();
        var mensagemErroCapacidade = driver!.FindElement(By.CssSelector("span[data-se='Capacidade-error']")).Text;
        
        // Assert
        Assert.AreEqual("O campo \"Capacidade\" precisa conter um valor acima de 0.", mensagemErroCapacidade);
    }

    [TestMethod]
    public void Deve_Validar_Numero_Sala_Duplicado_ao_Cadastrar_Sala()
    {
        // Arrange
        var IndexPage = new SalaIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        IndexPage
          .ClickCadastrar()
          .PreencherNumero(9)
          .PreencherCapacidade(170)
          .Confirmar();

        // Act
        var FormPage = IndexPage.ClickCadastrar();
        FormPage.PreencherNumero(9)
                .PreencherCapacidade(180)
                .ClicarEmConfirmar();
        var mensagemErroNumero = driver!.FindElement(By.CssSelector("div[role='alert']"))?.Text;
        
        // Assert
        Assert.AreEqual("Já existe uma sala registrada com este número.", mensagemErroNumero);
    }

    [TestMethod]
    public void Deve_Validar_Numero_Sala_Duplicado_ao_Editar_Sala()
    {
        // Arrange
        var IndexPage = new SalaIndexPageObject(driver!)
        .IrPara(enderecoBase!);

        IndexPage
          .ClickCadastrar()
          .PreencherNumero(10)
          .PreencherCapacidade(190)
          .Confirmar();

        IndexPage
          .ClickCadastrar()
          .PreencherNumero(20)
          .PreencherCapacidade(200)
          .Confirmar();

        // Act
        var FormPage = IndexPage.ClickEditar();
        FormPage.PreencherNumero(20)
                .PreencherCapacidade(200)
                .ClicarEmConfirmar();

        var mensagemErroNumero = driver!.FindElement(By.CssSelector("div[role='alert']"))?.Text;
        
        // Assert
        Assert.AreEqual("Já existe uma sala registrada com este número.", mensagemErroNumero);
    }
}