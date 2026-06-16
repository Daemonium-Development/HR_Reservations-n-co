#Requires -Version 5
<#
.SYNOPSIS
  Generates a .docx copy of every in-scope Markdown doc, side by side with the .md.
.DESCRIPTION
  Scope is fixed to Documentation/Team and Documentation/Individual/Soufian Manai, 1114385
  per requests/2026-06-documents-migration.md. Requires Pandoc on PATH
  (winget install --id JohnMacFarlane.Pandoc). The original .md files are never modified or
  deleted; embedded images are resolved relative to each file's directory and embedded by
  Pandoc.
#>
$ErrorActionPreference = 'Stop'

if (-not (Get-Command pandoc -ErrorAction SilentlyContinue)) {
    throw "Pandoc not found on PATH. Install with: winget install --id JohnMacFarlane.Pandoc"
}

# Script lives in Documentation/, so resolve scope roots relative to $PSScriptRoot.
$roots = @(
    (Join-Path $PSScriptRoot 'Team'),
    (Join-Path $PSScriptRoot 'Individual\Soufian Manai, 1114385')
)

$count = 0
foreach ($root in $roots) {
    if (-not (Test-Path $root)) { throw "Scope root not found: $root" }
    Get-ChildItem -LiteralPath $root -Recurse -Filter *.md -File | ForEach-Object {
        $md = $_
        $docxName = [System.IO.Path]::GetFileNameWithoutExtension($md.Name) + '.docx'
        Push-Location -LiteralPath $md.DirectoryName
        try {
            # Run from the file's own directory so relative image paths resolve and the
            # .docx is written next to the .md. --from gfm covers GitHub tables / task lists.
            pandoc $md.Name --from gfm --resource-path . -o $docxName
            if ($LASTEXITCODE -ne 0) { throw "pandoc failed for $($md.FullName)" }
            Write-Host "converted: $($md.FullName) -> $docxName"
            $count++
        }
        finally {
            Pop-Location
        }
    }
}
Write-Host "Done. $count file(s) converted."
