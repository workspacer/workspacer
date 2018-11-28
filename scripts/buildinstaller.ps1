 param (
    [string]$buildDir,
    [string]$version = ""
 )

$setupDir = "src\Workspacer.Setup"

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

& ".\$wixDir\heat.exe" dir $buildDir -o $outDir\Workspacer.wxs -t $setupDir\Workspacer.xslt -scon -sfrag -srd -sreg -gg -cg Workspacer -dr INSTALLDIR -var var.SourceDir

& ".\$wixDir\candle.exe" $setupDir\Product.wxs -ext WixUIExtension -o $outDir\Product.wixobj 
& ".\$wixDir\candle.exe" $outDir\Workspacer.wxs -ext WixUIExtension -o $outDir\Workspacer.wixobj -dSourceDir="$buildDir"
& ".\$wixDir\light.exe" -out $outDir\Workspacer$version.msi $outDir\Product.wixobj $outDir\Workspacer.wixobj -ext WixUIExtension