# Documentation & Architecture Rules

## Responsibility
You are responsible for maintaining project documentation in the `/docs` folder, which syncs to the GitHub Wiki.

## Structure

### Directory Organization
- All documentation files must be saved in the `/docs` directory
- Architecture and User Flow diagrams must be saved in `/docs/architecture/`
- Keep documentation organized by topic/feature

### File Naming
- Use kebab-case for file names
- Use descriptive names that indicate content
- Group related documentation together

## Diagram Standards

### Format
- Use **Mermaid.js** syntax for all diagrams
- User Flows must use the `graph TD` (Top Down) or `sequenceDiagram` format
- Ensure all nodes have descriptive labels and clear decision paths (e.g., Yes/No)

### Best Practices
- Keep diagrams simple and focused
- Use consistent styling and colors
- Include legends when necessary
- Update diagrams when architecture changes

## Wiki Formatting

### Markdown Standards
- Use standard Markdown syntax
- Follow consistent heading hierarchy
- Use code blocks for code examples
- Include links to related documentation

### Maintenance
- Every new feature request must include a corresponding update to the `/docs` folder
- Maintain a `_Sidebar.md` and `Home.md` to ensure the GitHub Wiki navigation stays functional
- Keep documentation synchronized with code changes
- Review and update documentation during code reviews

## Documentation Types

### Architecture Documentation
- System architecture diagrams
- Component relationships
- Data flow diagrams
- Deployment architecture

### User Documentation
- User guides and tutorials
- Feature documentation
- API documentation
- Troubleshooting guides

### Developer Documentation
- Setup instructions
- Development workflows
- Testing guidelines
- Contribution guidelines
