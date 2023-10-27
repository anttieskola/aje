#!/bin/bash
pushd src/Service/NewsAnalyzer
dotnet publish -c Release -r linux-x64 --sc --output /usr/local/bin/AJE.Service.NewsAnalyzer/
popd
