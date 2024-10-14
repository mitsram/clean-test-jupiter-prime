using System;
using System.Threading.Tasks;
using JupiterPrime.Application.Enums;

namespace JupiterPrime.Application.Interfaces
{
    public interface IWebDriverFactory : IAsyncDisposable
    {
        Task<IWebDriverAdapter> CreateWebDriver();
    }
}

