using JupiterPrime.Application.Interfaces;
using JupiterPrime.Infrastructure.Drivers.WebDriver.Adapters;
using Microsoft.Playwright;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;

namespace JupiterPrime.Infrastructure.WebDriver
{
    public class WebDriverFactory : IWebDriverFactory, IAsyncDisposable
    {
        private readonly WebDriverType _webDriverType;
        private IWebDriverAdapter _currentDriver;
        private IPlaywright _playwright;
        private IBrowser _browser;

        public WebDriverFactory(WebDriverType webDriverType)
        {
            _webDriverType = webDriverType;
        }

        public async Task<IWebDriverAdapter> CreateWebDriver()
        {
            switch (_webDriverType)
            {
                case WebDriverType.Selenium:
                    _currentDriver = CreateSeleniumWebDriver();
                    return _currentDriver;
                case WebDriverType.Playwright:
                    _currentDriver = await CreatePlaywrightWebDriver();
                    return _currentDriver;
                default:
                    throw new ArgumentException("Invalid WebDriver type");
            }
        }

        private IWebDriverAdapter CreateSeleniumWebDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            var driver = new ChromeDriver(options);
            return new SeleniumWebDriverAdapter(driver);
        }

        private async Task<IWebDriverAdapter> CreatePlaywrightWebDriver()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });
            var page = await _browser.NewPageAsync();
            return new PlaywrightWebDriverAdapter(page);
        }

        public async ValueTask DisposeAsync()
        {
            if (_currentDriver != null)
            {
                await _currentDriver.DisposeAsync();
            }

            if (_browser != null)
            {
                await _browser.DisposeAsync();
            }

            _playwright?.Dispose();
        }
    }

    public enum WebDriverType
    {
        Selenium,
        Playwright
    }
}
