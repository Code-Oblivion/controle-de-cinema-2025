using ControleDeCinema.Testes.Interface.ModuloGenero;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ControleDeCinema.Testes.Interface.ModuloFilme;

public class FilmeFormPageObject
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    public FilmeFormPageObject(IWebDriver driver)
    {
        this.driver = driver;

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        wait.Until(d => d.FindElement(By.CssSelector("form[data-se='Formulario']")).Displayed);
    }

    public FilmeFormPageObject PreencherTitulo(string nome)
    {
        var inputDescricao = driver?.FindElement(By.CssSelector("input[data-se='Titulo']"));
        inputDescricao?.Clear();
        inputDescricao?.SendKeys(nome);

        return this;
    }

    public FilmeFormPageObject PreencherDuracao(int duracao)
    {
        var inputDescricao = driver?.FindElement(By.CssSelector("input[data-se='Duracao']"));
        inputDescricao?.Clear();
        inputDescricao?.SendKeys(duracao.ToString());
        return this;
    }

    public FilmeFormPageObject MarcarLancamento(bool lancamento)
    {
        var checkboxLancamento = driver?.FindElement(By.CssSelector("input[type='checkbox'][data-se='checkbox-lancamento']"));
        if (checkboxLancamento != null && checkboxLancamento.Selected != lancamento)
            checkboxLancamento.Click();
        return this;
    }

    public FilmeFormPageObject SelecionarGenero(string? genero)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        var byGenero = By.CssSelector("select[data-se='Genero']");

        var selectEl = wait.Until(d =>
        {
            var el = d.FindElement(byGenero);
            return (el.Displayed && el.Enabled) ? el : null;
        });

        var select = new SelectElement(selectEl);
        wait.Until(_ => select.Options.Count > 0);

        if (string.IsNullOrWhiteSpace(genero))
        {
            try { select.SelectByValue(""); }
            catch { select.SelectByIndex(0); }
        }
        else
        {
            select.SelectByText(genero, true);
        }

        return this;
    }

    public FilmeIndexPageObject Confirmar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();

        wait.Until(d => d.FindElement(By.CssSelector("a[data-se='btnCadastrar']")).Displayed);

        return new FilmeIndexPageObject(driver!);
    }

    public FilmeIndexPageObject ClicarEmConfirmar()
    {
        wait.Until(d => d.FindElement(By.CssSelector("button[data-se='btnConfirmar']"))).Click();

        return new FilmeIndexPageObject(driver!);
    }
}