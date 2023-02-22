@echo off

rem Scripts Projekt compilieren
"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" /t:Scripts /p:Configuration=Debug
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe" /t:Scripts /p:Configuration=Debug

echo | .\packages\JavaScriptMerger\Connectiv.JavaScriptMerger.exe "%cd%\Scripts"

rem Scripte hochladen using default connection. Remove piping to prompt user to select a connection
echo | .\packages\JavaScriptUploader\Connectiv.JavaScriptUploader.exe 

:: rem also possible to upload scripts using named connection
:: .\packages\JavaScriptUploader\Connectiv.JavaScriptUploader.exe <connectionName>

pause
