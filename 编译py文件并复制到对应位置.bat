REM ����

pyinstaller --console --onefile --clean �۸�ɼ�.py
xcopy /Y .\dist\�۸�ɼ�.exe .\price_collect\bin\x64\Debug
xcopy /Y .\dist\�۸�ɼ�.exe .\price_collect\bin\x64\Release
xcopy /Y .\dist\�۸�ɼ�.exe .\price_collect\bin\x86\Debug
xcopy /Y .\dist\�۸�ɼ�.exe .\price_collect\bin\x86\Release
xcopy /Y .\dist\�۸�ɼ�.exe .\price_collect\bin\Debug
xcopy /Y .\dist\�۸�ɼ�.exe .\price_collect\bin\Release

REM ɾ��������м��ļ�

del /S /Q build
del /S /Q __pycache__

REM ɾ�����ļ���

RD /S /Q build
RD /S /Q __pycache__

pause
