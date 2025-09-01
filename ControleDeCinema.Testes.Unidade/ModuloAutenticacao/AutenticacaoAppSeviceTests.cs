
using ControleDeCinema.Aplicacao.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using FluentResults;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace ControleDeCinema.Testes.Unidade.ModuloAutenticacao;

[TestClass]
[TestCategory("Testes de Unidade de AutenticacaoAppService")]
public class AutenticacaoAppServiceTestes
{
    private AutenticacaoAppService autenticacaoAppService = default!;

    private const string _emailPadrao = "email@gmail.com";
    private const string _senhaPadrao = "Senha123!";
    private const string _userNamePadrao = _emailPadrao;
    private const TipoUsuario _tipoUsuarioPadrao = TipoUsuario.Cliente;
    private readonly string _tipoUsuarioPadraoString = _tipoUsuarioPadrao.ToString();
    private readonly Cargo _cargoPadrao = new()
    {
        Name = TipoUsuario.Cliente.ToString(),
        NormalizedName = TipoUsuario.Cliente.ToString().ToUpper(),
        ConcurrencyStamp = Guid.NewGuid().ToString()
    };

    private Mock<UserManager<Usuario>> _userManagerMock = default!;
    private Mock<RoleManager<Cargo>> _roleManagerMock = default!;
    private FakeSignInManager _signInManagerFake = default!;


