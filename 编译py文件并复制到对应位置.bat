REM 编译
pyinstaller --console --onefile 价格采集.py
xcopy /Y .\dist\价格采集.exe .\price_collect\bin\x64\Debug
REM 删除编译的中间文件
del /S /Q build
del /S /Q __pycache__
del /S /Q dist
REM 删除空文件夹
RD /S /Q build
RD /S /Q __pycache__
RD /S /Q dist
pause
