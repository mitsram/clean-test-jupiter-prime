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

    public PlaywrightWebDriverAdapter(IPage page)
    {
        _page = page ?? throw new ArgumentNullException(nameof(page));
    }

    public void NavigateToUrl(string url) => _page.GotoAsync(url).GetAwaiter().GetResult();

    public IWebElementAdapter FindElementById(string id) => 
        new PlaywrightWebElementAdapter(_page.Locator($"#{id}"));

    // public IWebElementAdapter FindElementByTestId(string testId) =>
    //     new PlaywrightWebElementAdapter(_page.Locator($"[data-testid=\"{testId}\"]"));

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
}

public class PlaywrightWebElementAdapter : IWebElementAdapter
{
    private readonly ILocator _element;

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
    private readonly IElementHandle _element;

    public PlaywrightElementHandleAdapter(IElementHandle element)
    {
        _element = element ?? throw new ArgumentNullException(nameof(element));
    }

    public void SendKeys(string text) => _element.FillAsync(text).GetAwaiter().GetResult();
    public void Click() => _element.ClickAsync().GetAwaiter().GetResult();
    public string Text => _element.TextContentAsync().GetAwaiter().GetResult();
}
