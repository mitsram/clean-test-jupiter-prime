using System;
using System.Threading.Tasks;

namespace JupiterPrime.Application.Interfaces
{
    public interface IWebDriverFactory : IAsyncDisposable
    {
        Task<IWebDriverAdapter> CreateWebDriver();
    }
}

