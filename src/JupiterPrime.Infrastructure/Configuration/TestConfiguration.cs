using JupiterPrime.Infrastructure.Drivers.WebDriver;
using Microsoft.Extensions.Configuration;
using JupiterPrime.Application.Enums;

namespace JupiterPrime.Infrastructure.Configuration
{
    public class TestConfiguration
    {
        public WebDriverType WebDriverType { get; }
        public BrowserType BrowserType { get; }
        public string BaseUrl { get; }
        public int Timeout { get; }

        public TestConfiguration(IConfiguration configuration)
        {
            WebDriverType = configuration.GetValue<WebDriverType>("WebDriverType", WebDriverType.Selenium);
            BrowserType = configuration.GetValue<BrowserType>("BrowserType", BrowserType.Chrome);
            BaseUrl = configuration.GetValue<string>("TestSettings:BaseUrl", "http://jupiterprime-react-prod.s3-website.us-east-2.amazonaws.com/");
            Timeout = configuration.GetValue<int>("TestSettings:Timeout", 30);
        }
    }
}
