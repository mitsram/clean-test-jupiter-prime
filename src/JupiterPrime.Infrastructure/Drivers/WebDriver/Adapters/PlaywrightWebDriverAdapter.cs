using Microsoft.Playwright;
using JupiterPrime.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JupiterPrime.Infrastructure.Drivers.WebDriver.Adapters;

public class PlaywrightWebDriverAdapter : IWebDriverAdapter
{
    private readonly IPage _page;
    private readonly int _defaultTimeoutMs;

    public PlaywrightWebDriverAdapter(IPage page)
    {
        _page = page ?? throw new ArgumentNullException(nameof(page));
        _defaultTimeoutMs = 30 * 5000;
    }

    public void NavigateToUrl(string url) => _page.GotoAsync(url).GetAwaiter().GetResult();

    public IWebElementAdapter FindElementById(string id) => 
        new PlaywrightWebElementAdapter(_page.Locator($"#{id}"));

    public IWebElementAdapter FindElementByTestId(string testId) =>
        new PlaywrightWebElementAdapter(_page.GetByTestId(testId));

    public IWebElementAdapter FindElementByXPath(string xpath) =>
        new PlaywrightWebElementAdapter(_page.Locator(xpath));

    public IWebElementAdapter FindElementByClassName(string className) =>
        new PlaywrightWebElementAdapter(_page.Locator($".{className}"));

    public IReadOnlyCollection<IWebElementAdapter> FindElementsByCssSelector(string cssSelector) =>
        _page.QuerySelectorAllAsync(cssSelector).GetAwaiter().GetResult()
            .Select(e => new PlaywrightElementHandleAdapter(e))
            .ToList();

    public IReadOnlyCollection<IWebElementAdapter> FindElementsByXPath(string xpath) =>
        _page.QuerySelectorAllAsync(xpath).GetAwaiter().GetResult()
            .Select(e => new PlaywrightElementHandleAdapter(e))
            .ToList();

    public IReadOnlyCollection<IWebElementAdapter> FindElementsByClassName(string className) =>
        _page.QuerySelectorAllAsync($".{className}").GetAwaiter().GetResult()
            .Select(e => new PlaywrightElementHandleAdapter(e))
            .ToList();

    public string GetCurrentUrl() => _page.Url;

    public void Dispose() => _page.CloseAsync().GetAwaiter().GetResult();

    public async ValueTask DisposeAsync() => await _page.CloseAsync();

    public IWebElementAdapter FindElementByDataLocator(string dataLocator)
    {
        // return new PlaywrightWebElementAdapter(_page.Locator($"[data-locator='{dataLocator}']"));
        var elements = _page.Locator($"[data-locator='{dataLocator}']").AllAsync().GetAwaiter().GetResult();
        var firstElement = elements.FirstOrDefault();
        return firstElement != null ? new PlaywrightWebElementAdapter(_page.Locator($"[data-locator='{dataLocator}']")) : null;
    }

    public IWebElementAdapter FindElementByDataLocator(IWebElementAdapter parentElement, string dataLocator)
    {
        var playwrightElement = (parentElement as PlaywrightWebElementAdapter)._element;
        // return new PlaywrightWebElementAdapter(playwrightElement.Locator($"[data-locator='{dataLocator}']"));
        var elements = playwrightElement.Locator($"[data-locator='{dataLocator}']").AllAsync().GetAwaiter().GetResult();
        var firstElement = elements.FirstOrDefault();
        return firstElement != null ? new PlaywrightWebElementAdapter(firstElement) : null;
    }

    public IReadOnlyCollection<IWebElementAdapter> FindElementsByTestId(string testId) 
    {
        var locator = _page.GetByTestId(testId);
        locator.First.WaitForAsync(new LocatorWaitForOptions 
        {
            State = WaitForSelectorState.Visible,
            Timeout = _defaultTimeoutMs
        }).GetAwaiter().GetResult();

        return locator.AllAsync().GetAwaiter().GetResult()
            .Select(e => new PlaywrightWebElementAdapter(e))
            .ToList();
    }
        

    // public IReadOnlyCollection<IWebElementAdapter> FindElementsByTestId(string testId) =>
    //     _page.GetByTestId(testId).AllAsync().GetAwaiter().GetResult()
    //         .Select(e => new PlaywrightWebElementAdapter(e))
    //         .ToList();

