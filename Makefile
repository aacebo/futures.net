publish:
    dotnet nuget push "**/*.nupkg" --skip-duplicate --api-key $env:NUGET_API_KEY -s https://api.nuget.org/v3/index.json