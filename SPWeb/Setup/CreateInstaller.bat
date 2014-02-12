
@ECHO OFF
CLS
REM Bundles binaries and web resources only for the Supporting People Web application into a 7zip self-extracting installer.
REM Previous installer will be overwritten.

REM Requires 7-Zip commandline version from http://www.7-zip.org 
REM and extra Professional SFX for modules available 
REM from http://georgwittberger.gmxhome.de/7z/

REM History
REM	-------
REM 3   22/10/2007  MikeVO      .NET 2.0 upgrade.
REM	2	16/03/2007	MikeVO		Copy SP COM DLLs to TargetWeb\bin folder before creating installer.
REM								Added excluded list.
REM	1	15/03/2007	MikeVO		Initial version.

SET SRC_TARGET_WEB_ROOT=C:\Inetpub\wwwroot\TargetWeb
SET SP_COM_DLLS_FOLDER=C:\Program Files\Target Systems\SPLS
SET AJAX_EXTENSIONS_FOLDER=C:\Program Files\Microsoft ASP.NET\ASP.NET 2.0 AJAX Extensions\v1.0.61025
SET LOGFILE=CreateInstaller.log
SET INSTALLER_ARCHIVE=SPWebSetup.7z
SET INSTALLER_EXE=SPWebSetup.exe
SET ORIGINAL_FOLDER=%CD%

ECHO ========================================================
ECHO Deleting previous installer...
DEL "%INSTALLER_ARCHIVE%"
DEL "%INSTALLER_EXE%"
DEL "%LOGFILE%"

ECHO ========================================================
ECHO Preparing to create installer from source folders:
ECHO Preparing to create installer from source folders: >> %LOGFILE%
ECHO "%SRC_TARGET_WEB_ROOT%"
ECHO "%SRC_TARGET_WEB_ROOT%" >> "%LOGFILE%"
ECHO "%SP_COM_DLLS_FOLDER%"
ECHO "%SP_COM_DLLS_FOLDER%" >> "%LOGFILE%"
ECHO "%AJAX_EXTENSIONS_FOLDER%"
ECHO "%AJAX_EXTENSIONS_FOLDER%" >> "%LOGFILE%"

ECHO ========================================================
ECHO Copying SP COM DLLs...
ECHO Copying SP COM DLLs... >> "%LOGFILE%"
XCOPY /Y "%SP_COM_DLLS_FOLDER%\ErrorClass.dll" "%SRC_TARGET_WEB_ROOT%\bin" >> "%LOGFILE%"
XCOPY /Y "%SP_COM_DLLS_FOLDER%\SPFunctions.dll" "%SRC_TARGET_WEB_ROOT%\bin" >> "%LOGFILE%"
XCOPY /Y "%SP_COM_DLLS_FOLDER%\SPClasses.dll" "%SRC_TARGET_WEB_ROOT%\bin" >> "%LOGFILE%"

ECHO ========================================================
ECHO Copying AJAX extension DLLs...
ECHO Copying AJAX extension DLLs... >> "%LOGFILE%"
XCOPY /Y "%AJAX_EXTENSIONS_FOLDER%\System.Web.Extensions.dll" "%SRC_TARGET_WEB_ROOT%\bin" >> "%LOGFILE%"

ECHO ===========================================================
ECHO Compressing installation files...
ECHO Compressing installation files... >> "%LOGFILE%"
CD /D "%SRC_TARGET_WEB_ROOT%"
"C:\Program Files\7-Zip\7z.exe" a "%ORIGINAL_FOLDER%\%INSTALLER_ARCHIVE%" -r0 @"%ORIGINAL_FOLDER%\CreateInstallerFiles.txt" -x@"%ORIGINAL_FOLDER%\CreateInstallerExcludedFiles.txt" >> "%ORIGINAL_FOLDER%\%LOGFILE%"

ECHO ===========================================================
ECHO Creating self-extracting EXE...
ECHO Creating self-extracting EXE... >> "%ORIGINAL_FOLDER%\%LOGFILE%"
CD /D "%ORIGINAL_FOLDER%"
COPY /B "C:\Program Files\7-Zip\7z.sfx" + "%INSTALLER_ARCHIVE%" "%INSTALLER_EXE%" >> "%LOGFILE%"

ECHO ===========================================================
ECHO Deleting compressed archive...
ECHO Deleting compressed archive... >> "%LOGFILE%"
DEL "%INSTALLER_ARCHIVE%" >> "%LOGFILE%"

ECHO ===========================================================
ECHO Done.
ECHO ===========================================================

PAUSE
