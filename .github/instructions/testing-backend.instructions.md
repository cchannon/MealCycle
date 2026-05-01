---
description: "Use when writing xUnit unit tests, integration tests, or test helpers for C# backend code. Covers test structure, naming, mocking, assertions, and integration test setup with Azure Storage Table persistence."
applyTo: "backend/**/Tests/**, backend/**/*.Tests/**, backend/**/*Tests.cs, backend/**/*Spec.cs"
---

## C# Testing Standards (xUnit)

### Structure
- Mirror the source project structure under `tests/Unit/` and `tests/Integration/`
- One test class per subject class: `OrderServiceTests` tests `OrderService`
- Group related tests with `[Trait("Category", "...")]` for selective runs

### Naming
- Test class: `<Subject>Tests`
- Test method: `<Method>_<Scenario>_<ExpectedOutcome>`
  - e.g., `CreateMealCycle_WhenValidationFails_ReturnsBadRequest`
  - e.g., `GetMealCycle_WhenRecordMissing_ReturnsNotFound`

### Assertions
- FluentAssertions throughout — no bare `Assert.Equal`
- Chain multiple assertions on the same subject: `result.Should().NotBeNull().And.HaveCount(3)`
- For exceptions: `act.Should().ThrowAsync<NotFoundException>().WithMessage("*order*")`

### Mocking
- Moq for mocking interfaces — mock only what the subject directly depends on
- Use `Mock<T>.Setup()` for specific call setups; `.Verifiable()` + `mock.VerifyAll()` when confirming side effects
- Avoid mocking concrete classes — if you need to, it signals a missing interface abstraction

### Unit Tests
- Arrange / Act / Assert pattern with blank lines separating sections
- No I/O, no network, no database — test pure logic with mocked dependencies
- Use `AutoFixture` (or `Bogus`) for generating realistic test data; avoid magic strings like `"test"` or `"foo"`

### Integration Tests
- Run against Azurite with Table service enabled for repository/integration tests
- Use `WebApplicationFactory<Program>` for API integration tests
- Each test class that touches storage starts with isolated table names or a clean test partition
- Seed test data explicitly in the test — never depend on pre-existing data state

### Azure Service Fakes
- Use `Azurite` for Blob/Table Storage integration tests
- For Service Bus: use `ServiceBusAdministrationClient` to create temporary test queues; clean up in `IAsyncLifetime.DisposeAsync()`

### Coverage
- Aim for full coverage on `Application/` and `Domain/` layers
- Infrastructure/repository layer: integration tests only — don't mock Azure Tables SDK internals
- Controllers: covered via API integration tests, not isolated unit tests
