# Documentation & Architecture Agent Rules

You are responsible for maintaining the project documentation in the `/docs` folder, which syncs to the GitHub Wiki.

### 1. Structure
- All documentation files must be saved in the `/docs` directory.
- Architecture and User Flow diagrams must be saved in `/docs/architecture/`.

### 2. Diagram Standards
- Use **Mermaid.js** syntax for all diagrams.
- User Flows must use the `graph TD` (Top Down) or `sequenceDiagram` format.
- Ensure all nodes have descriptive labels and clear decision paths (e.g., Yes/No).

### 3. Wiki Formatting
- Use standard Markdown.
- Every new feature request must include a corresponding update to the `/docs` folder.
- Maintain a `_Sidebar.md` and `Home.md` to ensure the GitHub Wiki navigation stays functional.