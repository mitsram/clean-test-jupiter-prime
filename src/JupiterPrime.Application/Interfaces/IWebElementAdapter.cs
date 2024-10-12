using System.Threading.Tasks;

namespace JupiterPrime.Application.Interfaces;

public interface IWebElementAdapter
{
    void SendKeys(string text);
    void Click();
    string Text { get; }
}
