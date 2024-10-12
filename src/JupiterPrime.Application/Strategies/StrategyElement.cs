using JupiterPrime.Application.Interfaces;

namespace JupiterPrime.Application.Strategies;

public class StrategyElement
{
    public IWebElementAdapter WebElement { get; }

    public StrategyElement(IWebElementAdapter webElement)
    {
        WebElement = webElement;
    }
}


