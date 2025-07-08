using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Linq;

namespace SteamTest
{
    public class SteamGameTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private ExtentReports extent;
        private ExtentTest? test;

        [SetUp]
        public void Setup()
        {
            string browser = TestContext.Parameters.Get("browser", "Chrome");
            InitializeDriver(browser);

            var reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"SeleniumReport_{DateTime.Now:yyyyMMdd_HHmmss}.html");
            var htmlReporter = new ExtentSparkReporter(reportPath);
            extent = new ExtentReports();
            extent.AttachReporter(htmlReporter);
        }
        
        [Test]
        public void SteamGameTest_Case()
        {

            test = extent.CreateTest("Search and navigate to the Steam About page");

            try
            {
                #region pre-condition
                test.Log(Status.Info, "Pre-condition 0.1: The browser should open in incognito mode");
                test.Log(Status.Info, "Pre-condition 0.2: Go to https://store.steampowered.com");
                driver.Navigate().GoToUrl("https://store.steampowered.com/");
                #endregion pre-condition

                #region TestStep 1

                #region Action
                test.Log(Status.Info, "Step 1: Type 'FIFA' in the search field");
                var searchBox = wait.Until(d => d.FindElement(By.Id("store_nav_search_term")));
                searchBox.Clear();
                searchBox.SendKeys("FIFA");
                #endregion Action

                #region Verification 1.1
                wait.Until(d => d.FindElements(By.CssSelector(".match_app")).Count > 1);

                var resultTitles = driver.FindElements(By.CssSelector(".match_app")).ToList();
                string firstTitle = resultTitles[0].Text.Trim();
                firstTitle =firstTitle[..firstTitle.IndexOf('\r')];
                string secondTitle = resultTitles[1].Text.Trim();

                try
                {
                    test.Log(Status.Info, $"Verification 1.1: first search results: 1st = '{firstTitle}'");
                    Assert.That(firstTitle, Is.EqualTo("EA SPORTS FC™ 25").IgnoreCase);
                    test.Log(Status.Pass, $"Expected Result 1.1: The first search result is 'EA SPORTS FCTM 25': '{firstTitle}'");
                }
                catch (AssertionException)
                {
                    test.Log(Status.Fail, $"Verification 1.1: Verifed search results: expected 1st item = 'EA SPORTS FC™ 25', actual 1st item = '{firstTitle}'");
                }
                #endregion Verification 1.1

                #region Verification 1.2
                try
                {
                    test.Log(Status.Info, $"Verification 1.2: second search results: 2st = '{secondTitle}'");
                    Assert.That(secondTitle, Is.EqualTo("FIFA 22").IgnoreCase);
                    test.Log(Status.Pass, $"Expected Result 1.2: The second search result is 'FIFA 22': '{secondTitle}'");
                }
                catch (AssertionException)
                {
                    test.Log(Status.Fail, $"Verification 1.2: Verified search results: expected 2nd item = 'FIFA 22', actual 2nd item = '{secondTitle}'");
                }
                #endregion Verification 1.2

                #endregion TestStep 1

                #region TestStep 2

                #region Action
                test.Log(Status.Info, "Step 2: Clicked first result using JavaScript");
                var firstRow = driver.FindElements(By.CssSelector(".match_app")).First();
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", firstRow);
                #endregion Action

                #region Verification 2.1
                test.Log(Status.Info, $"Verification 2.1: The Game page is displayed");
                var appPage = wait.Until(d => d.Url.Contains("store.steampowered.com/app"));
                Assert.True(appPage);
                test.Log(Status.Pass, "Expected Result 2.1: The Game page is displayed");
                #endregion Verification 2.1

                #region Verification 2.2
                test.Log(Status.Info, $"Verification 2.2: The game name equals the game name from the 1st search result");
                var detailTitle = wait.Until(d => d.FindElement(By.CssSelector(".apphub_AppName"))).Text.Trim();
                try
                {
                    Assert.That(detailTitle, Is.EqualTo(firstTitle).IgnoreCase);
                    test.Log(Status.Pass, $"Expected Result 2.2: The game name equals the game name from the 1st search result");
                }
                catch (AssertionException)
                {
                    test.Log(Status.Fail, $"Verification 2.2: Expected game name = '{firstTitle}', actual game name = '{detailTitle}'");
                }
                #endregion Verification 2.1

                #endregion TestStep 2

                #region TestStep 3
                test.Log(Status.Pass, "Step 3: Clicked 'Download' button");
                var downloadBtn = wait.Until(d => d.FindElement(By.XPath("//*[@id=\"demoGameBtn\"]/a")));
                downloadBtn.Click();
                test.Log(Status.Pass, "Step 3: Clicked 'Download' button");
                #endregion TestStep 3

                #region TestStep 4
                #region Action
                test.Log(Status.Pass, "Step 4: Click 'No, I need Steam' button");
                var noSteamBtn = wait.Until(d => d.FindElement(By.XPath("//a[*[text()='No, I need Steam']]")));
                noSteamBtn.Click();
                #endregion Action

                #region Verification 4.1
                test.Log(Status.Info, $"Verification 4.1: 'About Steam' page is displayed");
                var aboutPage = wait.Until(d => d.Url.Contains("store.steampowered.com/about"));

                Assert.True(aboutPage);
                var aboutHeading = wait.Until(d => d.FindElement(By.Id("about_header_area")));
                Assert.That(aboutHeading.Enabled);
                test.Log(Status.Pass, "Expected Result 4.1: 'About Steam' page is displayed");
                #endregion Verification 4.1

                #region Verification 4.2
                test.Log(Status.Info, $"Verification 4.2: 'Install Steam' button is clickable");
                var installBtn = wait.Until(d => d.FindElement(By.ClassName("about_install_steam_link")));
                Assert.That(installBtn.Displayed && installBtn.Enabled);
                test.Log(Status.Pass, "Expected Result 4.2: 'Install Steam' button is clickable");
                #endregion Verification 4.2

                #region Verification 4.3
                test.Log(Status.Info, "Verification 4.3: 'Playing Now gamers' are less than 'Online gamers'");
                int playingNow = ParseSteamStat("online_stat_label gamers_in_game");
                int onlineNow = ParseSteamStat("online_stat_label gamers_online");
                Assert.That(playingNow, Is.LessThan(onlineNow));
                test.Log(Status.Pass, $"Expected Result 4.3: 'Playing Now' ({playingNow}) is less than 'Online' ({onlineNow})");
                #endregion Verification 4.3

                #endregion TestStep 4
            }
            catch (Exception ex)
            {
                test.Fail("Test failed: " + ex.Message);
                test.AddScreenCaptureFromPath(TakeScreenshot("failure"));
                throw;
            }
        }
        private int ParseSteamStat(string cssClass)
        {
            var stat = wait.Until(d => d.FindElement(By.XPath($"//div[div[@class='{cssClass}']]")));
            var rawText = stat.Text.Replace(",", "").Trim();
            var match = System.Text.RegularExpressions.Regex.Match(rawText, @"\d+");
            return match.Success ? int.Parse(match.Value) : 0;
        }

        private string TakeScreenshot(string name)
        {
            var ss = ((ITakesScreenshot)driver).GetScreenshot();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{name}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            ss.SaveAsFile(path);
            TestContext.AddTestAttachment(path, name);
            return path;
        }

        [TearDown]
        public void TearDown()
        {
            driver?.Quit();
            driver?.Dispose();
            extent?.Flush();
        }

        private void InitializeDriver(string browser)
        {
            if (browser.Equals("Chrome", StringComparison.OrdinalIgnoreCase))
            {
                var options = new ChromeOptions();
                options.AddArgument("--incognito");
                driver = new ChromeDriver(options);
            }
            else if (browser.Equals("Edge", StringComparison.OrdinalIgnoreCase))
            {
                var options = new EdgeOptions();
                options.AddArgument("inprivate");
                driver = new EdgeDriver(options);
            }
            else if (browser.Equals("Firefox", StringComparison.OrdinalIgnoreCase))
            {
                var options = new FirefoxOptions();
                options.AddArgument("-private");
                driver = new FirefoxDriver(options);
            }
            else
            {
                throw new ArgumentException("Unsupported browser: " + browser);
            }

            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

    }
}
