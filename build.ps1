<#
.SYNOPSIS
  Build OpenPCL (src-cs\OpenPCL) with a Visual Studio / Build Tools MSBuild.

.DESCRIPTION
  1. Finds MSBuild (-MSBuild, $env:MSBUILD, vswhere, or well-known folders). It must be a *modern* MSBuild
     that ships Roslyn (...\MSBuild\Current\Bin\Roslyn\csc.exe) - the .NET Framework 4.x MSBuild only knows C# 5.
  2. Extracts the embedded third-party assemblies (MeloongCore, MeloongCore.Wpf, PCLCS, Newtonsoft.Json, NAudio, ...)
     from OpenPCL\Our\Resources\PCL.Resources.resx into src-cs\lib. They are compile-time references only; at run time
     the program loads them from its own resources, exactly like the original single-file exe.
  3. Builds Release and copies OpenPCL.exe to dist\.

  Needs: Windows, .NET Framework 4.8 runtime, MSBuild with the WPF targets (VS "Desktop development with .NET").

.EXAMPLE
  .\build.ps1
  .\build.ps1 -MSBuild "D:\VSS\MSBuild\Current\Bin\MSBuild.exe"
#>
[CmdletBinding()]
param(
    [string]$MSBuild = $env:MSBUILD,
    [ValidateSet('Release', 'Debug')][string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$src = Join-Path $root 'src-cs'
$proj = Join-Path $src 'OpenPCL\OpenPCL.csproj'
$resx = Join-Path $src 'OpenPCL\Our\Resources\PCL.Resources.resx'
$lib = Join-Path $src 'lib'

# ---- 1. MSBuild --------------------------------------------------------------------------------
function Find-MSBuild {
    if ($MSBuild -and (Test-Path $MSBuild)) { return (Resolve-Path $MSBuild).Path }
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path $vswhere) {
        $found = & $vswhere -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
        if ($found) { return $found }
    }
    foreach ($p in @("${env:ProgramFiles}\Microsoft Visual Studio\*\*\MSBuild\Current\Bin\MSBuild.exe",
                     "${env:ProgramFiles(x86)}\Microsoft Visual Studio\*\*\MSBuild\Current\Bin\MSBuild.exe")) {
        $found = Get-ChildItem $p -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($found) { return $found.FullName }
    }
    throw 'MSBuild not found. Install Visual Studio / Build Tools (with the .NET desktop workload) or pass -MSBuild <path>.'
}
$msbuildExe = Find-MSBuild
if (-not (Test-Path (Join-Path (Split-Path $msbuildExe) 'Roslyn\csc.exe'))) {
    throw "This MSBuild has no Roslyn compiler next to it ($msbuildExe). The source needs a modern C# compiler; use Visual Studio 2019+ / Build Tools."
}
Write-Host "MSBuild : $msbuildExe"

# ---- 2. embedded libraries -> src-cs\lib -------------------------------------------------------
if (-not (Test-Path $resx)) {
    throw "Missing $resx (git-ignored asset). Re-create the project assets by decompiling the original exe with dnSpy."
}
if (-not (Test-Path (Join-Path $lib 'MeloongCore.dll')) -or ((Get-Item $resx).LastWriteTime -gt (Get-Item (Join-Path $lib 'MeloongCore.dll')).LastWriteTime)) {
    Write-Host "Extracting embedded assemblies to $lib ..."
    New-Item -ItemType Directory -Force -Path $lib | Out-Null
    $xml = New-Object System.Xml.XmlDocument
    $xml.Load($resx)
    foreach ($d in $xml.SelectNodes('//data[@type]')) {
        if ($d.type -notmatch 'System\.Byte\[\]') { continue }
        $bytes = [Convert]::FromBase64String(($d.value -replace '\s', ''))
        if ($bytes.Length -lt 2 -or $bytes[0] -ne 0x4D -or $bytes[1] -ne 0x5A) { continue }   # "MZ": a PE file
        [IO.File]::WriteAllBytes((Join-Path $lib ($d.name + '.dll')), $bytes)
    }
}

# ---- 3. build ----------------------------------------------------------------------------------
& $msbuildExe $proj -nologo -verbosity:minimal "-p:Configuration=$Configuration"
if ($LASTEXITCODE -ne 0) { throw "MSBuild failed (exit code $LASTEXITCODE)." }

$out = Join-Path $src "OpenPCL\bin\$Configuration"
$dist = Join-Path $root 'dist'
New-Item -ItemType Directory -Force -Path $dist | Out-Null
try {
    Copy-Item (Join-Path $out 'OpenPCL.exe') $dist -Force
    Copy-Item (Join-Path $out 'OpenPCL.exe.config') $dist -Force -ErrorAction SilentlyContinue
    $exe = Join-Path $dist 'OpenPCL.exe'
}
catch {
    # typically: an OpenPCL.exe from dist\ is still running
    Write-Warning "Could not update dist\OpenPCL.exe ($($_.Exception.Message.Trim())). Close the running OpenPCL and re-run, or use the fresh build in $out."
    $exe = Join-Path $out 'OpenPCL.exe'
}
Write-Host ("Built   : {0}  ({1:N0} bytes)" -f $exe, (Get-Item $exe).Length)
