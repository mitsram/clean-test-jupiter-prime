using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using BrowserType = JupiterPrime.Application.Enums.BrowserType;
using JupiterPrime.Application.Interfaces;
using JupiterPrime.Infrastructure.Drivers.WebDriver.Adapters;


namespace JupiterPrime.Infrastructure.Drivers.WebDriver.Factory
{
    public class PlaywrightWebDriverFactory : IWebDriverAdapterFactory, IAsyncDisposable
    {
        private IPlaywright playwright;
        private IBrowser browser;

        public async Task<IWebDriverAdapter> CreateWebDriver(BrowserType browserType)
        {
            playwright = await Playwright.CreateAsync();
            IBrowserType playwrightBrowserType = browserType switch
            {
                BrowserType.Chrome => playwright.Chromium,
                BrowserType.Firefox => playwright.Firefox,
                BrowserType.Edge => playwright.Chromium, // Edge uses Chromium
                _ => throw new ArgumentException("Unsupported browser type for Playwright")
            };

            browser = await playwrightBrowserType.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });
            var page = await browser.NewPageAsync();
            return new PlaywrightWebDriverAdapter(page);
        }

        public async ValueTask DisposeAsync()
        {
            if (browser != null)
            {
                await browser.DisposeAsync();
            }
            playwright?.Dispose();
        }
    }
}

