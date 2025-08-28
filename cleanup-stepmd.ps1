# Removes stray step .md files from the project root (if present)
# Keeps only the versions in the 'Test Steps' folder

Remove-Item .\Test1_Steps.md, .\Test2_Steps.md -ErrorAction SilentlyContinue
Write-Host "Cleanup complete: stray .md files removed from project root if they existed."
