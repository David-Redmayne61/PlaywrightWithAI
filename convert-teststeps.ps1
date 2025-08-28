# Converts all .md files in the 'Test Steps' folder to both .docx and .pdf using Pandoc
# Run this script from the project root

$testStepsPath = Join-Path $PSScriptRoot 'Test Steps'
$mdFiles = Get-ChildItem -Path $testStepsPath -Filter *.md
$pandocExe = 'C:\Program Files\Pandoc\pandoc.exe'

foreach ($md in $mdFiles) {
    $baseName = [System.IO.Path]::GetFileNameWithoutExtension($md.Name)
    $docxOut = Join-Path $testStepsPath ("$baseName.docx")
    $pdfOut = Join-Path $testStepsPath ("$baseName.pdf")
    & $pandocExe $md.FullName -o $docxOut
    try {
        & $pandocExe $md.FullName -o $pdfOut
    } catch {
        Write-Warning "PDF conversion failed for $($md.Name). Ensure a PDF engine (like wkhtmltopdf or pdflatex) is installed."
    }
}

Write-Host "Conversion complete. Check the 'Test Steps' folder for .docx and .pdf files."
