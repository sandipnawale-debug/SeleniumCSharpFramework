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
            // CI=true is set automatically by Azure DevOps hosted agents -> run headless there
            bool runHeadless = Environment.GetEnvironmentVariable("CI") == "true" ||
                                Environment.GetEnvironmentVariable("TF_BUILD") == "True";

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
