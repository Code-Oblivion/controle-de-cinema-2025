
using ControleDeCinema.Testes.Interface.ModuloGenero;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloSala;
public class SalaIndexPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public SalaIndexPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }
    public SalaIndexPageObject IrPara(string enderecoBase)
    {
        driver.Navigate().GoToUrl(Path.Combine(enderecoBase, "salas"));

        return this;
    }
    public SalaFormPageObject ClickCadastrar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']"))).Click();

        return new SalaFormPageObject(driver!);
    }

    public SalaFormPageObject ClickEditar()
    {
        wait.Until(d => d.FindElement(By.CssSelector(".card a[data-se='btnEdição']"))).Click();

        return new SalaFormPageObject(driver!);
    }

    public SalaFormPageObject ClickExcluir()
    {
        wait.Until(d => d.FindElement(By.CssSelector(".card a[data-se='btnExclusão']"))).Click();

        return new SalaFormPageObject(driver!);
    }

    public bool ContemSala(string numero)
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return driver.PageSource.Contains($"# {numero}");
    }
}