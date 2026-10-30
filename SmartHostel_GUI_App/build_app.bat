@echo off
setlocal
cd /d "%~dp0"

"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\csc.exe" /target:winexe /out:SmartHostel.exe /reference:System.Windows.Forms.dll /reference:System.Drawing.dll SmartHostelApp.cs

if errorlevel 1 (
    echo Build failed.
    pause
    exit /b 1
)

echo Build successful.
pause
