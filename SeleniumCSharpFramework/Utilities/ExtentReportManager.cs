using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace SeleniumCSharpFramework.Utilities
{
    /// <summary>
    /// Singleton wrapper around ExtentReports. Generates an HTML report
    /// with per-test steps, status, and screenshots on failure.
    /// Report is written to TestResults/ExtentReport_<timestamp>.html
    /// </summary>
    public static class ExtentReportManager
    {
        private static AventStack.ExtentReports.ExtentReports? _extent;
        private static readonly object _lock = new();
        [ThreadStatic] private static ExtentTest? _test;

        public static string ReportPath { get; private set; } = string.Empty;

        public static AventStack.ExtentReports.ExtentReports GetInstance()
        {
            lock (_lock)
            {
                if (_extent == null)
                {
                    var resultsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults");
                    Directory.CreateDirectory(resultsDir);

                    ReportPath = Path.Combine(resultsDir, $"ExtentReport_{DateTime.Now:yyyyMMdd_HHmmss}.html");

                    var htmlReporter = new ExtentHtmlReporter(ReportPath);
                    htmlReporter.Config.DocumentTitle = "Selenium C# Automation Report";
                    htmlReporter.Config.ReportName = "Regression Execution Report";
                    htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Standard;

                    _extent = new AventStack.ExtentReports.ExtentReports();
                    _extent.AttachReporter(htmlReporter);
                    _extent.AddSystemInfo("Environment", "QA");
                    _extent.AddSystemInfo("Framework", "Selenium + NUnit + C#");
                }
                return _extent;
            }
        }

        public static ExtentTest CreateTest(string testName, string? description = null)
        {
            _test = GetInstance().CreateTest(testName, description);
            return _test;
        }

        public static ExtentTest? GetTest() => _test;

        public static void LogPass(string message) => _test?.Pass(message);
        public static void LogInfo(string message) => _test?.Info(message);
        public static void LogFail(string message) => _test?.Fail(message);

        public static void LogFailWithScreenshot(string message, string screenshotPath)
        {
            _test?.Fail(message,
               AventStack.ExtentReports.MediaEntityBuilder
    .CreateScreenCaptureFromPath(screenshotPath).Build());

        }

        public static void Flush()
        {
            _extent?.Flush();
        }
    }
}
