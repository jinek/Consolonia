# Consolonia TUI Framework

Consolonia is a TUI (Text User Interface) implementation for Avalonia UI. It enables building modern, console-based applications using Avalonia's XAML markup, data binding, and declarative patterns while running entirely in the terminal.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Prerequisites and Setup
- **.NET 8.0 SDK** is required. The project targets `net8.0` framework.
- All commands should be run from the `/src` directory within the repository.
- NEVER CANCEL: Build operations may take 60+ minutes in some environments. Always set timeout values of 90+ minutes for build commands.
- NEVER CANCEL: Test operations may take 30+ minutes. Always set timeout values of 45+ minutes for test commands.

### Build and Test Commands (VALIDATED)
Bootstrap, build, and test the repository:

1. **Restore dependencies** (30s typical):
   ```bash
   cd src
   dotnet restore
   ```

2. **Build solution** - Debug configuration (43s typical):
   ```bash
   cd src
   dotnet build --no-restore
   ```
   - NEVER CANCEL: Set timeout to 90+ minutes minimum
   - Produces warnings in Experimental projects (normal)
   - Produces 1 obsolete theme warning in template (normal)

3. **Build solution** - Release configuration (27s typical):
   ```bash
   cd src
   dotnet build --no-restore --configuration release
   ```
   - NEVER CANCEL: Set timeout to 90+ minutes minimum

4. **Run tests** - Debug configuration (34s typical):
   ```bash
   cd src
   dotnet test
   ```
   - NEVER CANCEL: Set timeout to 45+ minutes minimum
   - Expected result: 148 passed, 1 skipped, 36 passed (Gallery tests)
   - Some tests may be skipped in CI environments

5. **Run tests** - Release configuration (33s typical):
   ```bash
   cd src
   dotnet test --configuration release
   ```
   - NEVER CANCEL: Set timeout to 45+ minutes minimum

### Code Quality and Formatting
**Always run formatting before committing changes:**
```bash
cd src
dotnet format
```
- Takes ~40s to complete
- May show "Unable to fix IDE1006" warnings (normal - naming rule issues require manual fixes)
- The CI build will fail if code is not properly formatted

**Check formatting without applying changes:**
```bash
cd src
dotnet format --verify-no-changes
```
- Exit code 2 indicates formatting issues exist
- Exit code 0 indicates no formatting issues

### Running Applications

#### Example Applications
**Basic Example App** (Console movie catalog):
```bash
cd src/Example
dotnet run
```
- Displays a functional TUI movie database application
- Press Ctrl+C to exit

**Consolonia Gallery** (Component showcase):
```bash
cd src/Consolonia.Gallery
dotnet run
```
- Shows all available Consolonia UI components
- Interactive demonstration of TUI capabilities
- Press Ctrl+C to exit

#### Global Tool Installation and Usage
**Install Consolonia Gallery globally** (5s):
```bash
dotnet tool install -g Consolonia.Gallery --prerelease
```

**Run globally installed Gallery**:
```bash
Consolonia.Gallery
```

**Uninstall global Gallery tool**:
```bash
dotnet tool uninstall -g Consolonia.Gallery
```

#### Project Templates
**Install project template** (4s):
```bash
dotnet new install Consolonia.Templates
```

**Create new Consolonia application**:
```bash
mkdir MyConsoloniaApp
cd MyConsoloniaApp
dotnet new consolonia.app --name MyConsoloniaApp
```

**Run template-generated application**:
```bash
dotnet run
```
- Creates a simple "Welcome to Consolonia!" application
- Contains FluentTheme deprecation warning (normal)

## Validation

### Manual Testing Requirements
**ALWAYS run through at least one complete end-to-end scenario after making changes:**

1. **Template Validation Scenario**:
   - Install templates: `dotnet new install Consolonia.Templates`
   - Create project: `dotnet new consolonia.app --name TestApp`
   - Build and run: `cd TestApp && dotnet run`
   - Verify: Application shows "Welcome to Consolonia!" message
   - Verify: UI is responsive and can be closed with Ctrl+C

2. **Gallery Validation Scenario**:
   - Build Gallery: `cd src/Consolonia.Gallery && dotnet run`
   - Verify: Left panel shows component list (Welcome, TextBlock, Button, etc.)
   - Verify: Can navigate through component list with arrow keys
   - Verify: Right panel shows selected component demonstration
   - Verify: Can interact with different UI components

3. **Example Validation Scenario**:
   - Build Example: `cd src/Example && dotnet run`
   - Verify: Movie catalog interface displays
   - Verify: Can select movies from the list
   - Verify: Movie details appear in right panel
   - Verify: Genre dropdown is functional

### Build Validation Steps
Always perform these validation steps for CI compatibility:
1. `dotnet restore` - must succeed
2. `dotnet build --configuration release` - must succeed with only expected warnings  
3. `dotnet test` - must pass with expected results (148+ passed, minimal skipped)
4. `dotnet format --verify-no-changes` - must return exit code 0

## Repository Structure

### Key Projects
- **`Consolonia.Core`** - Core TUI framework implementation
- **`Consolonia.Themes`** - Default themes and styling
- **`Consolonia.Gallery`** - Component showcase application (packaged as global tool)
- **`Example`** - Sample movie catalog application
- **`Consolonia.Templates`** - dotnet project templates  
- **`Tools/Consolonia.PreviewHost`** - Live AXAML preview tool
- **`Tests/`** - NUnit test projects
- **`Experimental/`** - Experimental features (may have warnings/errors)

### Important Files
- **`src/Consolonia.slnx`** - Main solution file
- **`src/.editorconfig`** - Code formatting rules (extensive)
- **`.github/workflows/general_build.yml`** - CI build configuration
- **`.github/workflows/editorconfig.yml`** - Code formatting CI check

## Common Tasks

### Development Workflow
1. Make changes to code
2. Build: `dotnet build`
3. Test: `dotnet test`  
4. Format: `dotnet format`
5. Validate: Run manual testing scenarios
6. Commit changes

### Package Management
**Create NuGet packages**:
```bash
cd src
dotnet pack --configuration release -o ./packages --version-suffix beta.test
```
- Creates packages in `src/packages/` directory
- Includes tools: `Consolonia.Gallery`, `Consolonia.PreviewHost`

### Common Issues and Solutions
- **Build warnings in Experimental projects**: Normal, these are development projects
- **FluentTheme obsolete warnings**: Normal, templates will be updated in future releases
- **Formatting failures**: Run `dotnet format` before committing
- **Test skips in CI**: Expected behavior, some tests require specific environments
- **Long build times**: Normal, set appropriate timeouts and never cancel operations

## Framework Information
- **Base Framework**: Built on Avalonia UI 11.3.4
- **Target Platform**: .NET 8.0
- **UI Technology**: Console-based TUI using XAML markup
- **Cross-Platform**: Windows, macOS, Linux support
- **Architecture**: Uses Avalonia's rendering pipeline optimized for text output

## CI/CD Information
- **Build timeout**: 30 minutes (may need adjustment for complex builds)
- **Test framework**: NUnit with custom Consolonia test base classes
- **Code analysis**: Uses JetBrains ReSharper for style checking
- **Formatting**: EditorConfig + dotnet format integration
- **Publishing**: Automated NuGet package publishing to nuget.org on main branch

Always build and exercise your changes using the validation scenarios above before committing.