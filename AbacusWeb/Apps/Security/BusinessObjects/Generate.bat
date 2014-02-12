CLS
@ECHO OFF

ECHO.
ECHO *************************************************
ECHO Running CodeSmith...
ECHO *************************************************
ECHO.
"C:\Program Files\CodeSmith\v3.0\cs.exe" /properties:BusinessObjects.xml

ExecuteT-SQL.bat

PAUSE