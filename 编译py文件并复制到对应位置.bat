REM ����
pyinstaller --console --onefile �۸�ɼ�.py
xcopy /Y .\dist\�۸�ɼ�.exe .\price_collect\bin\x64\Debug
REM ɾ��������м��ļ�
del /S /Q build
del /S /Q __pycache__
del /S /Q dist
REM ɾ�����ļ���
RD /S /Q build
RD /S /Q __pycache__
RD /S /Q dist
pause
