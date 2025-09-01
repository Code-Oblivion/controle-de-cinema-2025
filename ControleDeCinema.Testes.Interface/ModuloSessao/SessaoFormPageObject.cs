using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloSessao;
public class SessaoFormPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public SessaoFormPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        wait.Until(d => d.FindElement(By.CssSelector("form[data-se='Formulario']")).Displayed);
    }

    public SessaoFormPageObject PreencherÍnicio(string nome)
    {
        var inputDescricao = driver?.FindElement(By.CssSelector("input[data-se='Inicio']"));
        inputDescricao?.Clear();
        inputDescricao?.SendKeys(nome);

        return this;
    }

    public SessaoFormPageObject PreencherNumeroIngressos(string numero)
    {
        var inputDescricao = driver?.FindElement(By.CssSelector("input[data-se='NumeroMaximo']"));
        inputDescricao?.Clear();
        inputDescricao?.SendKeys(numero);

        return this;
    }

    public SessaoFormPageObject PreencherFilme(string nome)
    {
        var inputDescricao = driver?.FindElement(By.CssSelector("input[data-se='Filme']"));
        inputDescricao?.Clear();
        inputDescricao?.SendKeys(nome);

        return this;
    }

    public SessaoFormPageObject PreencherSala(string nome)
    {
        var inputDescricao = driver?.FindElement(By.CssSelector("input[data-se='Sala']"));
        inputDescricao?.Clear();
        inputDescricao?.SendKeys(nome);

        return this;
    }

    public SessaoIndexPageObject Confirmar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();

        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return new SessaoIndexPageObject(driver!);
    }

    public SessaoIndexPageObject ClicarEmConfirmar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();

        return new SessaoIndexPageObject(driver!);
    }
}