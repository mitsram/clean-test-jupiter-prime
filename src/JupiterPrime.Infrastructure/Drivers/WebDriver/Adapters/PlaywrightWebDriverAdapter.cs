using Microsoft.Playwright;
using JupiterPrime.Application.Interfaces;
using JupiterPrime.Application.Strategies;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace JupiterPrime.Infrastructure.Drivers.WebDriver.Adapters;

public class PlaywrightWebDriverAdapter : IWebDriverAdapter, IAsyncDisposable
{
    private readonly IPage _page;
    private bool _disposed = false;

    public PlaywrightWebDriverAdapter(IPage page)
    {
        _page = page ?? throw new ArgumentNullException(nameof(page));
    }

    public async Task NavigateToUrl(string url) => await _page.GotoAsync(url);

    public StrategyElement FindElementById(string id) => 
        new StrategyElement(new PlaywrightWebElementAdapter(_page.Locator($"#{id}")));

    public StrategyElement FindElementByXPath(string xpath) =>
        new StrategyElement(new PlaywrightWebElementAdapter(_page.Locator(xpath)));

    public StrategyElement FindElementByClassName(string className) =>
        new StrategyElement(new PlaywrightWebElementAdapter(_page.Locator($".{className}")));

    public async Task<IReadOnlyCollection<StrategyElement>> FindElementsByCssSelector(string cssSelector)
    {
        var elements = await _page.QuerySelectorAllAsync(cssSelector);
        return elements.Select(e => new StrategyElement(new PlaywrightElementHandleAdapter(e))).ToList();
    }

    public async Task<IReadOnlyCollection<StrategyElement>> FindElementsByXPath(string xpath)
    {
        var elements = await _page.QuerySelectorAllAsync(xpath);
        return elements.Select(e => new StrategyElement(new PlaywrightElementHandleAdapter(e))).ToList();
    }

    public async Task<IReadOnlyCollection<StrategyElement>> FindElementsByClassName(string className)
    {
        var elements = await _page.QuerySelectorAllAsync($".{className}");
        return elements.Select(e => new StrategyElement(new PlaywrightElementHandleAdapter(e))).ToList();
    }

    public string GetCurrentUrl() => _page.Url;

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            await _page.CloseAsync();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}

public class PlaywrightWebElementAdapter : IWebElementAdapter
{
    private readonly ILocator _element;

    public PlaywrightWebElementAdapter(ILocator element)
    {
        _element = element ?? throw new ArgumentNullException(nameof(element));
    }

    public async Task SendKeys(string text) => await _element.FillAsync(text);
    public async Task Click() => await _element.ClickAsync();
    public async Task<string> GetText() => await _element.TextContentAsync();
}

public class PlaywrightElementHandleAdapter : IWebElementAdapter
{
    private readonly IElementHandle _element;

    public PlaywrightElementHandleAdapter(IElementHandle element)
    {
        _element = element ?? throw new ArgumentNullException(nameof(element));
    }

    public async Task SendKeys(string text) => await _element.FillAsync(text);
    public async Task Click() => await _element.ClickAsync();
    public async Task<string> GetText() => await _element.TextContentAsync();
}
