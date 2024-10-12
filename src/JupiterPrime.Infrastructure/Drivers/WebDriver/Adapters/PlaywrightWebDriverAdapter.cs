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
        return new PlaywrightWebElementAdapter(_page.Locator($"[data-locator='{dataLocator}']"));
    }

    public IWebElementAdapter FindElementByDataLocator(IWebElementAdapter parentElement, string dataLocator)
    {
        var playwrightElement = (parentElement as PlaywrightWebElementAdapter)._element;
        return new PlaywrightWebElementAdapter(playwrightElement.Locator($"[data-locator='{dataLocator}']"));
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
}

public class PlaywrightWebElementAdapter : IWebElementAdapter
{
    internal readonly ILocator _element;

    public PlaywrightWebElementAdapter(ILocator element)
    {
        _element = element ?? throw new ArgumentNullException(nameof(element));
    }

    public void SendKeys(string text) => _element.FillAsync(text).GetAwaiter().GetResult();
    public void Click() => _element.ClickAsync().GetAwaiter().GetResult();
    public string Text => _element.TextContentAsync().GetAwaiter().GetResult();
}

public class PlaywrightElementHandleAdapter : IWebElementAdapter
{
    internal readonly IElementHandle _element;

    public PlaywrightElementHandleAdapter(IElementHandle element)
    {
        _element = element ?? throw new ArgumentNullException(nameof(element));
    }

    public void SendKeys(string text) => _element.FillAsync(text).GetAwaiter().GetResult();
    public void Click() => _element.ClickAsync().GetAwaiter().GetResult();
    public string Text => _element.TextContentAsync().GetAwaiter().GetResult();
}
