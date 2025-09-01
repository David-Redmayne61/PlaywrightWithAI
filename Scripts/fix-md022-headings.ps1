# Advanced Markdown Fixer - Focuses on MD022 heading spacing issues
# This script specifically targets the MD022 blanks-around-headings problems

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

Write-Host "🔍 Found $($mdFiles.Count) markdown files to fix MD022 issues" -ForegroundColor Cyan

$totalIssuesFixed = 0

foreach ($file in $mdFiles) {
    Write-Host "`n📄 Processing: $($file.Name)" -ForegroundColor Blue
    
    $content = Get-Content -Path $file.FullName -Raw
    $originalContent = $content
    $issuesFixed = 0
    
    # Split into lines for easier processing
    $lines = $content -split "`r?`n"
    $newLines = @()
    
    for ($i = 0; $i -lt $lines.Length; $i++) {
        $currentLine = $lines[$i]
        $isHeading = $currentLine -match '^#{1,6}\s'
        $prevLine = if ($i -gt 0) { $lines[$i-1] } else { "" }
        $nextLine = if ($i -lt $lines.Length - 1) { $lines[$i+1] } else { "" }
        
        # Check if we need blank line BEFORE heading
        if ($isHeading -and $i -gt 0 -and $prevLine.Trim() -ne "" -and $prevLine -notmatch '^#{1,6}\s') {
            # Add blank line before heading
            $newLines += ""
            $issuesFixed++
            Write-FixInfo "Added blank line before heading: $($currentLine.Trim())"
        }
        
        # Add the current line
        $newLines += $currentLine
        
        # Check if we need blank line AFTER heading
        if ($isHeading -and $i -lt $lines.Length - 1 -and $nextLine.Trim() -ne "" -and $nextLine -notmatch '^#{1,6}\s' -and $nextLine -notmatch '^-+$' -and $nextLine -notmatch '^=+$') {
            # Add blank line after heading
            $newLines += ""
            $issuesFixed++
            Write-FixInfo "Added blank line after heading: $($currentLine.Trim())"
        }
    }
    
    # Reconstruct content
    $newContent = $newLines -join "`n"
    
    # Ensure single trailing newline
    $newContent = $newContent.TrimEnd() + "`n"
    
    # Count total changes
    if ($newContent -ne $originalContent) {
        if (-not $WhatIf) {
            Set-Content -Path $file.FullName -Value $newContent -NoNewline
            Write-Success "Fixed $issuesFixed MD022 heading spacing issues"
            $totalIssuesFixed += $issuesFixed
        } else {
            Write-Host "⚠️  Would fix $issuesFixed issues (WhatIf mode)" -ForegroundColor Magenta
        }
    } else {
        Write-Host "✨ No MD022 heading issues found" -ForegroundColor DarkGreen
    }
}

Write-Host "`n🎉 MD022 Fix Summary:" -ForegroundColor Magenta
Write-Host "📁 Files processed: $($mdFiles.Count)" -ForegroundColor White
Write-Host "🔧 Total MD022 issues fixed: $totalIssuesFixed" -ForegroundColor White

if ($WhatIf) {
    Write-Host "`n💡 Run without -WhatIf to apply changes" -ForegroundColor Yellow
} else {
    Write-Host "`n✅ All MD022 heading spacing issues have been fixed!" -ForegroundColor Green
}
