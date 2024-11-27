using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace LibriProjectTest
{
    public class Tests
    {
        ChromeDriver driver; // IWebDriver if want Firefox too

        [OneTimeSetUp]
        public void Setup()
        {
            // Open the browser
            string path = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
            driver = new ChromeDriver(path + @"\drivers\");
        }

        [Test]
        public void OpenLibriHomePage()
        {
            // Test Home Page
            driver.Navigate().GoToUrl("https://libri-project.vercel.app/");
            Assert.That(driver.Title, Is.EqualTo("Home Page"), "The page title does not match.");

            // Test Products Page
            driver.Navigate().GoToUrl("https://libri-project.vercel.app/products");
            Assert.That(driver.Title, Is.EqualTo("Products Page"), "The page title does not match.");

            // Test About-Us Page
            driver.Navigate().GoToUrl("https://libri-project.vercel.app/about-us");
            Assert.That(driver.Title, Is.EqualTo("About-Us Page"), "The page title does not match.");
        }

        [Test]
        public void LoginTest()
        {
            // Ensure that the user is register
            EnsureUserRegistered();

            // Ensure that the client is logged in
            EnsureLoggedIn();

            // Assert that AuthorizationToken is exists
            bool isExists = new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(d =>
                ((IJavaScriptExecutor)d).ExecuteScript("return localStorage.getItem('AuthorizationToken')") != null);
            Assert.That(isExists, Is.True, "AuthorizationToken does not exist in localStorage.");
            Console.WriteLine($"Successfully got the AuthorizationToken.");
        }

        [Test]
        public void DeleteUser()
        {
            EnsureUserRegistered();
            EnsureLoggedIn();

            // if logged in go to profile
            driver.Navigate().GoToUrl("https://libri-project.vercel.app/profile");

            Thread.Sleep(1000);

            // Click on Delete account button
            driver.FindElement(By.CssSelector("button[popovertarget='delete-account']")).Click();

            // Press yes to verify account delete
            driver.FindElement(By.XPath("//button[text()='Yes']")).Click();

            try
            {
                // Check for an alert
                var alert = driver.SwitchTo().Alert();
                Console.WriteLine($"Alert detected with text: {alert.Text}");

                alert.Accept();
            }
            catch { }
        }

        public void EnsureUserRegistered()
        {
            // Goto Sign-up Page
            driver.Navigate().GoToUrl("https://libri-project.vercel.app/sign-up");

            // Insert user values
            driver.FindElement(By.CssSelector("input[id='email']")).SendKeys("asd@asd.com");
            driver.FindElement(By.CssSelector("input[id='username']")).SendKeys("asd");
            driver.FindElement(By.CssSelector("input[id='password']")).SendKeys("asd");

            // Click Sign Up button
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            Thread.Sleep(1000); // Wait for register process to complete
            try
            {
                // Check for an alert
                var alert = driver.SwitchTo().Alert();
                string message = $"Alert detected with text: {alert.Text}";

                alert.Accept();
                Console.WriteLine(message);
            }
            catch (NoAlertPresentException)
            {
                Console.WriteLine("asd");
            }
        }

        public void EnsureLoggedIn()
        {
            driver.Navigate().GoToUrl("https://libri-project.vercel.app/");
            // Check if already logged in by verifying localStorage or a specific element
            string isLoggedIn = (string)((IJavaScriptExecutor)driver).ExecuteScript("return localStorage.getItem('AuthorizationToken');");

            if (string.IsNullOrEmpty(isLoggedIn))
            {
                Console.WriteLine("Not logged in. Performing login...");
                // Goto Sign-In Page
                driver.Navigate().GoToUrl("https://libri-project.vercel.app/sign-in");

                // Insert user values
                driver.FindElement(By.CssSelector("input[name='username']")).SendKeys("asd");
                driver.FindElement(By.CssSelector("input[name='password']")).SendKeys("asd");

                // Click Sign Up button
                driver.FindElement(By.CssSelector("button[type='submit']")).Click();
                
                Thread.Sleep(1000); // Wait for login process to complete
                try
                {
                    // Check for an alert
                    var alert = driver.SwitchTo().Alert();
                    string message = $"Alert detected with text: {alert.Text}";

                    alert.Accept();
                    Assert.Fail(message);
                }
                catch (NoAlertPresentException)
                {
                    Console.WriteLine("Login successful.");
                }    
            }
            else
            {
                Console.WriteLine("Already logged in.");
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            // Shut down the browser
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
            }
        }
    }
}