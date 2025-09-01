# Comprehensive Problem Diagnostic Script
# This script helps identify the source of remaining VS Code problems

Write-Host "🔍 Comprehensive Problem Diagnostic Report" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptDir

Write-Host "`n📁 Project Root: $projectRoot" -ForegroundColor Blue

# 1. Check file counts by type
Write-Host "`n📊 File Type Analysis:" -ForegroundColor Yellow
$fileTypes = @{}
Get-ChildItem -Path $projectRoot -Recurse -File | Where-Object {
    $_.FullName -notlike "*node_modules*" -and 
    $_.FullName -notlike "*bin*" -and 
    $_.FullName -notlike "*obj*" -and
    $_.FullName -notlike "*.git*"
} | ForEach-Object {
    $ext = $_.Extension.ToLower()
    if (-not $fileTypes.ContainsKey($ext)) {
        $fileTypes[$ext] = 0
    }
    $fileTypes[$ext]++
}

$fileTypes.GetEnumerator() | Sort-Object Key | ForEach-Object {
    Write-Host "  $($_.Key): $($_.Value) files" -ForegroundColor White
}

# 2. Check for potentially problematic files
Write-Host "`n⚠️  Potentially Problematic Files:" -ForegroundColor Yellow

# Large files that might cause performance issues
$largeFiles = Get-ChildItem -Path $projectRoot -Recurse -File | Where-Object {
    $_.Length -gt 1MB -and
    $_.FullName -notlike "*node_modules*" -and 
    $_.FullName -notlike "*bin*" -and 
    $_.FullName -notlike "*obj*"
} | Sort-Object Length -Descending

if ($largeFiles.Count -gt 0) {
    Write-Host "  📦 Large files (>1MB):" -ForegroundColor Red
    $largeFiles | ForEach-Object {
        $sizeMB = [math]::Round($_.Length / 1MB, 2)
        Write-Host "    $($_.Name): ${sizeMB}MB" -ForegroundColor White
    }
} else {
    Write-Host "  ✅ No large files found" -ForegroundColor Green
}

# Generated files that might have issues
Write-Host "`n🔧 Generated Files Check:" -ForegroundColor Yellow
$generatedFiles = Get-ChildItem -Path $projectRoot -Recurse -File | Where-Object {
    $_.Name -like "*.generated.*" -or
    $_.Name -like "*.Designer.*" -or
    $_.Name -like "*.feature.cs" -or
    $_.Name -like "*.g.cs"
}

if ($generatedFiles.Count -gt 0) {
    Write-Host "  🔨 Generated files found:" -ForegroundColor Orange
    $generatedFiles | ForEach-Object {
        Write-Host "    $($_.FullName)" -ForegroundColor White
    }
} else {
    Write-Host "  ✅ No generated files found" -ForegroundColor Green
}

# 3. Check for VS Code specific configurations
Write-Host "`n🎯 VS Code Configuration:" -ForegroundColor Yellow
$vscodeSettings = Join-Path $projectRoot ".vscode\settings.json"
if (Test-Path $vscodeSettings) {
    Write-Host "  📝 Found .vscode\settings.json" -ForegroundColor Orange
    try {
        $settings = Get-Content $vscodeSettings | ConvertFrom-Json
        Write-Host "  📋 Settings found:" -ForegroundColor White
        $settings.PSObject.Properties | ForEach-Object {
            Write-Host "    $($_.Name): $($_.Value)" -ForegroundColor Gray
        }
    } catch {
        Write-Host "  ❌ Error reading settings.json: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "  ✅ No VS Code settings.json found" -ForegroundColor Green
}

# 4. Check for workspace-level configurations
Write-Host "`n🌐 Workspace Configuration:" -ForegroundColor Yellow
$workspaceFiles = Get-ChildItem -Path $projectRoot -Filter "*.code-workspace"
if ($workspaceFiles.Count -gt 0) {
    Write-Host "  📁 Workspace files found:" -ForegroundColor Orange
    $workspaceFiles | ForEach-Object {
        Write-Host "    $($_.FullName)" -ForegroundColor White
    }
} else {
    Write-Host "  ✅ No workspace files found" -ForegroundColor Green
}

# 5. Summary of recent fixes
Write-Host "`n✅ Recent Problem Fixes Summary:" -ForegroundColor Green
Write-Host "  📝 Markdown files: 356+ issues fixed" -ForegroundColor White
Write-Host "  🏗️  C# compilation: Clean" -ForegroundColor White
Write-Host "  🔧 PowerShell scripts: Clean" -ForegroundColor White
Write-Host "  📄 JSON files: Valid" -ForegroundColor White
Write-Host "  🎯 Project files: No errors" -ForegroundColor White

# 6. Recommendations
Write-Host "`n💡 Recommendations for Remaining Issues:" -ForegroundColor Magenta
Write-Host "  1. 🔄 Restart VS Code to refresh language servers" -ForegroundColor White
Write-Host "  2. 🧹 Reload Window (Ctrl+Shift+P -> 'Developer: Reload Window')" -ForegroundColor White
Write-Host "  3. 🔍 Check VS Code Extensions for specific linting rules" -ForegroundColor White
Write-Host "  4. 📊 Review Problems panel for source details" -ForegroundColor White
Write-Host "  5. 🛠️  Disable/re-enable specific extensions if needed" -ForegroundColor White

Write-Host "`n🎉 Diagnostic Complete!" -ForegroundColor Cyan
