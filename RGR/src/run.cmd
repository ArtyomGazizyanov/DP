@echo off
setlocal
                                                              
cd Node
start dotnet Node.dll "NODE1"
start dotnet Node.dll "NODE2"
start dotnet Node.dll "NODE3"
start dotnet Node.dll "NODE4"
start dotnet Node.dll "NODE5"

cd ../NodeManager
start dotnet NodeManager.dll --configuration Release --launch-profile Production

cd ../Client
start dotnet Client.dll --configuration Release --launch-profile Production