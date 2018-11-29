 param (
    [string]$buildDir,
    [string]$version = ""
 )

$setupDir = "src\Workspacer.Setup"

$outDir = "out"

if (!(Test-Path $outDir)) {
    mkdir $outDir
} else {
    del out/*
}

if (![string]::IsNullOrEmpty($version)) {
    $version = "-$version"
}

& ".\tools\wix\heat.exe" dir $buildDir -o $outDir\Workspacer.wxs -t $setupDir\Workspacer.xslt -scon -sfrag -srd -sreg -gg -cg Workspacer -dr INSTALLDIR -var var.SourceDir

& ".\tools\wix\candle.exe" $setupDir\Product.wxs -ext WixUIExtension -o $outDir\Product.wixobj 
& ".\tools\wix\candle.exe" $outDir\Workspacer.wxs -ext WixUIExtension -o $outDir\Workspacer.wixobj -dSourceDir="$buildDir"
& ".\tools\wix\light.exe" -out $outDir\Workspacer$version.msi $outDir\Product.wixobj $outDir\Workspacer.wixobj -ext WixUIExtension