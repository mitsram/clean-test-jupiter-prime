using Microsoft.Playwright;
using JupiterPrime.Application.Interfaces;
using System;

namespace JupiterPrime.Infrastructure.Drivers.WebDriver.Adapters;

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

    public IWebElementAdapter FindElement(Func<IWebElementAdapter, IWebElementAdapter> finder)
    {
        return finder(this);
    }
}
