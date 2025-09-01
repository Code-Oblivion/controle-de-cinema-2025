using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloGenero;

public class GeneroIndexPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public GeneroIndexPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    public GeneroIndexPageObject IrPara(string enderecoBase)
    {
        driver.Navigate().GoToUrl(Path.Combine(enderecoBase, "generos"));

        return this;
    }
    public GeneroFormPageObject ClickCadastrar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']"))).Click();

        return new GeneroFormPageObject(driver!);
    }

    public GeneroFormPageObject ClickEditar()
    {
        wait.Until(d => d.FindElement(By.CssSelector(".card a[data-se='btnEdição']"))).Click();

        return new GeneroFormPageObject(driver!);
    }

    public GeneroFormPageObject ClickExcluir()
    {
        wait.Until(d => d.FindElement(By.CssSelector(".card a[data-se='btnExclusão']"))).Click();

        return new GeneroFormPageObject(driver!);
    }

    public bool ContemGenero(string nome)
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return driver.PageSource.Contains(nome);
    }
}
