ApiKey=$1
Source=$2

nuget pack ./LazyAsync/LazyAsync.nuspec -Verbosity detailed
nuget push ./LazyAsync/LazyAsync.NET.nupkg -Verbosity detailed -ApiKey $NUGET_API_KEY -Source https://api.nuget.org/v3/index.json