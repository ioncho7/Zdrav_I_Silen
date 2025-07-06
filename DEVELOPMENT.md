# Development Setup Guide

## Getting Started

### Prerequisites
- Visual Studio 2022 or Visual Studio Code
- .NET 8.0 SDK
- SQL Server or SQL Server Express (or SQL Server LocalDB)
- Git

### Initial Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Zdrav_I_Silen
   ```

2. **Configure your local database connection**
   ```bash
   # Copy the template file
   cp appsettings.Local.json.template appsettings.Local.json
   
   # Edit appsettings.Local.json with your database connection string
   # Example for SQL Server Express:
   # "Server=YOUR_COMPUTER_NAME\\SQLEXPRESS;Database=ZdravISilen;..."
   # Example for LocalDB:
   # "Server=(localdb)\\mssqllocaldb;Database=ZdravISilen;..."
   ```

3. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

## Git Workflow Best Practices

### Branch Naming Convention
- `feature/feature-name` - New features
- `bugfix/bug-description` - Bug fixes
- `hotfix/critical-fix` - Critical production fixes
- `chore/task-description` - Maintenance tasks

### Commit Message Format
```
type: brief description

Longer description if needed
- Detail 1
- Detail 2

Closes #123
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or modifying tests
- `chore`: Maintenance tasks

### Workflow Steps

1. **Before starting work**
   ```bash
   git checkout main
   git pull origin main
   git checkout -b feature/your-feature-name
   ```

2. **During development**
   ```bash
   # Make small, focused commits
   git add .
   git commit -m "feat: add user authentication"
   
   # Push regularly to backup your work
   git push origin feature/your-feature-name
   ```

3. **Before submitting PR**
   ```bash
   # Update your branch with latest main
   git checkout main
   git pull origin main
   git checkout feature/your-feature-name
   git rebase main
   
   # Push the updated branch
   git push origin feature/your-feature-name --force-with-lease
   ```

## Database Management

### Entity Framework Migrations

**Creating a new migration:**
```bash
dotnet ef migrations add MigrationName
```

**Applying migrations:**
```bash
dotnet ef database update
```

**Removing last migration (if not applied):**
```bash
dotnet ef migrations remove
```

**Important:** Always review migrations before applying them. Coordinate with team when making schema changes.

## Configuration Management

### Environment-Specific Settings

- `appsettings.json` - Default settings (committed to git)
- `appsettings.Development.json` - Development overrides (committed to git)
- `appsettings.Local.json` - Your personal local settings (NOT committed to git)
- `appsettings.Production.json` - Production settings (committed to git)

### Adding Sensitive Configuration

1. Never commit sensitive data like passwords, API keys, or connection strings
2. Use `appsettings.Local.json` for development secrets
3. Use environment variables or Azure Key Vault for production secrets

## Common Issues and Solutions

### Database Connection Issues
- Check your connection string in `appsettings.Local.json`
- Ensure SQL Server service is running
- Verify database name matches the one in migrations

### Migration Conflicts
- Communicate with team before creating migrations
- If conflicts occur, coordinate to resolve them together
- Never force push migration changes

### Git Merge Conflicts in Project Files
- `.csproj` files can have conflicts when adding packages
- Resolve by including all package references
- Test build after resolving conflicts

## Code Quality Guidelines

### Naming Conventions
- PascalCase for classes, methods, properties
- camelCase for local variables, parameters
- Meaningful and descriptive names

### File Organization
- Controllers in `/Controllers`
- Models in `/Models`
- Views in `/Views`
- Static files in `/wwwroot`

### Before Committing
1. Build the project successfully
2. Test your changes
3. Check for any debugging code or console.log statements
4. Ensure proper formatting

## Team Communication

### When to Communicate
- Before making database schema changes
- When adding new NuGet packages
- When changing shared configuration
- When working on the same files/features

### Pull Request Guidelines
- Create descriptive PR titles and descriptions
- Include screenshots for UI changes
- Tag relevant team members for review
- Link to related issues/tickets
- Ensure all checks pass before requesting review

## Troubleshooting

### Common Git Issues

**Accidentally committed to main:**
```bash
git reset HEAD~1
git checkout -b feature/your-feature
git add .
git commit -m "your commit message"
```

**Need to undo last commit:**
```bash
git reset HEAD~1  # Keeps changes
git reset --hard HEAD~1  # Discards changes
```

**Branch diverged:**
```bash
git checkout main
git pull origin main
git checkout your-branch
git rebase main
```

### Common Build Issues

**Missing packages:**
```bash
dotnet restore
```

**Database out of sync:**
```bash
dotnet ef database update
```

**Port already in use:**
- Check `launchSettings.json` for port configuration
- Kill any processes using the port
- Use different port in configuration 