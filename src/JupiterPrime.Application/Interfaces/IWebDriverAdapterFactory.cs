using System.Threading.Tasks;
using JupiterPrime.Application.Enums;
using JupiterPrime.Application.Interfaces;

namespace JupiterPrime.Infrastructure.Drivers.WebDriver.Factory
{
    public interface IWebDriverAdapterFactory
    {
        Task<IWebDriverAdapter> CreateWebDriver(BrowserType browserType);
    }
}

