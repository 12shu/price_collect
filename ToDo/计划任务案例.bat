REM 创建计划任务
schtasks  /create  /tn  testschtask /tr  notepad.exe /sc  DAILY /st  22:28:00 
REM 删除
REM schtasks  /Delete  /tn testschtask  /F
REM 查询
chcp  437  
schtasks  /Query  /tn testschtask 
pause
