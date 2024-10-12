using OpenQA.Selenium;
using JupiterPrime.Application.Interfaces;
using System;
using System.Threading.Tasks;
using SeleniumElement = OpenQA.Selenium.IWebElement;

namespace JupiterPrime.Infrastructure.Drivers.WebDriver.Adapters;

public class SeleniumWebDriverAdapter : IWebDriverAdapter
{
    private readonly IWebDriver _driver;

    public SeleniumWebDriverAdapter(IWebDriver driver)
    {
        _driver = driver ?? throw new ArgumentNullException(nameof(driver));
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
}

public class SeleniumWebElementAdapter : IWebElementAdapter
{
    private readonly SeleniumElement _element;

    public SeleniumWebElementAdapter(SeleniumElement element)
    {
        _element = element;
    }

    public void SendKeys(string text) => _element.SendKeys(text);
    public void Click() => _element.Click();
    public string Text => _element.Text;
}
