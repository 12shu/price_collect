REM �����ƻ�����
schtasks  /create  /tn  testschtask /tr  notepad.exe /sc  DAILY /st  22:28:00 
REM ɾ��
REM schtasks  /Delete  /tn testschtask  /F
REM ��ѯ
chcp  437  
schtasks  /Query  /tn testschtask 
pause
