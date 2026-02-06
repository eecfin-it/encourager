# Coding Standards & Best Practices

## General Principles
- Write clean, maintainable, and well-documented code
- Follow language-specific conventions and best practices
- Use modern language features appropriately
- Prioritize readability and maintainability over cleverness

## .NET / C# Standards

### Code Style
- Use C# 14 features where appropriate
- Prefer minimal APIs for endpoints
- Use dependency injection for services
- Follow async/await patterns for I/O operations

### Testing
- Write unit tests for all business logic
- Use xUnit for testing framework
- Follow the test structure: Arrange, Act, Assert
- Use builders and mocks appropriately (see unit test rules)

### API Design
- Use RESTful conventions
- Return appropriate HTTP status codes
- Implement proper error handling
- Enable CORS for frontend access

## React / TypeScript Standards

### Code Style
- Use React 19 features and React Compiler optimizations
- Prefer functional components with hooks
- Use TypeScript strictly (no `any` types)
- Leverage React Compiler - avoid manual useMemo/useCallback unless necessary

### Component Structure
- Keep components small and focused
- Use composition over inheritance
- Extract reusable logic into custom hooks
- Follow single responsibility principle

### State Management
- Use `useState` for local component state
- Use Context API for shared state (e.g., LanguageContext)
- Avoid prop drilling - use context when appropriate
- Keep state as close to where it's used as possible

### Styling
- Use Tailwind CSS utility classes
- Follow mobile-first responsive design
- Use consistent spacing and sizing scales
- Reference branding colors from context/branding.md

### Performance
- Let React Compiler handle optimizations
- Use code splitting for large features
- Optimize images and assets
- Implement proper loading states

## Testing Standards

### Unit Tests
- Test behavior, not implementation
- Use descriptive test names: `[MethodName]_Should[ExpectedBehavior]_When[Condition]`
- Follow Arrange-Act-Assert pattern
- Use builders for complex objects, mocks for dependencies

### E2E Tests
- Test critical user flows
- Use Playwright for browser automation
- Keep tests independent and idempotent
- Use proper selectors (prefer data-testid or role-based)

## Documentation Standards

### Code Comments
- Write self-documenting code
- Add comments for complex logic or business rules
- Document public APIs and interfaces
- Keep comments up-to-date with code changes

### Architecture Documentation
- Maintain architecture diagrams in `/docs/architecture/`
- Use Mermaid.js for diagrams
- Update documentation when architecture changes
- Keep README.md current

## Git & Version Control

### Commit Messages
- Use clear, descriptive commit messages
- Follow conventional commit format when possible
- Reference issue numbers when applicable

### Branch Strategy
- Use feature branches for new work
- Keep main branch deployable
- Use pull requests for code review
