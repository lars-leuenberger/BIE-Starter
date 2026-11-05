@echo off
cd /d "%~dp0"
dotnet run --project src/BIE.Web --urls http://localhost:5080
pause
