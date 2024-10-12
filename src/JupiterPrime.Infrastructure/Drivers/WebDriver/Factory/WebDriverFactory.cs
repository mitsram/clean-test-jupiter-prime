using JupiterPrime.Application.Interfaces;
using JupiterPrime.Infrastructure.Drivers.WebDriver.Adapters;
using Microsoft.Playwright;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;

namespace JupiterPrime.Infrastructure.Drivers.WebDriver;

public class WebDriverFactory : IWebDriverFactory, IAsyncDisposable
{
    private readonly WebDriverType driverType;
    private IWebDriverAdapter driverAdapter;
    private IPlaywright playwright;
    private IBrowser browser;

    public WebDriverFactory(WebDriverType webDriverType)
    {
        driverType = webDriverType;
    }

    public async Task<IWebDriverAdapter> CreateWebDriver()
    {
        switch (driverType)
        {
            case WebDriverType.Selenium:
                driverAdapter = CreateSeleniumWebDriver();
                return driverAdapter;
            case WebDriverType.Playwright:
                driverAdapter = await CreatePlaywrightWebDriver();
                return driverAdapter;
            default:
                throw new ArgumentException("Invalid WebDriver type");
        }
    }

    private IWebDriverAdapter CreateSeleniumWebDriver()
    {
        var options = new ChromeOptions();
        // options.AddArgument("--headless");
        var driver = new ChromeDriver(options);
        return new SeleniumWebDriverAdapter(driver);
    }

    private async Task<IWebDriverAdapter> CreatePlaywrightWebDriver()
    {
        playwright = await Playwright.CreateAsync();
        browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });
        var page = await browser.NewPageAsync();
        return new PlaywrightWebDriverAdapter(page);
    }

    public async ValueTask DisposeAsync()
    {
        if (driverAdapter != null)
        {
            await driverAdapter.DisposeAsync();
        }

        if (browser != null)
        {
            await browser.DisposeAsync();
        }

        playwright?.Dispose();
    }
}

public enum WebDriverType
{
    Selenium,
    Playwright
}

