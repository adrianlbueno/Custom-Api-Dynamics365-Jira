param($installPath, $toolsPath, $package, $project)
New-Item .\packages\JavaScriptUploader -type directory -force
Copy-Item "$($toolsPath)\JavaScriptUploader\*" ".\packages\JavaScriptUploader\" -exclude "uploadScripts.bat" -Verbose
Copy-Item "$($toolsPath)\JavaScriptUploader\uploadScripts.bat" "." -Verbose