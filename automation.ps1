# create a simple script that does the git process

$branchName = "release-2024" 
$decommMessage = "TestDescription"
$jiraCardName = "TEST-123"
$newBranchName = "$jiraCardName-$decommMessage"
$commitMessage = "testing commit message"
$filesToAdd = @(
    "dir1\Huxley.API.Full.sln"
    )

function RunCommand {
    param (
        [string]$cmd
    )
    Invoke-Expression $cmd

    if ($LASTEXITCODE -ne 0) {
        Exit
    }
}

function RunScript() {
    # Set-Location C:\Users\Bella.LeberSmeaton\Documents\Mira\deccomscript\testRunningScripts\testRepo
    $scriptRoot = $PSScriptRoot # holds the dir where the script is located
    Set-Location $scriptRoot
        
    RunCommand("git checkout $branchName")

    RunCommand("git checkout -b $newBranchName")

    $versionToRemove = Read-Host "Please provide the version you wish to remove..."
    
    $arlRootDirectory = Read-Host "Please provide the root of your ARL repo on your location machine..."
    
    # Run console app
    AppExitCode = & ../../transformationScript.ps1 $versionToRemove $arlRootDirectory
    
    if ($consoleAppExitCode -eq 1) {
        RunCommand("git reset --hard")  # Reset any changes made
        Exit
    }

    Read-Host "Press Enter to continue after making your changes"
    
    RunCommand("git add " + ($filesToAdd -join " "))

    RunCommand("git commit -m ""$commitMessage""")

    RunCommand("git push --set-upstream origin $newBranchName")
}

RunScript

Exit