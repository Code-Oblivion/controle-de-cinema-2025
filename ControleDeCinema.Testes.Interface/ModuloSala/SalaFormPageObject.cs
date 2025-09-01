using ControleDeCinema.Testes.Interface.ModuloGenero;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloSala;

public class SalaFormPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public SalaFormPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        wait.Until(d => d.FindElement(By.CssSelector("form[data-se='Formulario']")).Displayed);
    }

    public SalaFormPageObject PreencherNumero(int numero)
    {
        var inputNumero = driver?.FindElement(By.CssSelector("input[data-se='Numero']"));
        inputNumero?.Clear();
        inputNumero?.SendKeys(numero.ToString());
        return this;
    }

    public SalaFormPageObject PreencherCapacidade(int capacidade)
    {
        var inputCapacidade = driver?.FindElement(By.CssSelector("input[data-se='Capacidade']"));
        inputCapacidade?.Clear();
        inputCapacidade?.SendKeys(capacidade.ToString());
        return this;
    }

    public SalaIndexPageObject Confirmar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();

        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return new SalaIndexPageObject(driver!);
    }

    public SalaIndexPageObject ClicarEmConfirmar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();

        return new SalaIndexPageObject(driver!);
    }
}