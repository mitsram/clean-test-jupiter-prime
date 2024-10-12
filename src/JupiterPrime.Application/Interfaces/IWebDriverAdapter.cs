using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JupiterPrime.Application.Strategies;

namespace JupiterPrime.Application.Interfaces;

public interface IWebDriverAdapter : IAsyncDisposable
{
    Task NavigateToUrl(string url);
    StrategyElement FindElementById(string id);
    StrategyElement FindElementByXPath(string xpath);
    StrategyElement FindElementByClassName(string className);
    Task<IReadOnlyCollection<StrategyElement>> FindElementsByCssSelector(string cssSelector);
    Task<IReadOnlyCollection<StrategyElement>> FindElementsByXPath(string xpath);
    Task<IReadOnlyCollection<StrategyElement>> FindElementsByClassName(string className);
    string GetCurrentUrl();
}
