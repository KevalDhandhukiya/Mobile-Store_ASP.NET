@echo off
echo Starting E-commerce Backend API on port 5001...
cd /d "D:\Z\E-commerce_MobileStore"
dotnet run --urls "http://localhost:5001" --no-launch-profile
pause
