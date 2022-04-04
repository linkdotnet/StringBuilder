@echo off
echo "This script uses docfx - Please make sure it is installed on your machine"
docfx site/docfx.json
docfx serve site/_site