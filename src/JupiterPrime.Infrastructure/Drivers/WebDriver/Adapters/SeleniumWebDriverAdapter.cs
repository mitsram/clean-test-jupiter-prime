using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using JupiterPrime.Application.Interfaces;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using SeleniumExtras;

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

    public string GetCurrentUrl() => _driver.Url;

    public void NavigateToUrl(string url) => _driver.Navigate().GoToUrl(url);

    public IWebElementAdapter FindElementById(string id) => 
        new SeleniumWebElementAdapter(_driver.FindElement(By.Id(id)));

    public IWebElementAdapter FindElementByClassName(string className) =>
        new SeleniumWebElementAdapter(_driver.FindElement(By.ClassName(className)));    

    public IWebElementAdapter FindElementByTestId(string testId) =>
        new SeleniumWebElementAdapter(_driver.FindElement(By.CssSelector($"[data-testid=\"{testId}\"]")));
    
    public IWebElementAdapter FindElementByTestId(IWebElementAdapter parentElement, string testId)
    {
        var seleniumElement = (parentElement as SeleniumWebElementAdapter)._element;
        return new SeleniumWebElementAdapter(seleniumElement.FindElement(By.CssSelector($"[data-testid='{testId}']")));
    }

    public IWebElementAdapter FindElementByXPath(string xpath) =>
        new SeleniumWebElementAdapter(_driver.FindElement(By.XPath(xpath)));

    public IWebElementAdapter FindElementByXPath(IWebElementAdapter parentElement, string xpath)
    {
        var seleniumElement = (parentElement as SeleniumWebElementAdapter)._element;
        return new SeleniumWebElementAdapter(seleniumElement.FindElement(By.XPath(xpath)));
    }

    public IWebElementAdapter FindElementByTitle(string title)
    {
        return new SeleniumWebElementAdapter(_driver.FindElement(By.XPath($"//*[@title='{title}']")));
    }

    public IWebElementAdapter FindElementByTitle(IWebElementAdapter parentElement, string title)
    {
        var seleniumElement = (parentElement as SeleniumWebElementAdapter)._element;
        return new SeleniumWebElementAdapter(seleniumElement.FindElement(By.XPath($".//*[@title='{title}']")));
    }

    public IWebElementAdapter FindElementByDataLocator(string dataLocator)
    {
        return new SeleniumWebElementAdapter(_driver.FindElement(By.CssSelector($"[data-locator='{dataLocator}']")));
    }

    public IWebElementAdapter FindElementByDataLocator(IWebElementAdapter parentElement, string dataLocator)
    {
        var seleniumElement = (parentElement as SeleniumWebElementAdapter)._element;
        try
        {
            var element = seleniumElement.FindElement(By.CssSelector($"[data-locator='{dataLocator}']"));
            return new SeleniumWebElementAdapter(element);
        }
        catch (NoSuchElementException)
        {
            Console.WriteLine($"Element with data-locator '{dataLocator}' not found");
            return null;
        }
    }

    public IReadOnlyCollection<IWebElementAdapter> FindElementsByClassName(string className) =>
        _driver.FindElements(By.ClassName(className))
            .Select(e => new SeleniumWebElementAdapter(e))
            .ToList();

    public IReadOnlyCollection<IWebElementAdapter> FindElementsByCssSelector(string cssSelector) =>
        _driver.FindElements(By.CssSelector(cssSelector))
            .Select(e => new SeleniumWebElementAdapter(e))
            .ToList();

    public IReadOnlyCollection<IWebElementAdapter> FindElementsByTestId(string testId)
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromMilliseconds(_defaultTimeoutMs));        
        // Wait for at least one element with the given test ID to be interactable
        wait.Until(driver => {
            var elements = driver.FindElements(By.CssSelector($"[data-testid='{testId}']"));
            return elements.Any(e => IsElementInteractable(e));
        });        
        var allElements = _driver.FindElements(By.CssSelector($"[data-testid='{testId}']"));
        // Filter out elements that are not interactable
        var interactableElements = allElements.Where(IsElementInteractable).ToList();
        return interactableElements.Select(e => new SeleniumWebElementAdapter(e, _driver)).ToList();
    }

    public IReadOnlyCollection<IWebElementAdapter> FindElementsByXPath(string xpath) =>
        _driver.FindElements(By.XPath(xpath))
            .Select(e => new SeleniumWebElementAdapter(e))
            .ToList();

    public IReadOnlyCollection<IWebElementAdapter> FindElementsByXPath(IWebElementAdapter parentElement, string xpath)
    {
        var seleniumElement = (parentElement as SeleniumWebElementAdapter)._element;
        return seleniumElement.FindElements(By.XPath(xpath))
            .Select(e => new SeleniumWebElementAdapter(e))
            .ToList();
    }

    public IReadOnlyCollection<IWebElementAdapter> FindElementsByDataLocator(string dataLocator)
    {
        return _driver.FindElements(By.CssSelector($"[data-locator='{dataLocator}']"))
            .Select(e => new SeleniumWebElementAdapter(e))
            .ToList();
    }

    public IReadOnlyCollection<IWebElementAdapter> FindChildElements(IWebElementAdapter parentElement, string selector)
    {
        var seleniumElement = (parentElement as SeleniumWebElementAdapter)._element;
        return seleniumElement.FindElements(By.CssSelector(selector))
            .Select(e => new SeleniumWebElementAdapter(e))
            .ToList();
    }  

    public void WaitForTextToChange(IWebElementAdapter element, string oldText, int timeoutInSeconds = 10)
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.Until(d => ((SeleniumWebElementAdapter)element)._element.Text != oldText);
    }

    public void WaitForElementToDisappear(IWebElementAdapter element, int timeoutInSeconds = 10)
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.Until(d => {
            try
            {
                return !((SeleniumWebElementAdapter)element)._element.Displayed;
            }
            catch (StaleElementReferenceException)
            {
                return true; // Element is no longer in the DOM
            }
        });
    }    

    public void WaitForPageToLoad(int timeoutInSeconds = 30)
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
    }

    public void WaitForElementToBeVisible(string selector, int timeoutInSeconds = 30)
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(selector)));
    }

    public void WaitForElementToBeHidden(string selector, int timeoutInSeconds = 30)
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(selector)));
    }

    public void WaitForNetworkIdle(int timeoutInSeconds = 30)
    {
        // Selenium doesn't have a built-in method to wait for network idle
        // We can approximate this by waiting for the document.readyState to be "complete"
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutInSeconds));
        wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
    }

    private bool IsElementInViewport(IWebElement element)
    {
        var js = (IJavaScriptExecutor)_driver;
        return (bool)js.ExecuteScript(@"
            var rect = arguments[0].getBoundingClientRect();
            return (
                rect.top >= 0 &&
                rect.left >= 0 &&
                rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
                rect.right <= (window.innerWidth || document.documentElement.clientWidth)
            );
        ", element);
    }

    private bool IsElementInteractable(IWebElement element)
    {
        try
        {
            return element.Displayed && element.Enabled && IsElementInViewport(element);
        }
        catch (StaleElementReferenceException)
        {
            return false;
        }
    } 

    public void Dispose() => _driver.Quit();

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}
