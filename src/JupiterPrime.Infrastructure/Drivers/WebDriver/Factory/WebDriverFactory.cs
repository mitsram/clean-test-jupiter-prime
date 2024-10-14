using System;
using System.Threading.Tasks;
using JupiterPrime.Application.Enums;
using JupiterPrime.Application.Interfaces;
using JupiterPrime.Infrastructure.Drivers.WebDriver.Factory;

namespace JupiterPrime.Infrastructure.Drivers.WebDriver.Factory
{
    public class WebDriverFactory : IWebDriverFactory, IAsyncDisposable
    {
        private readonly WebDriverType driverType;
        private readonly BrowserType browserType;
        private IWebDriverAdapter driverAdapter;
        private IWebDriverAdapterFactory adapterFactory;

        public WebDriverFactory(WebDriverType webDriverType, BrowserType browserType)
        {
            driverType = webDriverType;
            this.browserType = browserType;
            adapterFactory = CreateAdapterFactory(webDriverType);
        }

        private IWebDriverAdapterFactory CreateAdapterFactory(WebDriverType webDriverType)
        {
            return webDriverType switch
            {
                WebDriverType.Selenium => new SeleniumWebDriverFactory(),
                WebDriverType.Playwright => new PlaywrightWebDriverFactory(),
                _ => throw new ArgumentException("Invalid WebDriver type")
            };
        }

        public async Task<IWebDriverAdapter> CreateWebDriver()
        {
            driverAdapter = await adapterFactory.CreateWebDriver(browserType);
            return driverAdapter;
        }

        public async ValueTask DisposeAsync()
        {
            if (driverAdapter != null)
            {
                await driverAdapter.DisposeAsync();
            }

            if (adapterFactory is IAsyncDisposable disposableFactory)
            {
                await disposableFactory.DisposeAsync();
            }
        }
    }
}
