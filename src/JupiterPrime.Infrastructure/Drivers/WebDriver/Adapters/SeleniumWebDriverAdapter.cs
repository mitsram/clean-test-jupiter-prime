using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using JupiterPrime.Application.Interfaces;
using System;
using System.Threading.Tasks;
using SeleniumElement = OpenQA.Selenium.IWebElement;

namespace JupiterPrime.Infrastructure.Drivers.WebDriver.Adapters;

public class SeleniumWebDriverAdapter : IWebDriverAdapter
{
    private readonly IWebDriver _driver;
    private readonly int _defaultTimeoutMs;

    public SeleniumWebDriverAdapter(IWebDriver driver)
    {
        _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        _defaultTimeoutMs = 30 * 5000;
    }

    public void NavigateToUrl(string url) => _driver.Navigate().GoToUrl(url);

    public IWebElementAdapter FindElementById(string id) => 
        new SeleniumWebElementAdapter(_driver.FindElement(By.Id(id)));

    public IWebElementAdapter FindElementByTestId(string testId) =>
        new SeleniumWebElementAdapter(_driver.FindElement(By.CssSelector($"[data-testid=\"{testId}\"]")));

    public IWebElementAdapter FindElementByXPath(string xpath) =>
        new SeleniumWebElementAdapter(_driver.FindElement(By.XPath(xpath)));

    public IWebElementAdapter FindElementByClassName(string className) =>
        new SeleniumWebElementAdapter(_driver.FindElement(By.ClassName(className)));

    public IReadOnlyCollection<IWebElementAdapter> FindElementsByCssSelector(string cssSelector) =>
        _driver.FindElements(By.CssSelector(cssSelector))
            .Select(e => new SeleniumWebElementAdapter(e))
            .ToList();

    public IReadOnlyCollection<IWebElementAdapter> FindElementsByXPath(string xpath) =>
        _driver.FindElements(By.XPath(xpath))
            .Select(e => new SeleniumWebElementAdapter(e))
            .ToList();

    public IReadOnlyCollection<IWebElementAdapter> FindElementsByClassName(string className) =>
        _driver.FindElements(By.ClassName(className))
            .Select(e => new SeleniumWebElementAdapter(e))
            .ToList();

    public string GetCurrentUrl() => _driver.Url;

    public void Dispose() => _driver.Quit();

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }

    public IWebElementAdapter FindElementByDataLocator(string dataLocator)
    {
        return new SeleniumWebElementAdapter(_driver.FindElement(By.CssSelector($"[data-locator='{dataLocator}']")));
    }

    public IWebElementAdapter FindElementByDataLocator(IWebElementAdapter parentElement, string dataLocator)
    {
        var seleniumElement = (parentElement as SeleniumWebElementAdapter)._element;
        return new SeleniumWebElementAdapter(seleniumElement.FindElement(By.CssSelector($"[data-locator='{dataLocator}']")));
    }

    public IReadOnlyCollection<IWebElementAdapter> FindElementsByTestId(string testId)
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromMilliseconds(_defaultTimeoutMs));
        
        // Wait for at least one element with the given test ID to be visible
        wait.Until(driver => driver.FindElements(By.CssSelector($"[data-testid='{testId}']")).Any(e => e.Displayed));

        // Find all elements with the given test ID
        var elements = _driver.FindElements(By.CssSelector($"[data-testid='{testId}']"));

        return elements.Select(e => new SeleniumWebElementAdapter(e)).ToList();
    }

    // public IReadOnlyCollection<IWebElementAdapter> FindElementsByTestId(string testId) =>
    //     _driver.FindElements(By.CssSelector($"[data-testid='{testId}']"))
    //         .Select(e => new SeleniumWebElementAdapter(e))
    //         .ToList();

    public IWebElementAdapter FindElementByTestId(IWebElementAdapter parentElement, string testId)
    {
        var seleniumElement = (parentElement as SeleniumWebElementAdapter)._element;
        return new SeleniumWebElementAdapter(seleniumElement.FindElement(By.CssSelector($"[data-testid='{testId}']")));
    }

    public IReadOnlyCollection<IWebElementAdapter> FindElementsByDataLocator(string dataLocator)
    {
        return _driver.FindElements(By.CssSelector($"[data-locator='{dataLocator}']"))
            .Select(e => new SeleniumWebElementAdapter(e))
            .ToList();
    }
}

public class SeleniumWebElementAdapter : IWebElementAdapter
{
    internal readonly SeleniumElement _element;

    public SeleniumWebElementAdapter(SeleniumElement element)
    {
        _element = element;
    }

    public void SendKeys(string text) => _element.SendKeys(text);
    public void Click() => _element.Click();
    public string Text => _element.Text;
}
