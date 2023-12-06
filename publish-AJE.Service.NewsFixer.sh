#!/bin/bash
pushd src/Service/NewsFixer
dotnet publish -c Release -r linux-x64 --sc --output /usr/local/bin/AJE.Service.NewsFixer/
popd