    public IWebElementAdapter FindElementByTestId(IWebElementAdapter parentElement, string testId)
    {
        var playwrightElement = (parentElement as PlaywrightWebElementAdapter)._element;
        return new PlaywrightWebElementAdapter(playwrightElement.Locator($"[data-testid='{testId}']"));
    }

    public IReadOnlyCollection<IWebElementAdapter> FindElementsByDataLocator(string dataLocator)
    {
        return _page.Locator($"[data-locator='{dataLocator}']").AllAsync().GetAwaiter().GetResult()
            .Select(e => new PlaywrightWebElementAdapter(e))
            .ToList();
    }

    public void WaitForTextToChange(IWebElementAdapter element, string oldText, int timeoutInSeconds = 10)
    {
        var playwrightElement = ((PlaywrightWebElementAdapter)element)._element;
        var startTime = DateTime.Now;
        while (DateTime.Now - startTime < TimeSpan.FromSeconds(timeoutInSeconds))
        {
            var currentText = playwrightElement.TextContentAsync().GetAwaiter().GetResult();
            if (currentText != oldText)
            {
                return;
            }
            Task.Delay(100).Wait(); // Wait for 100ms before checking again
        }
        throw new TimeoutException($"Text did not change from '{oldText}' within {timeoutInSeconds} seconds.");
    }

    public void WaitForElementToDisappear(IWebElementAdapter element, int timeoutInSeconds = 10)
    {
        var playwrightElement = ((PlaywrightWebElementAdapter)element)._element;
        playwrightElement.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Hidden,
            Timeout = timeoutInSeconds * 1000
        }).GetAwaiter().GetResult();
    }    

    public IWebElementAdapter FindElementByXPath(IWebElementAdapter parentElement, string xpath)
    {
        var playwrightElement = (parentElement as PlaywrightWebElementAdapter)._element;
        return new PlaywrightWebElementAdapter(playwrightElement.Locator(xpath));
    }

    public IReadOnlyCollection<IWebElementAdapter> FindElementsByXPath(IWebElementAdapter parentElement, string xpath)
    {
        var playwrightElement = (parentElement as PlaywrightWebElementAdapter)._element;
        return playwrightElement.Locator(xpath).AllAsync().GetAwaiter().GetResult()
            .Select(e => new PlaywrightWebElementAdapter(playwrightElement.Locator(xpath).Filter(new() { Has = _page.Locator($":scope:has-text('{e.TextContentAsync().GetAwaiter().GetResult()}')")})))
            .ToList();
    }

    public IWebElementAdapter FindElementByTitle(string title)
    {
        return new PlaywrightWebElementAdapter(_page.Locator($"[title='{title}']"));
    }

    public IWebElementAdapter FindElementByTitle(IWebElementAdapter parentElement, string title)
    {
        var playwrightElement = (parentElement as PlaywrightWebElementAdapter)._element;
        return new PlaywrightWebElementAdapter(playwrightElement.Locator($"[title='{title}']"));
    }

    public IReadOnlyCollection<IWebElementAdapter> FindChildElements(IWebElementAdapter parentElement, string selector)
    {
        var playwrightElement = (parentElement as PlaywrightWebElementAdapter)._element;
        return playwrightElement.Locator(selector).AllAsync().GetAwaiter().GetResult()
            .Select(e => new PlaywrightWebElementAdapter(e))
            .ToList();
            // .Select(e => new PlaywrightWebElementAdapter(playwrightElement.Locator(selector).Filter(new() { Has = _page.Locator($":scope:has-text('{e.TextContentAsync().GetAwaiter().GetResult()}')")})))
    }

    public void WaitForPageToLoad(int timeoutInSeconds = 30)
    {
        _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = timeoutInSeconds * 1000 }).GetAwaiter().GetResult();
    }

    public void WaitForElementToBeVisible(string selector, int timeoutInSeconds = 30)
    {
        _page.Locator(selector).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = timeoutInSeconds * 1000
        }).GetAwaiter().GetResult();
    }

    public void WaitForElementToBeHidden(string selector, int timeoutInSeconds = 30)
    {
        _page.Locator(selector).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Hidden,
            Timeout = timeoutInSeconds * 1000
        }).GetAwaiter().GetResult();
    }

    public void WaitForNetworkIdle(int timeoutInSeconds = 30)
    {
        _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = timeoutInSeconds * 1000 }).GetAwaiter().GetResult();
    }
}

