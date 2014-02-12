
@ECHO OFF
CLS
REM Copies source code only for the Supporting People Web application to the destination directory.
REM The destination directory is created if necessary.
REM Files in the destination directory will be overwritten.

REM History
REM	-------
REM	1	14/03/2007	MikeVO		Initial version.

SET DEST_ROOT=P:\Target .NET Builds\SPWeb\1.3\Source
SET SRC_TARGET_WEB_ROOT=C:\Inetpub\wwwroot\TargetWeb
SET SRC_ROOT=R:
SET LOGFILE=ArchiveSource.log

ECHO ========================================================
ECHO Clearing destination folder...
RMDIR /S /Q "%DEST_ROOT%"
DEL "%LOGFILE%"

ECHO ========================================================
ECHO Preparing to archive source code from source folders:
ECHO Preparing to archive source code from source folders: >> "%LOGFILE%"
ECHO "%SRC_TARGET_WEB_ROOT%"
ECHO "%SRC_TARGET_WEB_ROOT%" >> "%LOGFILE%"
ECHO "%SRC_ROOT%"
ECHO "%SRC_ROOT%" >> %LOGFILE%
ECHO.
ECHO. >> "%LOGFILE%"
ECHO to destination folder:
ECHO to destination folder: >> "%LOGFILE%"
ECHO "%DEST_ROOT%"
ECHO "%DEST_ROOT%" >> "%LOGFILE%"

ECHO ========================================================
ECHO Copying TargetWeb source...
ECHO Copying TargetWeb source... >> "%LOGFILE%"
XCOPY "%SRC_TARGET_WEB_ROOT%"\*.* "%DEST_ROOT%"\TargetWeb\ /I /E /Y /EXCLUDE:ArchiveSourceExclude.txt >> "%LOGFILE%"
ECHO Copying support assembly source...
XCOPY "%SRC_ROOT%"\Library\*.* "%DEST_ROOT%"\SupportAssemblies\Library\ /I /E /Y /EXCLUDE:ArchiveSourceExclude.txt >> "%LOGFILE%"
XCOPY "%SRC_ROOT%"\SP\Library\*.* "%DEST_ROOT%"\SupportAssemblies\SP\Library\ /I /E /Y /EXCLUDE:ArchiveSourceExclude.txt >> "%LOGFILE%"

ECHO ========================================================
ECHO Compressing files...
ECHO Compressing files... >> "%LOGFILE%"
"C:\Program Files\7-Zip\7z.exe" a -r "%DEST_ROOT%"\SPWebSource.7z "%DEST_ROOT%" >> "%LOGFILE%"

ECHO ========================================================
ECHO Deleting copied source files...
ECHO Deleting copied source files... >> "%LOGFILE%"
RMDIR /S /Q "%DEST_ROOT%"\TargetWeb\ >> "%LOGFILE%"
RMDIR /S /Q "%DEST_ROOT%"\SupportAssemblies\ >> "%LOGFILE%"

ECHO ========================================================
ECHO Done.
ECHO ========================================================
ECHO.
PAUSE
