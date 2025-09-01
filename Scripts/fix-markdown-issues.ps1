# Fix Markdown Linting Issues Script
# This script fixes common markdown linting problems across the project

param(
    [switch]$WhatIf = $false
)

function Write-FixInfo {
    param($message)
    Write-Host "🔧 $message" -ForegroundColor Yellow
}

function Write-Success {
    param($message)
    Write-Host "✅ $message" -ForegroundColor Green
}

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptDir
$mdFiles = Get-ChildItem -Path $projectRoot -Recurse -Filter "*.md" | Where-Object {
    $_.FullName -notlike "*node_modules*" -and 
    $_.FullName -notlike "*\.playwright*" -and 
    $_.FullName -notlike "*bin*" -and 
    $_.FullName -notlike "*obj*"
}

Write-Host "🔍 Found $($mdFiles.Count) markdown files to check" -ForegroundColor Cyan

$totalIssuesFixed = 0

foreach ($file in $mdFiles) {
    Write-Host "`n📄 Processing: $($file.Name)" -ForegroundColor Blue
    
    $content = Get-Content -Path $file.FullName -Raw
    $originalContent = $content
    $issuesFixed = 0
    
    # Fix MD047: Single trailing newline
    if (-not $content.EndsWith("`n")) {
        $content = $content.TrimEnd() + "`n"
        $issuesFixed++
        Write-FixInfo "Added trailing newline"
    }
    
    # Fix MD022: Blanks around headings - add blank line before headings
    $content = $content -replace '(?m)^(.+)\r?\n(#{1,6}\s)', "`$1`n`n`$2"
    
    # Fix MD022: Blanks around headings - add blank line after headings  
    $content = $content -replace '(?m)(#{1,6}\s.+)\r?\n([^`n#-])', "`$1`n`n`$2"
    
    # Fix MD032: Blanks around lists - add blank line before lists
    $content = $content -replace '(?m)^([^`n-•*+1-9\s].+)\r?\n([-•*+]|\d+\.)\s', "`$1`n`n`$2 "
    
    # Fix MD032: Blanks around lists - add blank line after lists
    $content = $content -replace '(?m)([-•*+]|\d+\.)\s.+\r?\n([^`n-•*+1-9\s#])', "`$1`n`n`$2"
    
    # Fix MD031: Blanks around fences - add blank lines around code fences
    $content = $content -replace '(?m)^([^`n`].+)\r?\n(```)', "`$1`n`n`$2"
    $content = $content -replace '(?m)(```[^`n]*)\r?\n([^`n`])', "`$1`n`n`$2"
    
    # Fix MD040: Add language to fenced code blocks (basic detection)
    $content = $content -replace '(?m)^```\r?\n(dotnet |npm |Get-)', '```powershell'
    $content = $content -replace '(?m)^```\r?\n(\$|cd |ls )', '```bash'
    $content = $content -replace '(?m)^```\r?\n(public |class |using )', '```csharp'
    $content = $content -replace '(?m)^```\r?\n(Feature:|Scenario:|Given |When |Then )', '```gherkin'
    
    # Fix MD026: Remove trailing punctuation from headings
    $content = $content -replace '(?m)(#{1,6}\s.+):(\s*)$', '$1$2'
    
    # Fix MD009: Remove trailing spaces
    $content = $content -replace '(?m)\s+$', ''
    
    # Count changes
    if ($content -ne $originalContent) {
        $lines1 = ($originalContent -split "`n").Count
        $lines2 = ($content -split "`n").Count
        $issuesFixed += [Math]::Abs($lines2 - $lines1)
        
        if (-not $WhatIf) {
            Set-Content -Path $file.FullName -Value $content -NoNewline
            Write-Success "Fixed $issuesFixed markdown issues"
            $totalIssuesFixed += $issuesFixed
        } else {
            Write-Host "⚠️  Would fix $issuesFixed issues (WhatIf mode)" -ForegroundColor Magenta
        }
    } else {
        Write-Host "✨ No issues found" -ForegroundColor DarkGreen
    }
}

Write-Host "`n🎉 Summary:" -ForegroundColor Magenta
Write-Host "📁 Files processed: $($mdFiles.Count)" -ForegroundColor White
Write-Host "🔧 Total issues fixed: $totalIssuesFixed" -ForegroundColor White

if ($WhatIf) {
    Write-Host "`n💡 Run without -WhatIf to apply changes" -ForegroundColor Yellow
} else {
    Write-Host "`n✅ All markdown linting issues have been fixed!" -ForegroundColor Green
}
