using NUnit.Framework;
using SeleniumCSharpFramework.Pages;
using SeleniumCSharpFramework.Utilities;

namespace SeleniumCSharpFramework.Tests
{
    [TestFixture]
    [Category("Regression")]
    public class LoginTests : BaseTest
    {
        private const string LoginUrl = "https://the-internet.herokuapp.com/login";
        private LoginPage _loginPage = null!;

        [SetUp]
        public new void SetUp()
        {
            _loginPage = new LoginPage(Driver);
            _loginPage.NavigateTo(LoginUrl);
        }

        [Test]
        [Category("Smoke")]
        public void ValidLogin_ShouldShowSuccessMessage()
        {
            ExtentReportManager.LogInfo("Navigating to login page and entering valid credentials");
            _loginPage.Login("tomsmith", "SuperSecretPassword!");

            string message = _loginPage.GetFlashMessage();

            ExtentReportManager.LogInfo($"Flash message received: {message}");
            Assert.That(message, Does.Contain("You logged into a secure area"));
        }

        [Test]
        [Category("Smoke")]
        public void InvalidLogin_ShouldShowErrorMessage()
        {
            ExtentReportManager.LogInfo("Attempting login with invalid credentials");
            _loginPage.Login("invalidUser", "wrongPassword");

            string message = _loginPage.GetFlashMessage();

            ExtentReportManager.LogInfo($"Flash message received: {message}");
            Assert.That(message, Does.Contain("Your username is invalid"));
        }

        [Test]
        public void EmptyCredentials_ShouldShowErrorMessage()
        {
            ExtentReportManager.LogInfo("Attempting login with empty username and password");
            _loginPage.Login("", "");

            string message = _loginPage.GetFlashMessage();

            ExtentReportManager.LogInfo($"Flash message received: {message}");
            Assert.That(message, Does.Contain("invalid"));
        }
    }
}
