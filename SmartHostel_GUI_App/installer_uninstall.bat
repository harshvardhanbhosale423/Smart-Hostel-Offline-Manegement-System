@echo off
setlocal
set "INSTALL_DIR=%LOCALAPPDATA%\SmartHostelFullGUI"
set "SHORTCUT_NAME=Smart Hostel Full GUI.lnk"

"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -ExecutionPolicy Bypass -Command "$desktop=[Environment]::GetFolderPath('Desktop'); $programs=[Environment]::GetFolderPath('Programs'); $d=Join-Path $desktop '%SHORTCUT_NAME%'; $p=Join-Path $programs '%SHORTCUT_NAME%'; if(Test-Path $d){Remove-Item $d -Force}; if(Test-Path $p){Remove-Item $p -Force}"

if exist "%INSTALL_DIR%" rmdir /S /Q "%INSTALL_DIR%"

echo Uninstalled Smart Hostel Full GUI.
pause
