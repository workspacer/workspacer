 param (
    [string]$version,
    [switch]$yes,
    [switch]$nogit
 )


if (!$yes) {
    $answer = Read-Host "this will update the version and create/push a git tag for $version (y/n)"
    if ($answer -ne 'y') {
        exit
    }
}

$infos = Get-ChildItem -Path . -Filter "AssemblyInfo.cs" -Recurse -ErrorAction SilentlyContinue -Force
foreach ($file in $infos)
{
    "setting version for $file to $version"
    (Get-Content $file.PSPath) |
    Foreach-Object { $_ -replace "AssemblyVersion\("".*""\)", "AssemblyVersion(""$version"")" } |
    Set-Content $file.PSPath
}

$setupProjs = Get-ChildItem -Path . -Filter "Product.wxs" -Recurse -ErrorAction SilentlyContinue -Force
foreach ($file in $setupProjs)
{
    "setting version for $file to $version"
    (Get-Content $file.PSPath) |
    Foreach-Object { $_ -replace "Version="".*"" Manu", "Version=""$version"" Manu" } |
    Set-Content $file.PSPath
}

"setting version for VERSION to $version"
"$version" | Set-Content "VERSION"

if (!$nogit) {
    git add .
    git status

    if (!$yes) {
        $answer = Read-Host "see git status above, does this look correct? (y/n)"
        if ($answer -ne 'y') {
            exit
        }
    }

    git commit -m "bumped version to v$version"
    git push

    git tag -a v$version -m v$version
    git push --tags
}