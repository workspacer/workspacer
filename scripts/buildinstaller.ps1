 param (
    [string]$config,
    [string]$platform
 )

$setupDir = "src\Workspacer.Setup"

$buildDir = "src\Workspacer\bin\$platform\$config\"
$barBuildDir = "src\Workspacer.Bar\bin\$platform\$config\"
$menuBuildDir = "src\Workspacer.ActionMenu\bin\$platform\$config\"
$focusBuildDir = "src\Workspacer.FocusIndicator\bin\$platform\$config\"

$version = Get-Content "VERSION"

$outDir = "out"

if (!(Test-Path $outDir)) {
    mkdir $outDir
} else {
    del out/*
}

heat dir $buildDir -o $setupDir\Workspacer.wxs -t $setupDir\Workspacer.xslt -scon -sfrag -srd -sreg -gg -cg Workspacer -dr INSTALLDIR -var var.SourceDir
heat dir $barBuildDir -o $setupDir\Workspacer.Bar.wxs -t $setupDir\Workspacer.Bar.xslt -scon -sfrag -srd -sreg -gg -cg Workspacer.Bar -dr INSTALLDIR -var var.SourceDir
heat dir $menuBuildDir -o $setupDir\Workspacer.ActionMenu.wxs -t $setupDir\Workspacer.ActionMenu.xslt -scon -sfrag -srd -sreg -gg -cg Workspacer.ActionMenu -dr INSTALLDIR -var var.SourceDir
heat dir $focusBuildDir -o $setupDir\Workspacer.FocusIndicator.wxs -t $setupDir\Workspacer.FocusIndicator.xslt -scon -sfrag -srd -sreg -gg -cg Workspacer.FocusIndicator -dr INSTALLDIR -var var.SourceDir

candle $setupDir\Product.wxs -ext WixUIExtension -o $outDir\Product.wixobj 
candle $setupDir\Workspacer.wxs -ext WixUIExtension -o $outDir\Workspacer.wixobj -dSourceDir="$buildDir"
candle $setupDir\Workspacer.Bar.wxs -ext WixUIExtension -o $outDir\Workspacer.Bar.wixobj -dSourceDir="$barBuildDir"
candle $setupDir\Workspacer.ActionMenu.wxs -ext WixUIExtension -o $outDir\Workspacer.ActionMenu.wixobj -dSourceDir="$menuBuildDir"
candle $setupDir\Workspacer.FocusIndicator.wxs -ext WixUIExtension -o $outDir\Workspacer.FocusIndicator.wixobj -dSourceDir="$focusBuildDir"
light -out $outDir\Workspacer-$version.msi $outDir\Product.wixobj $outDir\Workspacer.wixobj $outDir\Workspacer.Bar.wixobj $outdir\Workspacer.ActionMenu.wixobj $outdir\Workspacer.FocusIndicator.wixobj -ext WixUIExtension