using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JupiterPrime.Application.Strategies;

namespace JupiterPrime.Application.Interfaces;

public interface IWebDriverAdapter : IDisposable, IAsyncDisposable
{
    void NavigateToUrl(string url);
    IWebElementAdapter FindElementById(string id);
    IWebElementAdapter FindElementByTestId(string testId);
    IWebElementAdapter FindElementByXPath(string xpath);
    IWebElementAdapter FindElementByClassName(string className);
    IReadOnlyCollection<IWebElementAdapter> FindElementsByCssSelector(string cssSelector);
    IReadOnlyCollection<IWebElementAdapter> FindElementsByXPath(string xpath);
    IReadOnlyCollection<IWebElementAdapter> FindElementsByClassName(string className);
    string GetCurrentUrl();
}
