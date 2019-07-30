#!/usr/bin/env bash
set -e 

API_KEY=$1

dotnet pack LazyAsync/LazyAsync.csproj -c Release -o out /p:NuspecFile=LazyAsync.nuspec

dotnet nuget push LazyAsync/out/LazyAsync.NET.1.0.0.nupkg --api-key $API_KEY --source https://api.nuget.org/v3/index.json