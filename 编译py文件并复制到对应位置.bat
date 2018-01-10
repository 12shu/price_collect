REM 编译
pyinstaller --console --onefile --clean 价格采集.py
REM xcopy /Y .\dist\价格采集.exe .\price_collect\bin\x64\Debug
REM 删除编译的中间文件
REM del /S /Q build
REM del /S /Q __pycache__
REM del /S /Q dist
REM 删除空文件夹
REM RD /S /Q build
REM RD /S /Q __pycache__
REM RD /S /Q dist
REM pause
