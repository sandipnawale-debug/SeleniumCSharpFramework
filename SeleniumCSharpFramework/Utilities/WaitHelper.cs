using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SeleniumCSharpFramework.Utilities
{
    /// <summary>
    /// Centralized explicit-wait helper. Avoids hard Thread.Sleep waits
    /// which are a common cause of flaky tests.
    /// </summary>
    public class WaitHelper
    {
        private readonly WebDriverWait _wait;
        private readonly IWebDriver _driver;

        public WaitHelper(IWebDriver driver, int timeoutInSeconds = 15)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            _wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
        }

        public IWebElement WaitForElementVisible(By locator)
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }

        public IWebElement WaitForElementClickable(By locator)
        {
            return _wait.Until(ExpectedConditions.ElementToBeClickable(locator));
        }

        public bool WaitForElementInvisible(By locator)
        {
            return _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(locator));
        }

        public bool WaitForTextPresent(By locator, string text)
        {
            return _wait.Until(ExpectedConditions.TextToBePresentInElementLocated(locator, text));
        }

        public bool WaitForUrlContains(string partialUrl)
        {
            return _wait.Until(ExpectedConditions.UrlContains(partialUrl));
        }

        public bool WaitForPageLoadComplete(int timeoutInSeconds = 15)
        {
            var pageLoadWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutInSeconds));
            return pageLoadWait.Until(driver =>
                ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState")!.Equals("complete"));
        }
    }
}
