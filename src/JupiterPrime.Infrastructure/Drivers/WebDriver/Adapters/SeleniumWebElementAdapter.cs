using OpenQA.Selenium;
using SeleniumElement = OpenQA.Selenium.IWebElement;
using JupiterPrime.Application.Interfaces;

namespace JupiterPrime.Infrastructure.Drivers.WebDriver.Adapters;

public class SeleniumWebElementAdapter : IWebElementAdapter
{
    internal readonly SeleniumElement _element;
    private readonly IWebDriver _driver;

    public SeleniumWebElementAdapter(SeleniumElement element)
    {
        _element = element;
    }

    public SeleniumWebElementAdapter(IWebElement element, IWebDriver driver)
    {
        _element = element;
        _driver = driver;
    }

    public void SendKeys(string text) => _element.SendKeys(text);
    public void Click() => _element.Click();
    public string Text => _element.Text;
}