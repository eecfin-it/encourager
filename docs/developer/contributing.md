# Contributing Guide

Thank you for your interest in contributing to Encourager! This guide will help you get started.

## Getting Started

1. **Read Documentation**: Familiarize yourself with the project
   - [Getting Started](./getting-started.md)
   - [Architecture Overview](../architecture/system-architecture.md)
   - [Development Workflows](./development-workflows.md)

2. **Set Up Development Environment**: Follow [Getting Started](./getting-started.md)

3. **Find an Issue**: Check GitHub issues for tasks
   - Good first issues are labeled `good first issue`
   - Pick an issue that interests you
   - Comment on the issue to claim it

## Contribution Process

### 1. Fork and Clone

```bash
# Fork the repository on GitHub (https://github.com/mitikutesh/Encourager)
# Then clone your fork
git clone https://github.com/<your-username>/Encourager.git
cd Encourager
```

### 2. Create Branch

```bash
# Create feature branch from main
git checkout -b feature/your-feature-name

# Or bugfix branch
git checkout -b bugfix/your-bugfix-name
```

### 3. Make Changes

- Write code following project conventions
- Add tests for new features
- Update documentation
- Follow coding standards

### 4. Test Your Changes

```bash
# Backend tests
dotnet test

# Frontend linting
cd frontend && npm run lint

# Frontend build
cd frontend && npm run build

# E2E tests (if applicable)
cd e2e && npx playwright test
```

### 5. Commit Changes

Follow [Conventional Commits](https://www.conventionalcommits.org/):

```bash
git add .
git commit -m "feat: add new feature description"
```

**Commit Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `test`: Test additions/changes
- `refactor`: Code refactoring
- `style`: Code style changes (formatting)
- `chore`: Build process or auxiliary tool changes

### 6. Push and Create PR

```bash
git push origin feature/your-feature-name
```

Then create a Pull Request on GitHub.

## Code Standards

### Backend (.NET)

- **C# 14**: Use modern C# features
- **Nullable Reference Types**: Enabled, handle correctly
- **Minimal APIs**: Keep endpoints simple
- **Naming**: PascalCase for classes, camelCase for methods
- **Async/Await**: Use async/await, not `.Result` or `.Wait()`

**Example:**

```csharp
public class VerseService
{
    public VerseResult GetRandom(string lang)
    {
        // Implementation
    }
}
```

### Frontend (React/TypeScript)

- **TypeScript Strict Mode**: No `any` types
- **React 19**: Use latest React features
- **Functional Components**: Use hooks, not classes
- **Naming**: PascalCase for components, camelCase for functions
- **Styling**: Tailwind CSS utility classes

**Example:**

```typescript
export default function MyComponent() {
  const [state, setState] = useState<string>('');
  
  return <div className="text-eccfin-navy">{state}</div>;
}
```

### Testing

- **Backend**: xUnit + NSubstitute
- **Naming**: `[Method]_Should[Behavior]_When[Condition]`
- **Structure**: Arrange-Act-Assert
- **Builders**: Use Test Data Builders for domain objects

**Example:**

```csharp
[Fact]
public void GetRandom_ShouldReturnVerse_WhenLanguageIsValid()
{
    var sut = new VerseService();
    
    var actual = sut.GetRandom("en");
    
    Assert.NotNull(actual);
}
```

## Pull Request Guidelines

### PR Title

Follow conventional commits format:
```
feat: add Finnish language support
fix: correct daily blessing date comparison
docs: update API documentation
```

### PR Description

Include:

1. **What**: What changes are made
2. **Why**: Why these changes are needed
3. **How**: How the changes work
4. **Testing**: How to test the changes
5. **Screenshots**: If UI changes

**Template:**

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Documentation update
- [ ] Refactoring

## Testing
- [ ] Unit tests added/updated
- [ ] E2E tests added/updated
- [ ] Manual testing completed
- [ ] All languages tested (if applicable)

## Checklist
- [ ] Code follows project conventions
- [ ] Tests pass
- [ ] Documentation updated
- [ ] No breaking changes (or documented)
```

### Review Process

1. **Automated Checks**: CI/CD runs tests and linting
2. **Code Review**: At least one approval required
3. **Address Feedback**: Make requested changes
4. **Merge**: Squash and merge preferred

## Areas for Contribution

### Code

- **New Features**: Add requested features
- **Bug Fixes**: Fix reported bugs
- **Refactoring**: Improve code quality
- **Performance**: Optimize performance

### Documentation

- **User Guides**: Improve user documentation
- **API Docs**: Update API documentation
- **Architecture**: Update architecture diagrams
- **Examples**: Add code examples

### Testing

- **Unit Tests**: Increase test coverage
- **E2E Tests**: Add E2E test scenarios
- **Test Infrastructure**: Improve test setup

### Infrastructure

- **Deployment**: Improve deployment scripts
- **CI/CD**: Enhance GitHub Actions workflows
- **Monitoring**: Add monitoring/logging

## Reporting Issues

### Bug Reports

Include:

1. **Description**: Clear description of the bug
2. **Steps to Reproduce**: Detailed steps
3. **Expected Behavior**: What should happen
4. **Actual Behavior**: What actually happens
5. **Environment**: Browser, OS, device
6. **Screenshots**: If applicable

### Feature Requests

Include:

1. **Description**: Clear description of the feature
2. **Use Case**: Why this feature is needed
3. **Proposed Solution**: How it could work
4. **Alternatives**: Other solutions considered

## Code of Conduct

- **Be Respectful**: Treat everyone with respect
- **Be Open**: Welcome newcomers and help them
- **Be Collaborative**: Work together constructively
- **Be Professional**: Keep discussions professional

## Getting Help

- **Documentation**: Check project documentation
- **Issues**: Search existing issues
- **Discussions**: Use GitHub Discussions (if enabled)
- **Contact**: Reach out to maintainers

## Recognition

Contributors will be:

- Listed in CONTRIBUTORS.md (if file exists)
- Credited in release notes
- Appreciated by the community

## License

By contributing, you agree that your contributions will be licensed under the same license as the project.

## Questions?

If you have questions about contributing:

1. Check the documentation
2. Search existing issues
3. Open a new issue with the `question` label
4. Contact maintainers

Thank you for contributing to Encourager! 🙏
