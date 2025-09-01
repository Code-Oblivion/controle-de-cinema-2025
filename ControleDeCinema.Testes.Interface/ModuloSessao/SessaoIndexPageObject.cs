using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloSessao;
public class SessaoIndexPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public SessaoIndexPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    public SessaoIndexPageObject IrPara(string enderecoBase)
    {
        driver.Navigate().GoToUrl(Path.Combine(enderecoBase, "sessoes"));

        return this;
    }
    public SessaoFormPageObject ClickCadastrar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']"))).Click();

        return new SessaoFormPageObject(driver!);
    }

    public SessaoFormPageObject ClickEditar()
    {
        wait.Until(d => d.FindElement(By.CssSelector(".card a[data-se='btnEdição-Sessao']"))).Click();

        return new SessaoFormPageObject(driver!);
    }

    public SessaoFormPageObject ClickExcluir()
    {
        wait.Until(d => d.FindElement(By.CssSelector(".card a[data-se='btnExclusão-Sessao']"))).Click();

        return new SessaoFormPageObject(driver!);
    }

    public SessaoFormPageObject ClickEncerrar()
    {
        wait.Until(d => d.FindElement(By.CssSelector(".card a[data-se='btnEncerrar-Sessao']"))).Click();
        return new SessaoFormPageObject(driver!);
    }

    public bool ContemSala(string nome)
    {
        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return driver.PageSource.Contains(nome);
    }
}