$sourceCommit = (git rev-parse HEAD) | Out-String

git checkout gh-pages
git checkout $sourceCommit -- docs
git reset HEAD .

$toolsPath = "$PSScriptRoot\tools\"
$oldPath = $env:PATH

$env:PATH="$toolsPath;$oldPath"

hugo.exe --config docs/config.toml -s docs

remove-item -force -recurse docs

git add .
git commit -m ".\scripts\buildsite.ps1 - generated site at $(Get-Date)"

$env:PATH = $oldPath