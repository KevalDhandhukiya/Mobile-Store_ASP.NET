@echo off
echo Starting E-commerce Frontend Web on port 5000...
cd /d "D:\Z\E-commerce_Mobile"
dotnet run --urls "http://localhost:5000" --no-launch-profile
pause
