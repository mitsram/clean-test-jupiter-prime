using JupiterPrime.Infrastructure.Configuration;
using JupiterPrime.Infrastructure.Drivers.WebDriver;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace JupiterPrime.Tests
{
    public abstract class BaseTest : IAsyncDisposable
    {
        private WebDriverFactory _webDriverFactory;
        protected TestConfiguration TestConfig { get; private set; }

        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(TestContext.CurrentContext.TestDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            TestConfig = new TestConfiguration(configuration);
        }

        [SetUp]
        public virtual void Setup()
        {
            _webDriverFactory = new WebDriverFactory(TestConfig.WebDriverType);            
        }

        [TearDown]
        public virtual async Task TearDown()
        {
            await DisposeAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (_webDriverFactory != null)
            {
                await _webDriverFactory.DisposeAsync();
            }
        }
    }
}
