using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumCSharpFramework.Utilities
{
    /// <summary>
    /// ThreadLocal WebDriver factory - safe for parallel test execution.
    /// </summary>
    public static class DriverFactory
    {
        private static readonly ThreadLocal<IWebDriver?> _driver = new();

        public static IWebDriver Driver => _driver.Value
            ?? throw new InvalidOperationException("Driver not initialized. Call InitDriver() first.");

        public static void InitDriver(bool headless = false)
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--remote-allow-origins=*");

            // Headless mode is required for CI agents (Azure DevOps hosted agents have no display)
            if (headless)
            {
                options.AddArgument("--headless=new");
                options.AddArgument("--window-size=1920,1080");
                options.AddArgument("--disable-gpu");
                options.AddArgument("--no-sandbox");
            }

            var driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero; // rely on explicit waits only
            _driver.Value = driver;
        }

        public static void QuitDriver()
        {
            _driver.Value?.Quit();
            _driver.Value?.Dispose();
            _driver.Value = null;
        }
    }
}
