using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JupiterPrime.Application.Strategies;

namespace JupiterPrime.Application.Interfaces;

public interface IWebDriverAdapter : IDisposable, IAsyncDisposable
{
    string GetCurrentUrl();
    void NavigateToUrl(string url);
    IWebElementAdapter FindElementById(string id);
    IWebElementAdapter FindElementByClassName(string className);
    IWebElementAdapter FindElementByTestId(string testId);
    IWebElementAdapter FindElementByTestId(IWebElementAdapter parentElement, string testId);
    IWebElementAdapter FindElementByXPath(string xpath);
    IWebElementAdapter FindElementByXPath(IWebElementAdapter parentElement, string xpath);    
    IWebElementAdapter FindElementByTitle(string title);
    IWebElementAdapter FindElementByTitle(IWebElementAdapter parentElement, string title);
    IWebElementAdapter FindElementByDataLocator(string dataLocator);
    IWebElementAdapter FindElementByDataLocator(IWebElementAdapter parentElement, string dataLocator);    
    IReadOnlyCollection<IWebElementAdapter> FindElementsByClassName(string className);    
    IReadOnlyCollection<IWebElementAdapter> FindElementsByCssSelector(string cssSelector);
    IReadOnlyCollection<IWebElementAdapter> FindElementsByTestId(string testId);
    IReadOnlyCollection<IWebElementAdapter> FindElementsByXPath(string xpath);   
    IReadOnlyCollection<IWebElementAdapter> FindElementsByXPath(IWebElementAdapter parentElement, string xpath);
    IReadOnlyCollection<IWebElementAdapter> FindElementsByDataLocator(string dataLocator);
    IReadOnlyCollection<IWebElementAdapter> FindChildElements(IWebElementAdapter parentElement, string selector);
    void WaitForTextToChange(IWebElementAdapter element, string oldText, int timeoutInSeconds = 10);
    void WaitForElementToDisappear(IWebElementAdapter element, int timeoutInSeconds = 10);
    void WaitForPageToLoad(int timeoutInSeconds = 30);
    void WaitForElementToBeVisible(string selector, int timeoutInSeconds = 30);
    void WaitForElementToBeHidden(string selector, int timeoutInSeconds = 30);
    void WaitForNetworkIdle(int timeoutInSeconds = 30);
}
