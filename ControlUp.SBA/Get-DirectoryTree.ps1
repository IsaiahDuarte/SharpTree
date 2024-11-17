<#
.SYNOPSIS
    Retrieves the directory tree structure starting from a specified path and outputs it as a JSON file.

.PARAMETER Path
    The root directory path to start reading the directory tree. Defaults to the user's profile directory.

.PARAMETER SharpTreePath
    The path to the SharpTree DLL. If not provided, the script will download and extract it.

.PARAMETER MinSize
    The minimum file size (in bytes) to include in the directory tree. Defaults to 10 MB.

.PARAMETER MaxDepth
    The maximum depth to traverse in the directory tree. Defaults to -1 (no limit).

.PARAMETER OutputJsonPath
    The path to save the output JSON file. Defaults to a file named "DirectoryTree.json" in the TEMP directory.

.EXAMPLE
    .\Get-DirectoryTree.ps1 -Path "C:\Users\ExampleUser" -MinSize 5242880 -MaxDepth 5 -OutputJsonPath "C:\Temp\DirectoryTree.json"
    Retrieves the directory tree starting from "C:\Users\ExampleUser", including files larger than 5 MB, up to a depth of 5 levels, and saves the output to "C:\Temp\DirectoryTree.json".

.NOTES
    This script requires PowerShell 5 or 7 and an internet connection to download the SharpTree DLL if not provided.
#>
param(
    [Parameter(Mandatory = $false)]
    [ValidateScript({ Test-Path $_ -and (Get-Item $_).PSIsContainer })]
    [string] $Path = $ENV:USERPROFILE,

    [Parameter(Mandatory = $false)]
    [ValidateScript({ Test-Path $_ })]
    [string] $SharpTreePath,

    [Parameter(Mandatory = $false)]
    [ValidateRange(0, [int]::MaxValue)]
    [int] $MinSize = 10485760,

    [Parameter(Mandatory = $false)]
    [ValidateRange(-1, [int]::MaxValue)]
    [int] $MaxDepth = -1,

    [Parameter(Mandatory = $false)]
    [string] $OutputJsonPath = (Join-Path -Path $ENV:TEMP -ChildPath "DirectoryTree.json")
)

function Get-GitPath {
    switch ($PSVersionTable.PSVersion.Major) {
        5 { return "https://github.com/user-attachments/files/17787130/SharpTree.Core.Powershell.zip" }
        7 { return "https://github.com/user-attachments/files/17787130/SharpTree.Core.zip" }
        default {
            Write-Error "Unsupported PowerShell version: $($PSVersionTable.PSVersion)"
            exit 1
        }
    }
}

function Get-SharpTree {
    param(
        [string] $GitPath,
        [string] $DestinationPath
    )

    try {
        if (!(Test-Path $DestinationPath)) {
            New-Item -ItemType Directory -Path $DestinationPath -Force | Out-Null
        }

        $zipPath = Join-Path $DestinationPath "SharpTree.zip"
        Invoke-WebRequest -Uri $GitPath -OutFile $zipPath

        Expand-Archive -Path $zipPath -DestinationPath $DestinationPath -Force
        Remove-Item -Path $zipPath -Force

        $dllPath = Get-ChildItem -Path $DestinationPath -Filter "SharpTree*.dll" -Recurse | Select-Object -First 1 -ExpandProperty FullName
        return $dllPath
    }
    catch {
        Write-Error "Failed to download or extract SharpTree DLL: $_"
        exit 1
    }
}

try {
    $ErrorActionPreference = "Stop"
    $GitPath = Get-GitPath
    $TempSharpTreePath = "$env:TEMP\SharpTree-$($PSVersionTable.PSVersion.Major)"

    if ([string]::IsNullOrEmpty($SharpTreePath)) {
        if ((Get-ChildItem -Path "$TempSharpTreePath" -Filter "SharpTree*.dll" -Recurse -ErrorAction SilentlyContinue | Measure-Object).Count -eq 0) {
            $SharpTreeDll = Get-SharpTree -GitPath $GitPath -DestinationPath $TempSharpTreePath
        } else {
            $SharpTreeDll = Get-ChildItem -Path "$TempSharpTreePath" -Filter "SharpTree*.dll" -Recurse | Select-Object -First 1 -ExpandProperty FullName
        }
    } else {
        $SharpTreeDll = $SharpTreePath
    }

    if (!(Test-Path $SharpTreeDll)) {
        Write-Error "SharpTree DLL not found at '$SharpTreeDll'"
        exit 1
    }

    Add-Type -Path $SharpTreeDll
    $RootPath = ($Path -Split "\\")[0] + "\"
    $FSBehavior = [SharpTree.Core.Behaviors.FilesystemBehaviorType]::SingleVolume
    $FSBehaviors = [SharpTree.Core.Behaviors.FilesystemBehaviorsFactory]::Create($FSBehavior, $RootPath)
    $ErrorActionPreference = "SilentlyContinue"
    $Node = [SharpTree.Core.Services.FileSystemReader]::ReadRecursive($Path, $false, $FSBehaviors, $MinSize, $MaxDepth)
    $ErrorActionPreference = "Stop"
    $Node | ConvertTo-Json -Depth 100 | Out-File -FilePath $OutputJsonPath
        
    Write-Host "Saved Directory Tree to $OutputJsonPath"
} catch {
    Write-Error "Failed to get directory tree: $_"
    exit 1
}