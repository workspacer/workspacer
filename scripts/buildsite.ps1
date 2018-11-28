git checkout gh-pages
git checkout master -- docs
git reset HEAD .

hugo --config docs/config.toml -s docs

remove-item -force -recurse docs

git add .
git commit -m ".\scripts\buildsite.ps1 - generated site at $(Get-Date)"