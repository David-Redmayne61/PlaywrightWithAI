# Converts all .md files in the 'Test Steps' folder to both .docx and .pdf using Pandoc
# PDF generation uses Chrome headless mode for reliable conversion
# Run this script from the project root

$projectRoot = Split-Path -Parent $PSScriptRoot
$testStepsPath = Join-Path $projectRoot 'Test Steps'
$mdFiles = Get-ChildItem -Path $testStepsPath -Filter *.md
$pandocExe = 'C:\Program Files\Pandoc\pandoc.exe'
$chromePath = "C:\Program Files\Google\Chrome\Application\chrome.exe"

Write-Host "Converting markdown files to .docx and .pdf formats..." -ForegroundColor Cyan
Write-Host "Found $($mdFiles.Count) markdown files to convert" -ForegroundColor Gray

foreach ($md in $mdFiles) {
    $baseName = [System.IO.Path]::GetFileNameWithoutExtension($md.Name)
    $docxOut = Join-Path $testStepsPath ("$baseName.docx")
    $pdfOut = Join-Path $testStepsPath ("$baseName.pdf")
    $htmlTemp = Join-Path $testStepsPath ("$baseName.html")
    
    Write-Host "Processing: $($md.Name)" -ForegroundColor White
    
    # Create DOCX using Pandoc
    try {
        & $pandocExe $md.FullName -o $docxOut
        Write-Host "  ✅ Created $baseName.docx" -ForegroundColor Green
    } catch {
        Write-Warning "  ❌ DOCX conversion failed for $($md.Name). Error: $($_.Exception.Message)"
        continue
    }
    
    # Create PDF using Chrome headless (via HTML intermediate)
    try {
        # Step 1: Convert MD to HTML
        & $pandocExe $md.FullName -o $htmlTemp
        
        # Step 2: Check if Chrome is available
        if (Test-Path $chromePath) {
            # Step 3: Convert HTML to PDF using Chrome
            & $chromePath --headless --disable-gpu --print-to-pdf="$pdfOut" $htmlTemp
            Start-Sleep -Seconds 3  # Wait for Chrome to finish processing
            
            # Step 4: Clean up HTML temp file
            if (Test-Path $htmlTemp) {
                Remove-Item $htmlTemp -Force
            }
            
            # Step 5: Verify PDF was created
            if (Test-Path $pdfOut) {
                Write-Host "  ✅ Created $baseName.pdf using Chrome headless" -ForegroundColor Green
            } else {
                Write-Warning "  ❌ PDF file was not created for $($md.Name)"
            }
        } else {
            Write-Warning "  ❌ Chrome not found at: $chromePath"
            Write-Warning "  Please install Google Chrome to enable PDF conversion"
        }
    } catch {
        Write-Warning "  ❌ PDF conversion failed for $($md.Name). Error: $($_.Exception.Message)"
        # Clean up HTML temp file if it exists
        if (Test-Path $htmlTemp) {
            Remove-Item $htmlTemp -Force -ErrorAction SilentlyContinue
        }
    }
}

Write-Host ""
Write-Host "Conversion completed!" -ForegroundColor Cyan
Write-Host "📁 Check the 'Test Steps' folder for .docx and .pdf files" -ForegroundColor Gray
Write-Host ""
Write-Host "📊 Conversion Summary:" -ForegroundColor White
$docxFiles = Get-ChildItem -Path $testStepsPath -Filter *.docx
$pdfFiles = Get-ChildItem -Path $testStepsPath -Filter *.pdf
Write-Host "  • DOCX files: $($docxFiles.Count)" -ForegroundColor Green
Write-Host "  • PDF files: $($pdfFiles.Count)" -ForegroundColor Green
Write-Host "  • Method: Pandoc (DOCX) + Chrome Headless (PDF)" -ForegroundColor Gray
