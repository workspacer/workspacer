$sourceCommit = (git rev-parse HEAD) | Out-String
$sourceCommit = $sourceCommit -replace "`n","" -replace "`r",""
$sourceCommit = $sourceCommit.Trim()

git checkout gh-pages
git checkout $sourceCommit -- docs
git reset HEAD .

$toolsPath = "$PSScriptRoot\tools\hugo\"
$oldPath = $env:PATH

$env:PATH="$toolsPath;$oldPath"

hugo --config docs/config.toml -s docs

remove-item -force -recurse docs

git add .
git commit -m ".\scripts\buildsite.ps1 - generated site at $(Get-Date)"

$env:PATH = $oldPath