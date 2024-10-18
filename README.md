# clean-test-jupiter-prime

This project is an automation testing framework implementing clean architecture principles and utilizing SpecFlow for behavior-driven development (BDD).

## Table of Contents

1. [Project Structure](#project-structure)
2. [Technologies Used](#technologies-used)
3. [Getting Started](#getting-started)
4. [Running Tests](#running-tests)
5. [Writing Tests](#writing-tests)
6. [Reporting](#reporting)
7. [Contributing](#contributing)
8. [License](#license)

## Project Structure

The solution is organized into the following projects:

- `JupiterPrime.Domain`: Contains domain entities and interfaces
- `JupiterPrime.Infrastructure`: Implements services and drivers
- `JupiterPrime.Application`: Contains business logic and use cases
- `JupiterPrime.Tests`: Contains NUnit-based tests for both UI and API.
- `JupiterPrime.Specflow`: SpecFlow-based BDD tests

## Technologies Used

- .NET 8.0
- NUnit
- SpecFlow
- Selenium WebDriver
- Playwright
- Microsoft.Playwright.NUnit

## Getting Started

1. Clone the repository:
   ```
   git clone git@github.com:mitsram/clean-test-jupiter-prime.git
   ```

2. Install the required .NET SDK (version 8.0 or later).

3. Restore NuGet packages:
   ```
   dotnet restore
   ```

4. Install Playwright browsers:
   ```
   pwsh bin/Debug/net8.0/playwright.ps1 install
   ```

## Running Tests

To run all tests:

```
dotnet test
```

To run specific test projects:

```
dotnet test tests/JupiterPrime.Tests
dotnet test tests/JupiterPrime.Specflow
```


### Configuration

The `appsettings.json` file in the `JupiterPrime.Tests` project allows you to configure:

- WebDriver type (Selenium or Playwright)
- Browser type
- Base URL for tests
- Default timeout

### Driver Initialization

The framework will initialize the appropriate driver through `WebDriverFactory` with driver and browser type from `TestConfiguration`:


```csharp:tests/JupiterPrime.Tests/BaseTest.cs
[SetUp]
public virtual async Task SetupAsync()
{
    webDriverFactory = new WebDriverFactory(TestConfig.WebDriverType, TestConfig.BrowserType);            
}
```

### Usage in Tests

When writing tests, use the `driver` field to interact with the browser. This abstraction allows your tests to work with either Playwright or Selenium without modification:

```csharp:tests/JupiterPrime.Tests/SampleTest.cs
[Test]
public async Task SampleTest()
{
    await driver.NavigateToAsync("https://www.example.com");
    // Perform test actions using the driver
}
```

### Teardown

The framework handles the proper disposal of resources for both Playwright and Selenium in the `TearDown` method:

```csharp:tests/JupiterPrime.Tests/BaseTest.cs
[TearDown]
public virtual async Task TearDown()
{
    await DisposeAsync();
}

public async ValueTask DisposeAsync()
{
    if (webDriverFactory != null)
    {
        await webDriverFactory.DisposeAsync();
    }
}
```

By following this approach, you can easily switch between Playwright and Selenium while keeping most of your test code unchanged. The framework handles the initialization and teardown processes automatically based on the selected driver type.

## Writing Tests

### NUnit Tests

Add new test classes to the `JupiterPrime.Tests` project. Example:

```csharp
[TestFixture]
public class LoginTests : BaseTest
{
    [Test]
    public void Should_LoginSuccessfully_WhenCredentialsAreValid()
    {
        // Arrange
        // Act
        // Assert
    }
}
```

### BDD Tests (SpecFlow)

1. Add new feature files to the `JupiterPrime.Specflow/Features` directory.
2. Implement step definitions in the `JupiterPrime.Specflow/StepDefinitions` directory.

Example Feature:

```gherkin
Feature: Login
    As a user of the JupiterPrime website
    I want to be able to log in
    So that I can access the inventory page

Scenario: Successful login with valid credentials
    Given I am on the login page
    When I enter valid credentials
    Then I should be logged in successfully
```

## Reporting

The framework generates Playwright HTML reports after test execution. You can find these reports in the `playwright-report` directory.

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.