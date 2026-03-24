# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/p3rpc.asyncassetloader/*" -Force -Recurse
dotnet publish "./p3rpc.asyncassetloader.csproj" -c Release -o "$env:RELOADEDIIMODS/p3rpc.asyncassetloader" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location