$msbuild = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe"

$setupDir = "src\Workspacer.Setup"
$config = "Release"
$platform ="x64"


$buildDir = "src\Workspacer\bin\$platform\$config\"
$barBuildDir = "src\Workspacer.Bar\bin\$platform\$config\"

$infos = Get-ChildItem -Path . -Filter "AssemblyInfo.cs" -Recurse -ErrorAction SilentlyContinue -Force
$version = Get-Content "VERSION"

$outDir = "out"

if (!(Test-Path $outDir)) {
    mkdir $outDir
} else {
    del out/*
}

foreach ($file in $infos)
{
    "fixing version for $file"
    (Get-Content $file.PSPath) |
    Foreach-Object { $_ -replace "AssemblyVersion\("".*""\)", "AssemblyVersion(""$version.*"")" } |
    Set-Content $file.PSPath
}

$setupProjs = Get-ChildItem -Path . -Filter "Product.wxs" -Recurse -ErrorAction SilentlyContinue -Force
foreach ($file in $setupProjs)
{
    "fixing version for $file"
    (Get-Content $file.PSPath) |
    Foreach-Object { $_ -replace "Version="".*"" Manu", "Version=""$version"" Manu" } |
    Set-Content $file.PSPath
}

& $msbuild Workspacer.sln /t:Clean,Build /p:Configuration=Release /p:Platform=x64

heat dir $buildDir -o $setupDir\Workspacer.wxs -t $setupDir\Workspacer.xslt -scon -sfrag -srd -sreg -gg -cg Workspacer -dr INSTALLDIR -var var.SourceDir
heat dir $barBuildDir -o $setupDir\Workspacer.Bar.wxs -t $setupDir\Workspacer.Bar.xslt -scon -sfrag -srd -sreg -gg -cg Workspacer.Bar -dr INSTALLDIR -var var.SourceDir

candle $setupDir\Product.wxs -ext WixUIExtension -o $outDir\Product.wixobj 
candle $setupDir\Workspacer.wxs -ext WixUIExtension -o $outDir\Workspacer.wixobj -dSourceDir="$buildDir"
candle $setupDir\Workspacer.Bar.wxs -ext WixUIExtension -o $outDir\Workspacer.Bar.wixobj -dSourceDir="$barBuildDir"
light -out $outDir\Workspacer-$version.msi $outDir\Product.wixobj $outDir\Workspacer.wixobj $outDir\Workspacer.Bar.wixobj -ext WixUIExtension