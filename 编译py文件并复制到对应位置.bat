REM 编译

pyinstaller --console --onefile --clean 价格采集.py
xcopy /Y .\dist\价格采集.exe .\price_collect\bin\x64\Debug
xcopy /Y .\dist\价格采集.exe .\price_collect\bin\x64\Release
xcopy /Y .\dist\价格采集.exe .\price_collect\bin\x86\Debug
xcopy /Y .\dist\价格采集.exe .\price_collect\bin\x86\Release
xcopy /Y .\dist\价格采集.exe .\price_collect\bin\Debug
xcopy /Y .\dist\价格采集.exe .\price_collect\bin\Release

REM 删除编译的中间文件

del /S /Q build
del /S /Q __pycache__

REM 删除空文件夹

RD /S /Q build
RD /S /Q __pycache__

pause
