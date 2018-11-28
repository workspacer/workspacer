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
$wixDir = "wix"

if (!(Test-Path $outDir)) {
    mkdir $outDir
} else {
    del out/*
}

& ".\$wixDir\heat.exe" dir $buildDir -o $setupDir\Workspacer.wxs -t $setupDir\Workspacer.xslt -scon -sfrag -srd -sreg -gg -cg Workspacer -dr INSTALLDIR -var var.SourceDir
& ".\$wixDir\heat.exe" dir $barBuildDir -o $setupDir\Workspacer.Bar.wxs -t $setupDir\Workspacer.Bar.xslt -scon -sfrag -srd -sreg -gg -cg Workspacer.Bar -dr INSTALLDIR -var var.SourceDir
& ".\$wixDir\heat.exe" dir $menuBuildDir -o $setupDir\Workspacer.ActionMenu.wxs -t $setupDir\Workspacer.ActionMenu.xslt -scon -sfrag -srd -sreg -gg -cg Workspacer.ActionMenu -dr INSTALLDIR -var var.SourceDir
& ".\$wixDir\heat.exe" dir $focusBuildDir -o $setupDir\Workspacer.FocusIndicator.wxs -t $setupDir\Workspacer.FocusIndicator.xslt -scon -sfrag -srd -sreg -gg -cg Workspacer.FocusIndicator -dr INSTALLDIR -var var.SourceDir

& ".\$wixDir\candle.exe" $setupDir\Product.wxs -ext WixUIExtension -o $outDir\Product.wixobj 
& ".\$wixDir\candle.exe" $setupDir\Workspacer.wxs -ext WixUIExtension -o $outDir\Workspacer.wixobj -dSourceDir="$buildDir"
& ".\$wixDir\candle.exe" $setupDir\Workspacer.Bar.wxs -ext WixUIExtension -o $outDir\Workspacer.Bar.wixobj -dSourceDir="$barBuildDir"
& ".\$wixDir\candle.exe" $setupDir\Workspacer.ActionMenu.wxs -ext WixUIExtension -o $outDir\Workspacer.ActionMenu.wixobj -dSourceDir="$menuBuildDir"
& ".\$wixDir\candle.exe" $setupDir\Workspacer.FocusIndicator.wxs -ext WixUIExtension -o $outDir\Workspacer.FocusIndicator.wixobj -dSourceDir="$focusBuildDir"
& ".\$wixDir\light.exe" -out $outDir\Workspacer-$version.msi $outDir\Product.wixobj $outDir\Workspacer.wixobj $outDir\Workspacer.Bar.wixobj $outdir\Workspacer.ActionMenu.wixobj $outdir\Workspacer.FocusIndicator.wixobj -ext WixUIExtension