@echo off
setlocal

set /p string=<config/config.json

rem Remove quotes
set string=%string:"=%
rem Remove braces
set "string=%string:~2,-2%"
rem Change colon+space by equal-sign
set "string=%string:: ==%"
rem Separate parts at comma into individual assignments
set "%string:, =" & set "%"
                                                               
cd Backend
start dotnet Backend.dll --configuration Release --launch-profile Production
                                                        
cd ../Frontend
start dotnet Frontend.dll --configuration Release --launch-profile Production
                                                        
cd ../TextListener
start dotnet TextListener.dll --configuration Release --launch-profile Production
