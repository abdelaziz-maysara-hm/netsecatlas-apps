# Publishes every app as a single-file, self-contained exe with a stable file name.
param([string]$Rid = "win-x64", [string]$Out = "dist")
$ErrorActionPreference = "Stop"
$apps = Get-Content "$PSScriptRoot/apps.json" | ConvertFrom-Json
New-Item -ItemType Directory -Force -Path $Out | Out-Null
foreach ($a in $apps) {
  $tmp = Join-Path "obj-publish" "$($a.project)-$Rid"
  dotnet publish "apps/$($a.project)/$($a.project).csproj" -c Release -r $Rid --self-contained true `
    -p:PublishSingleFile=true -o $tmp -nologo
  if ($LASTEXITCODE -ne 0) { throw "publish failed: $($a.project)" }
  Copy-Item (Join-Path $tmp "NetSecAtlas.$($a.project).exe") (Join-Path $Out "NetSecAtlas-$($a.project)-$Rid.exe") -Force
}
Get-ChildItem $Out | Format-Table Name, Length
