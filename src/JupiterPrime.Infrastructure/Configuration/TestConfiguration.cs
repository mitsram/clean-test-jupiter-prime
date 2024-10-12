using JupiterPrime.Infrastructure.Drivers.WebDriver;
using Microsoft.Extensions.Configuration;

namespace JupiterPrime.Infrastructure.Configuration
{
    public class TestConfiguration
    {
        public WebDriverType WebDriverType { get; }
        public string BaseUrl { get; }
        public int Timeout { get; }

        public TestConfiguration(IConfiguration configuration)
        {
            WebDriverType = configuration.GetValue<WebDriverType>("WebDriverType", WebDriverType.Selenium);
            BaseUrl = configuration.GetValue<string>("TestSettings:BaseUrl", "http://jupiterprime-react-prod.s3-website.us-east-2.amazonaws.com/");
            Timeout = configuration.GetValue<int>("TestSettings:Timeout", 30);
        }
    }
}

