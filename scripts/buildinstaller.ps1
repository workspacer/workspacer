 param (
    [string]$config,
    [string]$platform,
    [string]$version = ""
 )

$setupDir = "src\Workspacer.Setup"

$buildDir = "src\Workspacer\bin\$platform\$config\"
$outDir = "out"
$wixDir = "wix"

if (!(Test-Path $outDir)) {
    mkdir $outDir
} else {
    del out/*
}

if (![string]::IsNullOrEmpty($version)) {
    $version = "-$version"
}

& ".\$wixDir\heat.exe" dir $buildDir -o $setupDir\Workspacer.wxs -t $setupDir\Workspacer.xslt -scon -sfrag -srd -sreg -gg -cg Workspacer -dr INSTALLDIR -var var.SourceDir

& ".\$wixDir\candle.exe" $setupDir\Product.wxs -ext WixUIExtension -o $outDir\Product.wixobj 
& ".\$wixDir\candle.exe" $setupDir\Workspacer.wxs -ext WixUIExtension -o $outDir\Workspacer.wixobj -dSourceDir="$buildDir"
& ".\$wixDir\light.exe" -out $outDir\Workspacer$version.msi $outDir\Product.wixobj $outDir\Workspacer.wixobj -ext WixUIExtension