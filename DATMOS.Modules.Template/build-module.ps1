# DATMOS Module Build Script
# Use this script to build and test modules

param(
    [string]$ModuleName = "Template",
    [switch]$Clean,
    [switch]$Test,
    [switch]$Pack,
    [switch]$Help
)

if ($Help) {
    Write-Host "DATMOS Module Build Script"
    Write-Host "Usage: .\build-module.ps1 [options]"
    Write-Host ""
    Write-Host "Options:"
    Write-Host "  -ModuleName <name>    Module name (default: Template)"
    Write-Host "  -Clean                Clean build artifacts"
    Write-Host "  -Test                 Run tests"
    Write-Host "  -Pack                 Create NuGet package"
    Write-Host "  -Help                 Show this help message"
    Write-Host ""
    exit 0
}

$ProjectPath = "DATMOS.Modules.$ModuleName\DATMOS.Modules.$ModuleName.csproj"
$OutputPath = "bin\Release\DATMOS.Modules.$ModuleName"
$PackagePath = "packages"

# Colors for output
$SuccessColor = "Green"
$ErrorColor = "Red"
$InfoColor = "Cyan"
$WarningColor = "Yellow"

function Write-Success {
    param([string]$Message)
    Write-Host "✓ $Message" -ForegroundColor $SuccessColor
}

function Write-Error {
    param([string]$Message)
    Write-Host "✗ $Message" -ForegroundColor $ErrorColor
}

function Write-Info {
    param([string]$Message)
    Write-Host "ℹ $Message" -ForegroundColor $InfoColor
}

function Write-Warning {
    param([string]$Message)
    Write-Host "⚠ $Message" -ForegroundColor $WarningColor
}

# Check if project exists
if (-not (Test-Path $ProjectPath)) {
    Write-Error "Project not found: $ProjectPath"
    Write-Info "Available modules:"
    Get-ChildItem -Directory -Filter "DATMOS.Modules.*" | ForEach-Object {
        Write-Host "  $($_.Name)"
    }
    exit 1
}

Write-Info "Building DATMOS.Modules.$ModuleName"

# Clean if requested
if ($Clean) {
    Write-Info "Cleaning build artifacts..."
    dotnet clean $ProjectPath --configuration Release
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Clean failed"
        exit $LASTEXITCODE
    }
    Write-Success "Clean completed"
}

# Restore packages
Write-Info "Restoring packages..."
dotnet restore $ProjectPath
if ($LASTEXITCODE -ne 0) {
    Write-Error "Restore failed"
    exit $LASTEXITCODE
}
Write-Success "Restore completed"

# Build project
Write-Info "Building project..."
dotnet build $ProjectPath --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit $LASTEXITCODE
}
Write-Success "Build completed"

# Run tests if requested
if ($Test) {
    $TestProject = "DATMOS.Modules.$ModuleName.Tests\DATMOS.Modules.$ModuleName.Tests.csproj"
    if (Test-Path $TestProject) {
        Write-Info "Running tests..."
        dotnet test $TestProject --configuration Release --no-build
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Tests failed"
            exit $LASTEXITCODE
        }
        Write-Success "Tests completed"
    } else {
        Write-Warning "Test project not found: $TestProject"
    }
}

# Create package if requested
if ($Pack) {
    Write-Info "Creating NuGet package..."
    
    # Create packages directory if it doesn't exist
    if (-not (Test-Path $PackagePath)) {
        New-Item -ItemType Directory -Path $PackagePath -Force | Out-Null
    }
    
    dotnet pack $ProjectPath --configuration Release --no-build --output $PackagePath
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Pack failed"
        exit $LASTEXITCODE
    }
    
    # List created packages
    $packages = Get-ChildItem "$PackagePath\*.nupkg" | Sort-Object LastWriteTime -Descending
    if ($packages.Count -gt 0) {
        Write-Success "Package created:"
        foreach ($package in $packages) {
            Write-Host "  $($package.Name)" -ForegroundColor $SuccessColor
        }
    } else {
        Write-Warning "No packages were created"
    }
}

# Summary
Write-Success "Build process completed for DATMOS.Modules.$ModuleName"
Write-Host ""
Write-Host "Next steps:" -ForegroundColor $InfoColor
Write-Host "  1. Add module reference to DATMOS.Web.csproj"
Write-Host "  2. Register module in Program.cs"
Write-Host "  3. Test module integration"
Write-Host "  4. Deploy module"
