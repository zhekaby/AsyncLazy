ApiKey=$1
Source=$2
curl -L -o nuget.exe "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"

nuget pack ./LazyAsync/LazyAsync.nuspec -Verbosity detailed
nuget push ./LazyAsync/LazyAsync.NET.nupkg -Verbosity detailed -ApiKey $ApiKey -Source $Source
