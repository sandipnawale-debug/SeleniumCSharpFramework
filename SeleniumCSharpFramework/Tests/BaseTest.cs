using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using SeleniumCSharpFramework.Utilities;

namespace SeleniumCSharpFramework.Tests
{
    [TestFixture]
    public abstract class BaseTest
    {
        protected IWebDriver Driver => DriverFactory.Driver;

        [OneTimeSetUp]
        public void GlobalSetUp()
        {
            ExtentReportManager.GetInstance();
        }

        [SetUp]
        public void SetUp()
        {
            // CI=true / TF_BUILD=True are set automatically by Azure DevOps hosted agents -> run headless there.
            // HEADLESS lets you force it explicitly (e.g. from the pipeline yaml) regardless of auto-detection.
            bool runHeadless =
                string.Equals(Environment.GetEnvironmentVariable("HEADLESS"), "true", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(Environment.GetEnvironmentVariable("TF_BUILD"), "true", StringComparison.OrdinalIgnoreCase);

            Console.WriteLine($"[BaseTest] CI={Environment.GetEnvironmentVariable("CI")}, " +
                               $"TF_BUILD={Environment.GetEnvironmentVariable("TF_BUILD")}, " +
                               $"HEADLESS={Environment.GetEnvironmentVariable("HEADLESS")}, " +
                               $"runHeadless={runHeadless}");

            DriverFactory.InitDriver(headless: runHeadless);
            ExtentReportManager.CreateTest(TestContext.CurrentContext.Test.Name);
        }

        [TearDown]
        public void TearDown()
        {
            var result = TestContext.CurrentContext.Result.Outcome.Status;

            if (result == TestStatus.Failed)
            {
                string screenshotPath = CaptureScreenshot(TestContext.CurrentContext.Test.Name);
                ExtentReportManager.LogFailWithScreenshot(
                    $"Test failed: {TestContext.CurrentContext.Result.Message}", screenshotPath);
            }
            else if (result == TestStatus.Passed)
            {
                ExtentReportManager.LogPass("Test passed");
            }

            DriverFactory.QuitDriver();
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            ExtentReportManager.Flush();
        }

        private string CaptureScreenshot(string testName)
        {
            var screenshotsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults", "Screenshots");
            Directory.CreateDirectory(screenshotsDir);

            string fileName = $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            string filePath = Path.Combine(screenshotsDir, fileName);

            var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
            screenshot.SaveAsFile(filePath);

            return filePath;
        }
    }
}