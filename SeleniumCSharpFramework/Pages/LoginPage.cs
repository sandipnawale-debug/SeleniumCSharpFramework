using OpenQA.Selenium;
using SeleniumCSharpFramework.Utilities;

namespace SeleniumCSharpFramework.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;
        private readonly WaitHelper _wait;

        private readonly By usernameField = By.Id("username");
        private readonly By passwordField = By.Id("password");
        private readonly By loginButton = By.CssSelector("button[type='submit']");
        private readonly By flashMessage = By.Id("flash");

        public LoginPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WaitHelper(driver);
        }

        public void NavigateTo(string url)
        {
            _driver.Navigate().GoToUrl(url);
            _wait.WaitForPageLoadComplete();
        }

        public void EnterUsername(string username)
        {
            var field = _wait.WaitForElementVisible(usernameField);
            field.Clear();
            field.SendKeys(username);
        }

        public void EnterPassword(string password)
        {
            var field = _wait.WaitForElementVisible(passwordField);
            field.Clear();
            field.SendKeys(password);
        }

        public void ClickLogin()
        {
            var button = _wait.WaitForElementClickable(loginButton);
            button.Click();
        }

        public string GetFlashMessage()
        {
            var message = _wait.WaitForElementVisible(flashMessage);
            return message.Text.Trim();
        }

        public void Login(string username, string password)
        {
            EnterUsername(username);
            EnterPassword(password);
            ClickLogin();
        }
    }
}
