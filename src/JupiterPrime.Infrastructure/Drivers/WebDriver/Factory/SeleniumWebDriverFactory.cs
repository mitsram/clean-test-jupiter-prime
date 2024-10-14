using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using System;
using System.Threading.Tasks;
using JupiterPrime.Application.Enums;
using JupiterPrime.Application.Interfaces;
using JupiterPrime.Infrastructure.Drivers.WebDriver.Adapters;


namespace JupiterPrime.Infrastructure.Drivers.WebDriver.Factory
{
    public class SeleniumWebDriverFactory : IWebDriverAdapterFactory
    {
        public Task<IWebDriverAdapter> CreateWebDriver(BrowserType browserType)
        {
            IWebDriver driver = browserType switch
            {
                BrowserType.Chrome => new ChromeDriver(new ChromeOptions()),
                BrowserType.Firefox => new FirefoxDriver(new FirefoxOptions()),
                BrowserType.Edge => new EdgeDriver(new EdgeOptions()),
                _ => throw new ArgumentException("Unsupported browser type for Selenium")
            };

            return Task.FromResult<IWebDriverAdapter>(new SeleniumWebDriverAdapter(driver));
        }
    }
}

