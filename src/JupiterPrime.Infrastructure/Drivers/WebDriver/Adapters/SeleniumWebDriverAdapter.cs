using OpenQA.Selenium;
using JupiterPrime.Application.Interfaces;
using JupiterPrime.Application.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JupiterPrime.Infrastructure.Drivers.WebDriver.Adapters;

public class SeleniumWebDriverAdapter : IWebDriverAdapter, IAsyncDisposable
{
    private readonly IWebDriver _driver;

    public SeleniumWebDriverAdapter(IWebDriver driver)
    {
        _driver = driver ?? throw new ArgumentNullException(nameof(driver));
    }

    public Task NavigateToUrl(string url)
    {
        _driver.Navigate().GoToUrl(url);
        return Task.CompletedTask;
    }

    public StrategyElement FindElementById(string id) => 
        new StrategyElement(new SeleniumWebElementAdapter(_driver.FindElement(By.Id(id))));

    public StrategyElement FindElementByXPath(string xpath) =>
        new StrategyElement(new SeleniumWebElementAdapter(_driver.FindElement(By.XPath(xpath))));

    public StrategyElement FindElementByClassName(string className) =>
        new StrategyElement(new SeleniumWebElementAdapter(_driver.FindElement(By.ClassName(className))));

    public Task<IReadOnlyCollection<StrategyElement>> FindElementsByCssSelector(string cssSelector)
    {
        var elements = _driver.FindElements(By.CssSelector(cssSelector))
            .Select(e => new StrategyElement(new SeleniumWebElementAdapter(e)))
            .ToList();
        return Task.FromResult<IReadOnlyCollection<StrategyElement>>(elements);
    }

    public Task<IReadOnlyCollection<StrategyElement>> FindElementsByXPath(string xpath)
    {
        var elements = _driver.FindElements(By.XPath(xpath))
            .Select(e => new StrategyElement(new SeleniumWebElementAdapter(e)))
            .ToList();
        return Task.FromResult<IReadOnlyCollection<StrategyElement>>(elements);
    }

    public Task<IReadOnlyCollection<StrategyElement>> FindElementsByClassName(string className)
    {
        var elements = _driver.FindElements(By.ClassName(className))
            .Select(e => new StrategyElement(new SeleniumWebElementAdapter(e)))
            .ToList();
        return Task.FromResult<IReadOnlyCollection<StrategyElement>>(elements);
    }

    public string GetCurrentUrl() => _driver.Url;

    public ValueTask DisposeAsync()
    {
        _driver.Quit();
        return ValueTask.CompletedTask;
    }
}

public class SeleniumWebElementAdapter : IWebElementAdapter
{
    private readonly IWebElement _element;

    public SeleniumWebElementAdapter(IWebElement element)
    {
        _element = element ?? throw new ArgumentNullException(nameof(element));
    }

    public Task SendKeys(string text)
    {
        _element.SendKeys(text);
        return Task.CompletedTask;
    }

    public Task Click()
    {
        _element.Click();
        return Task.CompletedTask;
    }

    public Task<string> GetText() => Task.FromResult(_element.Text);
}
