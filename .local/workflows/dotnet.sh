#!/bin/bash

dotnet restore ../../Commons/Commons/Commons.csproj
dotnet build --no-restore ../../Commons/Commons/Commons.csproj
dotnet pack -o ../../ ../../Commons/Commons/Commons.csproj -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg -p:PackageID="CFSant.Dev.Commons" -p:PackageVersion="0.0.0"