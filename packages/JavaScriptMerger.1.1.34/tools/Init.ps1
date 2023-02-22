param($installPath, $toolsPath, $package, $project)
New-Item .\packages\JavaScriptMerger -type directory -force
Copy-Item "$($toolsPath)\JavaScriptMerger\*" ".\packages\JavaScriptMerger\" -Verbose