# Selenium C# Automation Framework

A minimal, interview-ready Selenium + NUnit + C# framework with:
- **Page Object Model** (`Pages/LoginPage.cs`)
- **WaitHelper** — centralized explicit waits (`Utilities/WaitHelper.cs`)
- **ExtentReportManager** — HTML report with screenshots on failure (`Utilities/ExtentReportManager.cs`)
- **DriverFactory** — ThreadLocal WebDriver, headless-aware for CI (`Utilities/DriverFactory.cs`)
- **Azure DevOps pipeline** (`azure-pipelines.yml`) — build, test, publish results + ExtentReport artifact

Sample tests run against the public demo site `https://the-internet.herokuapp.com/login`.

---

## 1. Run locally

Prerequisites: [.NET 8 SDK](https://dotnet.microsoft.com/download), Google Chrome installed.

```bash
dotnet restore
dotnet build
dotnet test --filter "Category=Smoke"
```

After the run:
- ExtentReport HTML: `bin/Debug/net8.0/TestResults/ExtentReport_<timestamp>.html`
- Screenshots (on failure): `bin/Debug/net8.0/TestResults/Screenshots/`

---

## 2. Push to GitHub

```bash
git init
git add .
git commit -m "Initial commit: Selenium C# framework with ExtentReports and WaitHelper"
git branch -M main
git remote add origin https://github.com/<your-username>/<your-repo-name>.git
git push -u origin main
```

---

## 3. Run the pipeline from Azure DevOps

1. In Azure DevOps, go to **Pipelines → New Pipeline**
2. Choose **GitHub** as the source, authorize/select your repo
3. Select **Existing Azure Pipelines YAML file** → point to `/azure-pipelines.yml`
4. Save and run

What happens automatically:
- On every push to `main`/`develop` → **Smoke** tests run (fast feedback)
- Every night at 2 AM → **Regression** category runs (change `testCategory` variable or add a second pipeline for this if you want a separate scheduled definition)
- Test results appear under the pipeline's **Tests** tab
- The ExtentReport HTML is available under the pipeline run's **Artifacts** → `ExtentReport`

To run the full regression suite manually: **Run Pipeline** → set the `testCategory` variable override to `Regression`.

---

## 4. Project structure

```
SeleniumCSharpFramework/
├── Pages/
│   └── LoginPage.cs          # Page Object for the login page
├── Tests/
│   ├── BaseTest.cs           # SetUp/TearDown, driver + report lifecycle
│   └── LoginTests.cs         # Sample smoke + regression tests
├── Utilities/
│   ├── DriverFactory.cs      # ThreadLocal WebDriver (headless on CI)
│   ├── ExtentReportManager.cs
│   └── WaitHelper.cs
├── azure-pipelines.yml
├── .gitignore
└── SeleniumCSharpFramework.csproj
```

## 5. Extending this framework

- Add more Page Object classes under `Pages/`
- Tag tests with `[Category("Smoke")]` or `[Category("Regression")]` to control what each pipeline trigger runs
- Add a `RestSharp` API test project alongside this one (separate `.csproj`) and add a second `DotNetCoreCLI@2` test step in the pipeline for API regression
