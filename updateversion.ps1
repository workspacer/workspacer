$infos = Get-ChildItem -Path . -Filter "AssemblyInfo.cs" -Recurse -ErrorAction SilentlyContinue -Force

$version = Get-Content "VERSION"

foreach ($file in $infos)
{
    (Get-Content $file.PSPath) |
    Foreach-Object { $_ -replace "AssemblyVersion\("".*""\)", "AssemblyVersion(""$version.*"")" } |
    Set-Content $file.PSPath
}

$setupProjs = Get-ChildItem -Path . -Filter "*.vdproj" -Recurse -ErrorAction SilentlyContinue -Force
foreach ($file in $setupProjs)
{
    (Get-Content $file.PSPath) |
    Foreach-Object { $_ -replace """ProductVersion"" = ""8:.*""", """ProductVersion"" = ""8:$version""" } |
    Set-Content $file.PSPath
}