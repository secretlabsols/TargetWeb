
@ECHO OFF
CLS
REM Copies binaries to a known location ready for the MSI installer to be built.
REM The destination directory is created if necessary.
REM Files in the destination directory will be overwritten.

REM History
REM	-------
REM	1	17/11/2008	MikeVO		Initial version.

SET DEST_ROOT=..\..\..\InstallerFiles\AbacusIntranet
SET SRC_TARGET_WEB=..\..\..\TargetWeb
SET SRC_LIBRARY_ABACUS_DLLS=..\..\..\Abacus\Library\AbacusDLLs

ECHO ========================================================
ECHO "%DEST_ROOT%"
ECHO About to delete contents of destination folder.
PAUSE

ECHO ========================================================
ECHO Clearing destination folder...
RMDIR /S /Q "%DEST_ROOT%"

ECHO ========================================================
ECHO Copying binaries...

MKDIR "%DEST_ROOT%"

XCOPY "%SRC_LIBRARY_ABACUS_DLLS%"\Generated\AbacusInterfaces.dll "%DEST_ROOT%"\bin\
XCOPY "%SRC_LIBRARY_ABACUS_DLLS%"\Generated\Interop.AbacusClasses.dll "%DEST_ROOT%"\bin\
XCOPY "%SRC_LIBRARY_ABACUS_DLLS%"\Generated\Interop.AbacusClientServer.dll "%DEST_ROOT%"\bin\
XCOPY "%SRC_LIBRARY_ABACUS_DLLS%"\Generated\Interop.AbacusLibrary.dll "%DEST_ROOT%"\bin\
XCOPY "%SRC_LIBRARY_ABACUS_DLLS%"\Generated\MSXML.dll "%DEST_ROOT%"\bin\
XCOPY "%SRC_LIBRARY_ABACUS_DLLS%"\Generated\Scripting.dll "%DEST_ROOT%"\bin\
XCOPY "%SRC_LIBRARY_ABACUS_DLLS%"\Generated\VBA.dll "%DEST_ROOT%"\bin\
XCOPY "%SRC_LIBRARY_ABACUS_DLLS%"\Generated\adodb.dll "%DEST_ROOT%"\bin\

XCOPY "%SRC_TARGET_WEB%"\*.* "%DEST_ROOT%" /I /S /Y /EXCLUDE:CopyInstallerFilesExclusions.txt
XCOPY "%SRC_TARGET_WEB%"\AbacusWeb\bin\RegisterComDlls.bat "%DEST_ROOT%"\AbacusWeb\bin\
XCOPY "%SRC_TARGET_WEB%"\AbacusWeb\bin\UnRegisterComDlls.bat "%DEST_ROOT%"\AbacusWeb\bin\

DEL "%DEST_ROOT%"\bin\*.xml

ECHO ========================================================
ECHO Done.
PAUSE