# Runs each published x64 app with --demo/--screenshot to capture its real UI (English and Arabic).
param([string]$Dist = "dist", [string]$Out = "screenshots")
$apps = Get-Content "$PSScriptRoot/apps.json" | ConvertFrom-Json
New-Item -ItemType Directory -Force -Path $Out | Out-Null
foreach ($a in $apps) {
  foreach ($lang in "en", "ar") {
    $exe = Resolve-Path (Join-Path $Dist "NetSecAtlas-$($a.project)-win-x64.exe")
    $png = Join-Path (Resolve-Path $Out) "$($a.slug)-$lang.png"
    $p = Start-Process -FilePath $exe -ArgumentList "--lang", $lang, "--screenshot", "`"$png`"", "--delay", $a.delay -PassThru
    if (-not $p.WaitForExit(([int]$a.delay + 60) * 1000)) { $p.Kill(); Write-Warning "timeout: $($a.project) $lang" }
    if (Test-Path $png) { Write-Host "ok $png" } else { Write-Warning "no screenshot: $($a.project) $lang" }
  }
}
