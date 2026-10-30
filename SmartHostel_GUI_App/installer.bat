@echo off
setlocal
cd /d "%~dp0"

set "INSTALL_DIR=%LOCALAPPDATA%\SmartHostelFullGUI"
set "DESKTOP_DIR=%USERPROFILE%\Desktop"
set "START_MENU_DIR=%APPDATA%\Microsoft\Windows\Start Menu\Programs"
set "SHORTCUT_NAME=Smart Hostel Full GUI.lnk"

if not exist "SmartHostel.exe" (
    echo Build SmartHostel.exe first.
    pause
    exit /b 1
)

if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"
copy /Y "SmartHostel.exe" "%INSTALL_DIR%\" > nul
copy /Y "students.txt" "%INSTALL_DIR%\" > nul
copy /Y "entrylog.txt" "%INSTALL_DIR%\" > nul
copy /Y "exitlog.txt" "%INSTALL_DIR%\" > nul
copy /Y "visitors.txt" "%INSTALL_DIR%\" > nul
copy /Y "README.txt" "%INSTALL_DIR%\" > nul

"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -ExecutionPolicy Bypass -Command "$desktop=[Environment]::GetFolderPath('Desktop'); $startMenu=[Environment]::GetFolderPath('Programs'); $ws=New-Object -ComObject WScript.Shell; $s=$ws.CreateShortcut((Join-Path $desktop '%SHORTCUT_NAME%')); $s.TargetPath='%INSTALL_DIR%\SmartHostel.exe'; $s.WorkingDirectory='%INSTALL_DIR%'; $s.Save(); $m=$ws.CreateShortcut((Join-Path $startMenu '%SHORTCUT_NAME%')); $m.TargetPath='%INSTALL_DIR%\SmartHostel.exe'; $m.WorkingDirectory='%INSTALL_DIR%'; $m.Save()"

echo Installed to %INSTALL_DIR%
pause
