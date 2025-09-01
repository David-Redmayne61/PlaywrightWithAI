# Enhanced Markdown Fixer - Handles complex formatting issues
# This script fixes multiple markdown issues including duplicated content and spacing

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

Write-Host "🔍 Found $($mdFiles.Count) markdown files to fix" -ForegroundColor Cyan

$totalIssuesFixed = 0

foreach ($file in $mdFiles) {
    Write-Host "`n📄 Processing: $($file.Name)" -ForegroundColor Blue
    
    $content = Get-Content -Path $file.FullName -Raw
    $originalContent = $content
    $issuesFixed = 0
    
    # Fix duplicated bullet points (- - becomes -)
    $beforeDuplicates = $content
    $content = $content -replace '^\s*-\s+-\s+', '- '
    if ($content -ne $beforeDuplicates) {
        $duplicateMatches = ([regex]'^\s*-\s+-\s+').Matches($beforeDuplicates)
        $issuesFixed += $duplicateMatches.Count
        Write-FixInfo "Fixed $($duplicateMatches.Count) duplicated bullet points"
    }
    
    # Split into lines for better processing
    $lines = $content -split "`r?`n"
    $newLines = @()
    
    for ($i = 0; $i -lt $lines.Length; $i++) {
        $currentLine = $lines[$i]
        $prevLine = if ($i -gt 0) { $lines[$i-1] } else { "" }
        $nextLine = if ($i -lt $lines.Length - 1) { $lines[$i+1] } else { "" }
        
        # Check for headings
        $isHeading = $currentLine -match '^#{1,6}\s'
        $isList = $currentLine -match '^\s*[-*+]\s'
        $isCodeFence = $currentLine -match '^```'
        
        # MD022: Fix heading spacing
        if ($isHeading) {
            # Add blank line before heading if needed
            if ($i -gt 0 -and $prevLine.Trim() -ne "" -and $prevLine -notmatch '^#{1,6}\s') {
                $newLines += ""
                $issuesFixed++
                Write-FixInfo "Added blank line before heading: $($currentLine.Trim())"
            }
            
            # Add current line
            $newLines += $currentLine
            
            # Add blank line after heading if needed
            if ($i -lt $lines.Length - 1 -and $nextLine.Trim() -ne "" -and $nextLine -notmatch '^#{1,6}\s' -and $nextLine -notmatch '^-+$' -and $nextLine -notmatch '^=+$') {
                $newLines += ""
                $issuesFixed++
                Write-FixInfo "Added blank line after heading: $($currentLine.Trim())"
            }
        }
        # MD032: Fix list spacing
        elseif ($isList) {
            # Add blank line before list if needed
            if ($i -gt 0 -and $prevLine.Trim() -ne "" -and $prevLine -notmatch '^\s*[-*+]\s' -and $prevLine -notmatch '^#{1,6}\s') {
                $newLines += ""
                $issuesFixed++
                Write-FixInfo "Added blank line before list"
            }
            
            $newLines += $currentLine
            
            # Check if this is the last item in the list
            $isLastListItem = ($i -eq $lines.Length - 1) -or ($nextLine.Trim() -ne "" -and $nextLine -notmatch '^\s*[-*+]\s')
            
            # Add blank line after list if needed
            if ($isLastListItem -and $i -lt $lines.Length - 1 -and $nextLine.Trim() -ne "" -and $nextLine -notmatch '^#{1,6}\s') {
                $newLines += ""
                $issuesFixed++
                Write-FixInfo "Added blank line after list"
            }
        }
        # MD031: Fix code fence spacing
        elseif ($isCodeFence) {
            # Add blank line before code fence if needed
            if ($i -gt 0 -and $prevLine.Trim() -ne "") {
                $newLines += ""
                $issuesFixed++
                Write-FixInfo "Added blank line before code fence"
            }
            
            $newLines += $currentLine
            
            # Find the closing fence
            $closingFenceIndex = $i + 1
            while ($closingFenceIndex -lt $lines.Length -and $lines[$closingFenceIndex] -notmatch '^```') {
                $closingFenceIndex++
                $newLines += $lines[$closingFenceIndex - 1]
            }
            
            # Add the closing fence
            if ($closingFenceIndex -lt $lines.Length) {
                $newLines += $lines[$closingFenceIndex]
                $i = $closingFenceIndex
                
                # Add blank line after code fence if needed
                $nextLineAfterFence = if ($i -lt $lines.Length - 1) { $lines[$i + 1] } else { "" }
                if ($i -lt $lines.Length - 1 -and $nextLineAfterFence.Trim() -ne "") {
                    $newLines += ""
                    $issuesFixed++
                    Write-FixInfo "Added blank line after code fence"
                }
            }
        }
        else {
            $newLines += $currentLine
        }
    }
    
    # Reconstruct content
    $newContent = $newLines -join "`n"
    
    # MD040: Add language to code fences without language specification
    $beforeLangFix = $newContent
    $newContent = $newContent -replace '(?m)^```\s*$', '```text'
    if ($newContent -ne $beforeLangFix) {
        $langMatches = ([regex]'(?m)^```\s*$').Matches($beforeLangFix)
        $issuesFixed += $langMatches.Count
        Write-FixInfo "Added language specification to $($langMatches.Count) code blocks"
    }
    
    # MD036: Fix emphasis used as heading (convert **text** at start of line to heading)
    $beforeEmphasisFix = $newContent
    $newContent = $newContent -replace '(?m)^\*\*([^*]+)\*\*\s*$', '### $1'
    if ($newContent -ne $beforeEmphasisFix) {
        $emphasisMatches = ([regex]'(?m)^\*\*([^*]+)\*\*\s*$').Matches($beforeEmphasisFix)
        $issuesFixed += $emphasisMatches.Count
        Write-FixInfo "Converted $($emphasisMatches.Count) emphasis to headings"
    }
    
    # MD047: Ensure single trailing newline
    $newContent = $newContent.TrimEnd() + "`n"
    if ($newContent -ne $originalContent -and $newContent.TrimEnd() -eq $originalContent.TrimEnd()) {
        $issuesFixed++
        Write-FixInfo "Fixed trailing newline"
    }
    
    # MD012: Fix multiple consecutive blank lines
    $beforeMultipleBlanks = $newContent
    $newContent = $newContent -replace '\n\n\n+', "`n`n"
    if ($newContent -ne $beforeMultipleBlanks) {
        $blankMatches = ([regex]'\n\n\n+').Matches($beforeMultipleBlanks)
        $issuesFixed += $blankMatches.Count
        Write-FixInfo "Fixed $($blankMatches.Count) multiple consecutive blank lines"
    }
    
    # Count total changes
    if ($newContent -ne $originalContent) {
        if (-not $WhatIf) {
            Set-Content -Path $file.FullName -Value $newContent -NoNewline
            Write-Success "Fixed $issuesFixed markdown issues"
            $totalIssuesFixed += $issuesFixed
        } else {
            Write-Host "⚠️  Would fix $issuesFixed issues (WhatIf mode)" -ForegroundColor Magenta
        }
    } else {
        Write-Host "✨ No issues found" -ForegroundColor DarkGreen
    }
}

Write-Host "`n🎉 Enhanced Markdown Fix Summary:" -ForegroundColor Magenta
Write-Host "📁 Files processed: $($mdFiles.Count)" -ForegroundColor White
Write-Host "🔧 Total issues fixed: $totalIssuesFixed" -ForegroundColor White

if ($WhatIf) {
    Write-Host "`n💡 Run without -WhatIf to apply changes" -ForegroundColor Yellow
} else {
    Write-Host "`n✅ All markdown formatting issues have been fixed!" -ForegroundColor Green
}
