 param (
    [string]$version,
    [string]$build,
    [string]$url,
    [string]$out
 )

$xml = Get-Content "scripts/update-template.xml"

$xml = $xml -replace "REPLACE_VERSION", $version
$xml = $xml -replace "REPLACE_BUILD", $build
$xml = $xml -replace "REPLACE_URL", $url

$xml | Set-Content $out