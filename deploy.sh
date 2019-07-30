#!/usr/bin/env bash
set -e 

API_KEY=$1
VERSION=$2

dotnet pack LazyAsync/LazyAsync.csproj -c Release -o out /p:NuspecFile=LazyAsync.nuspec -p:PackageVersion=$VERSION

dotnet nuget push LazyAsync/out/LazyAsync.NET.$VERSION.nupkg --api-key $API_KEY --source https://api.nuget.org/v3/index.json