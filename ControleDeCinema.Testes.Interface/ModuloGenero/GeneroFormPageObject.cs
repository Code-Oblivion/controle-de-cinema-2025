using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloGenero;
public class GeneroFormPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public GeneroFormPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        wait.Until(d => d.FindElement(By.CssSelector("form[data-se='Formulario']")).Displayed);
    }

    public GeneroFormPageObject PreencherDescricao(string nome)
    {
        var inputDescricao = driver?.FindElement(By.CssSelector("input[data-se='Descricao']"));
        inputDescricao?.Clear();
        inputDescricao?.SendKeys(nome);

        return this;
    }

    public GeneroIndexPageObject Confirmar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();

        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return new GeneroIndexPageObject(driver!);
    }

    public GeneroIndexPageObject ClicarEmConfirmar() 
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();

        return new GeneroIndexPageObject(driver!);
    }
}