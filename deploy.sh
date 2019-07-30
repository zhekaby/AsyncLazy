#!/usr/bin/env bash
set -e 

API_KEY=$1
VERSION=$2

dotnet pack LazyAsync/S4b.LazyAsyncNET.csproj -c Release -o out -p:PackageVersion=$VERSION
dotnet nuget push LazyAsync/out/S4b.LazyAsyncNET.$VERSION.nupkg --api-key $API_KEY --source https://api.nuget.org/v3/index.json