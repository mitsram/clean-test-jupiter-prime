using System.Threading.Tasks;

namespace JupiterPrime.Application.Interfaces;

public interface IWebElementAdapter
{
    Task SendKeys(string text);
    Task Click();
    Task<string> GetText();
}
