@echo off
set DOCFX_VERSION=2.80.1
dotnet tool update -g docfx --version %DOCFX_VERSION% || dotnet tool install -g docfx --version %DOCFX_VERSION%
docfx site/docfx.json
docfx serve site/_site