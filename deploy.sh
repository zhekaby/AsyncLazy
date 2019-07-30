ApiKey=$1
Source=$2

nuget pack ./LazyAsync/LazyAsync.nuspec -Verbosity detailed
nuget push ./LazyAsync/LazyAsync.NET.nupkg -Verbosity detailed -ApiKey $ApiKey -Source $Source