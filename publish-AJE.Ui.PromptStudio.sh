#!/bin/bash
pushd src/Ui/PromptStudio
dotnet publish -c Release -r linux-x64 --sc --output /usr/local/bin/AJE.Ui.PromptStudio/
popd
