REM ����

REM pyinstaller --console --onefile --clean �۸�ɼ�.py
REM xcopy /Y .\dist\�۸�ɼ�.exe .\price_collect\bin\x64\Debug
REM xcopy /Y .\dist\�۸�ɼ�.exe .\price_collect\bin\x64\Release
REM xcopy /Y .\dist\�۸�ɼ�.exe .\price_collect\bin\x86\Debug
REM xcopy /Y .\dist\�۸�ɼ�.exe .\price_collect\bin\x86\Release
REM xcopy /Y .\dist\�۸�ɼ�.exe .\price_collect\bin\Debug
REM xcopy /Y .\dist\�۸�ɼ�.exe .\price_collect\bin\Release

REM ɾ��������м��ļ�

REM del /S /Q build
REM del /S /Q __pycache__

REM ɾ�����ļ���

REM RD /S /Q build
REM RD /S /Q __pycache__

del /s .\price_collect\bin\x64\Debug\phantomjs.exe
del /s .\price_collect\bin\x64\Release\phantomjs.exe
del /s .\price_collect\bin\x86\Debug\phantomjs.exe
del /s .\price_collect\bin\x86\Release\phantomjs.exe
del /s .\price_collect\bin\Debug\phantomjs.exe
del /s .\price_collect\bin\Release\phantomjs.exe

pause
