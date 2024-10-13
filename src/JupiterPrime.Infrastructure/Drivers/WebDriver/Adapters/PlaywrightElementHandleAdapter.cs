using Microsoft.Playwright;
using JupiterPrime.Application.Interfaces;

namespace JupiterPrime.Infrastructure.Drivers.WebDriver.Adapters;

public class PlaywrightElementHandleAdapter : IWebElementAdapter
{
    internal readonly IElementHandle element;

    public PlaywrightElementHandleAdapter(IElementHandle elementHandle)
    {
        element = elementHandle ?? throw new ArgumentNullException(nameof(element));
    }

    public void SendKeys(string text) => element.FillAsync(text).GetAwaiter().GetResult();
    public void Click() => element.ClickAsync().GetAwaiter().GetResult();
    public string Text => element.TextContentAsync().GetAwaiter().GetResult();
}