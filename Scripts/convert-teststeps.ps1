# Converts all .md files in the 'Test Steps' folder to both .docx and .pdf using Pandoc
# Run this script from the project root

$projectRoot = Split-Path -Parent $PSScriptRoot
$testStepsPath = Join-Path $projectRoot 'Test Steps'
$mdFiles = Get-ChildItem -Path $testStepsPath -Filter *.md
$pandocExe = 'C:\Program Files\Pandoc\pandoc.exe'

foreach ($md in $mdFiles) {
    $baseName = [System.IO.Path]::GetFileNameWithoutExtension($md.Name)
    $docxOut = Join-Path $testStepsPath ("$baseName.docx")
    $pdfOut = Join-Path $testStepsPath ("$baseName.pdf")
    
    # Create DOCX
    & $pandocExe $md.FullName -o $docxOut
    
    # Create PDF - try pdflatex first, then fallback to Chrome
    try {
        & $pandocExe $md.FullName -o $pdfOut
        Write-Host "Created $baseName.pdf using pdflatex" -ForegroundColor Green
    } catch {
        # Fallback to Chrome for PDF generation
        try {
            $htmlTemp = Join-Path $testStepsPath ("$baseName.html")
            & $pandocExe $md.FullName -o $htmlTemp
            
            $chromePath = "C:\Program Files\Google\Chrome\Application\chrome.exe"
            if (Test-Path $chromePath) {
                & $chromePath --headless --disable-gpu --print-to-pdf="$pdfOut" $htmlTemp
                Start-Sleep -Seconds 2  # Wait for Chrome to finish
                Remove-Item $htmlTemp   # Clean up HTML temp file
                Write-Host "Created $baseName.pdf using Chrome" -ForegroundColor Yellow
            } else {
                Write-Warning "PDF conversion failed for $($md.Name). Neither pdflatex nor Chrome found."
            }
        } catch {
            Write-Warning "PDF conversion failed for $($md.Name). Error: $($_.Exception.Message)"
        }
    }
}

Write-Host "Conversion complete. Check the 'Test Steps' folder for .docx and .pdf files."
