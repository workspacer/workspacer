 param (
    [string]$buildDir,
    [string]$version = ""
 )

$setupDir = "src\workspacer.Setup"

$outDir = "out"

if (!(Test-Path $outDir)) {
    mkdir $outDir
} else {
    del out/*
}

if (![string]::IsNullOrEmpty($version)) {
    $version = "-$version"
}

heat dir $buildDir -o $outDir\workspacer.wxs -t $setupDir\workspacer.xslt -scon -sfrag -srd -sreg -gg -cg workspacer -dr INSTALLDIR -var var.SourceDir

candle $setupDir\Product.wxs -ext WixUIExtension -o $outDir\Product.wixobj 
candle $outDir\workspacer.wxs -ext WixUIExtension -o $outDir\workspacer.wixobj -dSourceDir="$buildDir"
light -out $outDir\workspacer$version.msi $outDir\Product.wixobj $outDir\workspacer.wixobj -ext WixUIExtension
