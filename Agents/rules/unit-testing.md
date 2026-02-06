# Unit Testing Rules

## Test Structure

### Naming Convention
Format: `[MethodName]_Should[ExpectedBehavior]_When[Condition]`

Example: `ProcessPayment_ShouldMarkOrderAsPaid_AndNotify_WhenPaymentSucceeds`

### Test Organization
1. **Arrange** - Set up test data and dependencies (no blank lines between arrange statements)
2. **Act** - Invoke the method under test
3. **Assert** - Verify the outcome

## Test Data Builders

### When to Use Builders
- **Mandatory** for domain objects/DTOs with multiple fields
- **Optional** for simple objects (can use inline)
- **Always** use for maintainability when object has 3+ properties

### Builder Pattern
```csharp
var verse = new VerseBuilder()
    .WithText("For God so loved the world")
    .WithReference("John 3:16")
    .Build();
```

## Mocking Strategy

### NSubstitute Usage
- Use `Substitute.For<T>()` for interfaces
- Use `Substitute.For<T>()` for dependencies that need behavior verification
- Only mock what's necessary for the test

### Mock Setup
```csharp
var mockService = Substitute.For<IVerseService>();
mockService.GetRandomVerse(Arg.Any<string>()).Returns(expectedVerse);
```

## Stub Builders (Optional)

### When to Use
- Only if interface has complex, reusable behavior
- When it avoids repetitive mock setup
- For scenarios that are used across multiple tests

## Test Implementation Template

```csharp
[Fact]
public void [METHOD]_Should[EXPECTED_BEHAVIOR]_When[CONDITION]()
{
    var mockInterface = Substitute.For<IInterfaceType>();
    mockInterface.Method(Arg.Any<type>()).Returns(value);

    var objectParam = new ObjectBuilder()
        .WithProperty(value)
        .Build();

    var sut = new ClassUnderTest(dependencies);

    var actual = sut.MethodUnderTest(params);

    Assert.Condition(actual);
    Assert.Condition(objectParam.Property);
    mockInterface.Received(1).Method(args);
}
```

## Decision Table

| Object Type | Tool | Notes |
|------------|------|-------|
| Domain object/DTO with multiple fields | Test Data Builder | Always use for maintainability |
| Interface/dependency | NSubstitute mock | Default choice |
| Interface with complex reusable behavior | Stub Builder (optional) | Only if it avoids repetitive setup |
| Simple object/DTO | Inline | Builder unnecessary |
| Service under test | sut | Always |

## Important Rules

- **No comments** in unit tests
- **No blank lines** between Arrange code statements
- Only blank line should be between Arrange, Act, and Assert sections
- Focus on **behavior**, not internal implementation
- Always assign result to `actual` variable
- Use descriptive assertions
