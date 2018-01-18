REM 编译

REM pyinstaller --console --onefile --clean 价格采集.py
REM xcopy /Y .\dist\价格采集.exe .\price_collect\bin\x64\Debug
REM xcopy /Y .\dist\价格采集.exe .\price_collect\bin\x64\Release
REM xcopy /Y .\dist\价格采集.exe .\price_collect\bin\x86\Debug
REM xcopy /Y .\dist\价格采集.exe .\price_collect\bin\x86\Release
REM xcopy /Y .\dist\价格采集.exe .\price_collect\bin\Debug
REM xcopy /Y .\dist\价格采集.exe .\price_collect\bin\Release

REM 删除编译的中间文件

REM del /S /Q build
REM del /S /Q __pycache__

REM 删除空文件夹

REM RD /S /Q build
REM RD /S /Q __pycache__

del /s .\price_collect\bin\x64\Debug\phantomjs.exe
del /s .\price_collect\bin\x64\Release\phantomjs.exe
del /s .\price_collect\bin\x86\Debug\phantomjs.exe
del /s .\price_collect\bin\x86\Release\phantomjs.exe
del /s .\price_collect\bin\Debug\phantomjs.exe
del /s .\price_collect\bin\Release\phantomjs.exe

pause
