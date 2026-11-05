using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace BIE.E2ETests;

// Jeder Test erhält einen eigenen App-Prozess, Port, Browser und Datenbestand.
public abstract class BrowserTestBase
{
    protected IWebDriver Driver { get; private set; } = null!;
    protected WebDriverWait Wait { get; private set; } = null!;

    private Process? app;
    private string tempFolder = "";
    private readonly List<string> appLog = [];
    public TestContext TestContext { get; set; } = null!;

    [TestInitialize]
    public async Task Start()
    {
        try
        {
            var root = new DirectoryInfo(AppContext.BaseDirectory);
            while (root is not null && !File.Exists(Path.Combine(root.FullName, "BIE.slnx")))
                root = root.Parent;
            if (root is null)
                throw new InvalidOperationException("BIE.slnx nicht gefunden.");
            var configuration = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration ?? "Debug";
            var webRoot = Path.Combine(root.FullName, "src", "BIE.Web");
            var dll = Path.Combine(webRoot, "bin", configuration, "net10.0", "BIE.Web.dll");
            if (!File.Exists(dll))
                throw new FileNotFoundException("Zuerst dotnet build BIE.slnx ausführen.", dll);
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            var url = $"http://127.0.0.1:{port}";
            tempFolder = Path.Combine(Path.GetTempPath(), "bie-e2e-" + Guid.NewGuid());
            Directory.CreateDirectory(tempFolder);
            var start = new ProcessStartInfo("dotnet")
            {
                WorkingDirectory = webRoot,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            start.ArgumentList.Add(dll);
            start.ArgumentList.Add("--urls");
            start.ArgumentList.Add(url);
            start.Environment["DatabasePath"] = Path.Combine(tempFolder, "e2e.db");
            start.Environment["ASPNETCORE_ENVIRONMENT"] = "Testing";
            app = new Process
            {
                StartInfo = start
            };
            app.OutputDataReceived += (_, e) =>
            {
                if (e.Data is not null)
                    lock (appLog)
                        appLog.Add(e.Data);
            };
            app.ErrorDataReceived += (_, e) =>
            {
                if (e.Data is not null)
                    lock (appLog)
                        appLog.Add(e.Data);
            };
            app.Start();
            app.BeginOutputReadLine();
            app.BeginErrorReadLine();
            using var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(2)
            };
            var ready = false;
            var deadline = DateTime.UtcNow.AddSeconds(30);
            while (DateTime.UtcNow < deadline)
            {
                if (app.HasExited)
                    throw new InvalidOperationException("App-Prozess wurde beendet.");
                try
                {
                    ready = (await client.GetAsync(url + "/health")).IsSuccessStatusCode;
                }
                catch (HttpRequestException)
                {
                }
                catch (TaskCanceledException)
                {
                }

                if (ready)
                    break;
                await Task.Delay(200);
            }

            if (!ready)
                throw new TimeoutException("Die App war nach 30 Sekunden nicht bereit.");
            var options = new ChromeOptions();
            if (Environment.GetEnvironmentVariable("SHOW_BROWSER") != "1")
                options.AddArgument("--headless=new");
            options.AddArgument("--window-size=1440,1100");
            options.AddArgument("--disable-dev-shm-usage");
            Driver = new ChromeDriver(options); // Selenium Manager verwaltet den Treiber.
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            Wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
            Driver.Navigate().GoToUrl(url);
        }
        catch
        {
            Cleanup();
            throw;
        }
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (TestContext.CurrentTestOutcome != UnitTestOutcome.Passed)
        {
            var resultDir = TestContext.ResultsDirectory ?? Path.Combine(Path.GetTempPath(), "bie-results");
            Directory.CreateDirectory(resultDir);
            var stem = (TestContext.TestName ?? "test") + "-" + Guid.NewGuid().ToString("N");
            try
            {
                if (Driver is ITakesScreenshot screenshot)
                {
                    var file = Path.Combine(resultDir, stem + ".png");
                    screenshot.GetScreenshot().SaveAsFile(file);
                    TestContext.AddResultFile(file);
                }
            }
            catch (WebDriverException)
            {
            }

            var logFile = Path.Combine(resultDir, stem + ".log");
            lock (appLog)
                File.WriteAllLines(logFile, appLog);
            TestContext.AddResultFile(logFile);
        }

        try
        {
            Driver?.Quit();
        }
        catch (WebDriverException)
        {
        }

        Driver?.Dispose();
        Driver = null!;
        if (app is not null)
        {
            if (!app.HasExited)
            {
                app.Kill(entireProcessTree: true);
                app.WaitForExit(5000);
            }

            app.Dispose();
            app = null;
        }

        if (Directory.Exists(tempFolder))
            Directory.Delete(tempFolder, recursive: true);
    }

    protected void Fill(string name, string purpose, string days)
    {
        var field = Driver.FindElement(By.Id("name"));
        field.Clear();
        field.SendKeys(name);
        new SelectElement(Driver.FindElement(By.Id("planet"))).SelectByText("Orion");
        new SelectElement(Driver.FindElement(By.Id("purpose"))).SelectByText(purpose);
        var duration = Driver.FindElement(By.Id("days"));
        duration.Clear();
        duration.SendKeys(days);
    }

    protected IWebElement Decision(string status) => Wait.Until(driver =>
    {
        var result = driver.FindElement(By.Id("result"));
        return result.Displayed && result.GetDomAttribute("data-decision") == status ? result : null;
    });
}
