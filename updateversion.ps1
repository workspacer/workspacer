 param (
    [string]$version
 )

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

"$version" | Set-Content "VERSION"