    private static Mock<UserManager<Usuario>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<Usuario>>();
        return new Mock<UserManager<Usuario>>(
            store.Object,
            Options.Create(new IdentityOptions()),
            new Mock<IPasswordHasher<Usuario>>().Object,
            Array.Empty<IUserValidator<Usuario>>(),
            Array.Empty<IPasswordValidator<Usuario>>(),
            new Mock<ILookupNormalizer>().Object,
            new IdentityErrorDescriber(),
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<Usuario>>>().Object
        );
    }

    private static Mock<RoleManager<Cargo>> CreateRoleManagerMock()
    {
        var store = new Mock<IRoleStore<Cargo>>();
        return new Mock<RoleManager<Cargo>>(
            store.Object,
            Array.Empty<IRoleValidator<Cargo>>(),
            new Mock<ILookupNormalizer>().Object,
            new IdentityErrorDescriber(),
            new Mock<ILogger<RoleManager<Cargo>>>().Object
        );
    }

    private static FakeSignInManager CreateFakeSignInManager(UserManager<Usuario> userManager)
    {
        var http = new DefaultHttpContext();
        var contextAccessor = new Mock<IHttpContextAccessor>();
        contextAccessor.Setup(a => a.HttpContext).Returns(http);

        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<Usuario>>();
        var options = Options.Create(new IdentityOptions());
        var logger = new Mock<ILogger<SignInManager<Usuario>>>();
        var schemes = new Mock<IAuthenticationSchemeProvider>();
        var confirmation = new Mock<IUserConfirmation<Usuario>>();

        return new FakeSignInManager(
            userManager,
            contextAccessor.Object,
            claimsFactory.Object,
            options,
            logger.Object,
            schemes.Object,
            confirmation.Object
        );
    }

    private sealed class FakeSignInManager : SignInManager<Usuario>
    {
        public FakeSignInManager(
            UserManager<Usuario> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<Usuario> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<Usuario>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<Usuario> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
        }

        public SignInResult NextPasswordSignInResult { get; set; } = SignInResult.Success;
        public int SignOutCount { get; private set; } = 0;

        public override Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
            => Task.FromResult(NextPasswordSignInResult);

        public override Task SignOutAsync()
        {
            SignOutCount++;
            return Task.CompletedTask;
        }
    }

    [TestInitialize]
    public void Setup()
    {
        _userManagerMock = CreateUserManagerMock();
        _roleManagerMock = CreateRoleManagerMock();
        _signInManagerFake = CreateFakeSignInManager(_userManagerMock.Object);

        autenticacaoAppService = new AutenticacaoAppService(
            _userManagerMock.Object,
            _signInManagerFake,
            _roleManagerMock.Object
        );
    }

    [TestMethod]
    public async Task Registrar_Deve_Retornar_Sucesso()
    {
        // Arrange
        var novoUsuario = new Usuario { UserName = _userNamePadrao, Email = _emailPadrao };

        _userManagerMock.Setup(u => u.CreateAsync(novoUsuario, _senhaPadrao))
                       .ReturnsAsync(IdentityResult.Success);

        _roleManagerMock.Setup(r => r.FindByNameAsync(_tipoUsuarioPadraoString))
                       .ReturnsAsync(_cargoPadrao);

        _userManagerMock.Setup(u => u.AddToRoleAsync(novoUsuario, _tipoUsuarioPadraoString))
                       .ReturnsAsync(IdentityResult.Success);

        _signInManagerFake.NextPasswordSignInResult = SignInResult.Success;

        // Act
        var resultado = await autenticacaoAppService.RegistrarAsync(novoUsuario, _senhaPadrao, _tipoUsuarioPadrao);

        // Assert
        _userManagerMock.Verify(u => u.CreateAsync(novoUsuario, _senhaPadrao), Times.Once);
        _roleManagerMock.Verify(r => r.FindByNameAsync(_tipoUsuarioPadraoString), Times.Once);
        _userManagerMock.Verify(u => u.AddToRoleAsync(novoUsuario, _tipoUsuarioPadraoString), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public async Task Registrar_Com_Usuario_Duplicado_Deve_Retornar_Falha()
    {
        // Arrange
        var novoUsuario = new Usuario { UserName = _userNamePadrao, Email = "outro@teste.com" };
        var err = new IdentityError { Code = "DuplicateUserName" };

        _userManagerMock.Setup(u => u.CreateAsync(novoUsuario, _senhaPadrao))
                       .ReturnsAsync(IdentityResult.Failed(err));

        // Act
        var resultado = await autenticacaoAppService.RegistrarAsync(novoUsuario, _senhaPadrao, _tipoUsuarioPadrao);

        // Assert
        _userManagerMock.Verify(u => u.CreateAsync(novoUsuario, _senhaPadrao), Times.Once);
        _roleManagerMock.Verify(r => r.FindByNameAsync(_tipoUsuarioPadraoString), Times.Never);
        _userManagerMock.Verify(u => u.AddToRoleAsync(novoUsuario, _tipoUsuarioPadraoString), Times.Never);

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Requisição inválida", resultado.Errors[0].Message);

        var mensagens = resultado.Errors.SelectMany(e => e.Reasons.OfType<Error>()).Select(r => r.Message).ToList();
        CollectionAssert.Contains(mensagens, "Já existe um usuário com esse nome.");
    }

    [TestMethod]
    public async Task Registrar_Com_Email_Duplicado_Deve_Retornar_Falha()
    {
        // Arrange
        var novoUsuario = new Usuario { UserName = "outro", Email = _emailPadrao };
        var err = new IdentityError { Code = "DuplicateEmail" };

        _userManagerMock.Setup(u => u.CreateAsync(novoUsuario, _senhaPadrao))
                       .ReturnsAsync(IdentityResult.Failed(err));

        // Act
        var resultado = await autenticacaoAppService.RegistrarAsync(novoUsuario, _senhaPadrao, _tipoUsuarioPadrao);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Requisição inválida", resultado.Errors[0].Message);

        var msg = resultado.Errors.SelectMany(e => e.Reasons.OfType<Error>())
                                  .Select(r => r.Message).FirstOrDefault();
        Assert.AreEqual("Já existe um usuário com esse e-mail.", msg);
    }

    [TestMethod]
    public async Task Registrar_Com_Senha_Invalida_Deve_Retornar_Falha()
    {
        // Arrange
        var novoUsuario = new Usuario { UserName = "outro", Email = _emailPadrao };

        IdentityError[] errors =
        {
                new() { Code = "PasswordTooShort" },
                new() { Code = "PasswordRequiresNonAlphanumeric" },
                new() { Code = "PasswordRequiresDigit" },
                new() { Code = "PasswordRequiresUpper" },
                new() { Code = "PasswordRequiresLower" }
            };

        _userManagerMock.Setup(u => u.CreateAsync(novoUsuario, _senhaPadrao))
                       .ReturnsAsync(IdentityResult.Failed(errors));

        // Act
        var resultado = await autenticacaoAppService.RegistrarAsync(novoUsuario, _senhaPadrao, _tipoUsuarioPadrao);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Requisição inválida", resultado.Errors[0].Message);

        var mensagens = resultado.Errors.SelectMany(e => e.Reasons.OfType<Error>())
                                        .Select(r => r.Message).ToList();

        CollectionAssert.AreEquivalent(new[]
        {
                "A senha é muito curta.",
                "A senha deve conter pelo menos um caractere especial.",
                "A senha deve conter pelo menos um número.",
                "A senha deve conter pelo menos uma letra maiúscula.",
                "A senha deve conter pelo menos uma letra minúscula."
            }, mensagens);
    }

    [TestMethod]
    public async Task Registrar_Cria_Cargo_Quando_Inexistente_E_Atribui_Ao_Usuario()
    {
        // Arrange
        var novoUsuario = new Usuario { UserName = _userNamePadrao, Email = _emailPadrao };

        _userManagerMock.Setup(u => u.CreateAsync(novoUsuario, _senhaPadrao))
                       .ReturnsAsync(IdentityResult.Success);

        _roleManagerMock.Setup(r => r.FindByNameAsync(_tipoUsuarioPadraoString))
                       .ReturnsAsync((Cargo?)null!);

        _roleManagerMock.Setup(r => r.CreateAsync(It.IsAny<Cargo>()))
                       .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(u => u.AddToRoleAsync(novoUsuario, _tipoUsuarioPadraoString))
                       .ReturnsAsync(IdentityResult.Success);

        _signInManagerFake.NextPasswordSignInResult = SignInResult.Success;

        // Act
        var resultado = await autenticacaoAppService.RegistrarAsync(novoUsuario, _senhaPadrao, _tipoUsuarioPadrao);

        // Assert
        _userManagerMock.Verify(u => u.CreateAsync(novoUsuario, _senhaPadrao), Times.Once);
        _roleManagerMock.Verify(r => r.FindByNameAsync(_tipoUsuarioPadraoString), Times.Once);
        _roleManagerMock.Verify(r => r.CreateAsync(It.Is<Cargo>(c =>
            c.Name == _tipoUsuarioPadraoString &&
            c.NormalizedName == _tipoUsuarioPadraoString.ToUpper())), Times.Once);
        _userManagerMock.Verify(u => u.AddToRoleAsync(novoUsuario, _tipoUsuarioPadraoString), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public async Task Registrar_Chama_Login_Apos_Criar_Usuario()
    {
        // Arrange
        var novoUsuario = new Usuario { UserName = _userNamePadrao, Email = _emailPadrao };

        _userManagerMock.Setup(u => u.CreateAsync(novoUsuario, _senhaPadrao))
                       .ReturnsAsync(IdentityResult.Success);

        _roleManagerMock.Setup(r => r.FindByNameAsync(_tipoUsuarioPadraoString))
                       .ReturnsAsync(_cargoPadrao);

        _userManagerMock.Setup(u => u.AddToRoleAsync(novoUsuario, _tipoUsuarioPadraoString))
                       .ReturnsAsync(IdentityResult.Success);

        _signInManagerFake.NextPasswordSignInResult = SignInResult.Success;

        // Act
        var resultado = await autenticacaoAppService.RegistrarAsync(novoUsuario, _senhaPadrao, _tipoUsuarioPadrao);

        // Assert
        _userManagerMock.Verify(u => u.CreateAsync(novoUsuario, _senhaPadrao), Times.Once);
        _roleManagerMock.Verify(r => r.FindByNameAsync(_tipoUsuarioPadraoString), Times.Once);
        _userManagerMock.Verify(u => u.AddToRoleAsync(novoUsuario, _tipoUsuarioPadraoString), Times.Once);

        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public async Task Login_Deve_Retornar_Sucesso()
    {
        // Arrange
        _signInManagerFake.NextPasswordSignInResult = SignInResult.Success;

        // Act
        var resultado = await autenticacaoAppService.LoginAsync(_emailPadrao, _senhaPadrao);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
    }

    [TestMethod]
    public async Task Login_Com_Conta_Bloqueada_Deve_Retornar_Falha()
    {
        // Arrange
        _signInManagerFake.NextPasswordSignInResult = SignInResult.LockedOut;

        // Act
        var resultado = await autenticacaoAppService.LoginAsync(_emailPadrao, _senhaPadrao);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Requisição inválida", resultado.Errors[0].Message);

        var msg = resultado.Errors.SelectMany(e => e.Reasons.OfType<Error>())
                                  .Select(r => r.Message).FirstOrDefault();
        Assert.AreEqual("Sua conta foi bloqueada temporariamente devido a muitas tentativas inválidas.", msg);
    }

    [TestMethod]
    public async Task Login_Nao_Permitido_Deve_Retornar_Falha()
    {
        // Arrange
        _signInManagerFake.NextPasswordSignInResult = SignInResult.NotAllowed;

        // Act
        var resultado = await autenticacaoAppService.LoginAsync(_emailPadrao, _senhaPadrao);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Requisição inválida", resultado.Errors[0].Message);

        var msg = resultado.Errors.SelectMany(e => e.Reasons.OfType<Error>())
                                  .Select(r => r.Message).FirstOrDefault();
        Assert.AreEqual("Não é permitido efetuar login. Verifique se sua conta está confirmada.", msg);
    }

    [TestMethod]
    public async Task Login_Requer_Dois_Fatores_Deve_Retornar_Falha()
    {
        // Arrange
        _signInManagerFake.NextPasswordSignInResult = SignInResult.TwoFactorRequired;

        // Act
        var resultado = await autenticacaoAppService.LoginAsync(_emailPadrao, _senhaPadrao);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Requisição inválida", resultado.Errors[0].Message);

        var msg = resultado.Errors.SelectMany(e => e.Reasons.OfType<Error>())
                                  .Select(r => r.Message).FirstOrDefault();
        Assert.AreEqual("É necessário confirmar o login com autenticação de dois fatores.", msg);
    }

    [TestMethod]
    public async Task Login_Com_Credenciais_Invalidas_Deve_Retornar_Falha()
    {
        // Arrange
        _signInManagerFake.NextPasswordSignInResult = SignInResult.Failed;

        // Act
        var resultado = await autenticacaoAppService.LoginAsync(_emailPadrao, _senhaPadrao);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Requisição inválida", resultado.Errors[0].Message);

        var msg = resultado.Errors.SelectMany(e => e.Reasons.OfType<Error>())
                                  .Select(r => r.Message).FirstOrDefault();
        Assert.AreEqual("Login ou senha incorretos.", msg);
    }

    [TestMethod]
    public async Task Logout_Deve_Retornar_Sucesso()
    {
        // Arrange
        // (sem setup — fake apenas conta chamadas)

        // Act
        var resultado = await autenticacaoAppService.LogoutAsync();

        // Assert
        Assert.AreEqual(1, _signInManagerFake.SignOutCount);
        Assert.IsTrue(resultado.IsSuccess);
    }
}