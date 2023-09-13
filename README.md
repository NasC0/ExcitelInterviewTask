# Excitel Interview Task
## Build Status
![.NET Workflow](https://github.com/NasC0/ExcitelInterviewTask/actions/workflows/dotnet.yml/badge.svg)![.NET Workflow Tests](https://gist.githubusercontent.com/NasC0/dd7742357a5806549b3fa4b060a0fd33/raw/badge.svg)

The build & test GitHub Action can be seen at: https://github.com/NasC0/ExcitelInterviewTask/actions/workflows/dotnet.yml.

## Overview
### Changes to existing code
I've added Service interfaces (`ILeadsService` & `ISubAreasService`) and I've replaced the DI registration for their concrete implementations in order to make the API Controllers testable.

In addition, testability could be further improved by:
* Implementing a ILeadsContext abstraction, in order to be able to test the EF repositories.
* Implement a thin IFileSystem abstraction in the LeadsFileDb class in order to be able to test the data layer.

For simplicity's sake, I've opted to not go ahead with those changes.

### Unit Tests
Two Unit Test projects have been added, available in the `Tests` folder:
* `Leads.WebApi.UnitTests`
* `Leads.Services.UnitTests`

### Integration Tests
One Integration test project has been added, available in the `Tests` folder: `Leads.IntegrationTests`.

Since we discussed it on the interview, the Integration tests are running on an in-memory `TestServer`.

### Tech Stack Across Projects
* `XUnit`
* `Moq` (4.20.69, post security-vulnerability fix: https://github.com/moq/moq/releases/tag/v4.20.69)
* `FluentAssertions`
* `AutoFixture`

I've *decided against* using `RestSharp` for simplicity's sake, and since adding an existing HttpClient as a base (as created by the TestServer) is not well supported in the .NET Core 2.1 version of the library.

### Disclaimer
It appears EF Core migrations in SQLite are not very well supported in .NET Core 2.1, as tracked in this issue: https://github.com/dotnet/efcore/issues/6273.

As a result, the first time you run the tests you might get a single failure in the Integration test suite. Subsequent test runs should not exhibit the same behavior and all tests should pass correctly - that's also the reason I've added a 'warmup' step in the GitHub Actions workflow.

I've decided against upgrading the Solution from .NET Core 2.1 to .NET 6, as I believe that is outside the scope of the task. I've also targeted .NET Core 2.1 in the test projects in order to keep consistency with the original code.