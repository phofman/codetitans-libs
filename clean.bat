@echo off
echo Clean script v1.3 (2010-09-23) /* PH */
echo.

echo ###############################
echo #                             #
echo #   Removing unneeded files   #
echo #                             #
echo ###############################
echo.
echo. 
 
REM ################################
REM # removing global files
REM # with hiding errors:

echo Initializing cleaner...

attrib -h *.suo > NUL: 2> NUL:
del /S /Q *.suo > NUL: 2> NUL:
del /S /Q *.ncb > NUL: 2> NUL:
del /S /Q *.aps > NUL: 2> NUL:
del /S /Q *.user > NUL: 2> NUL:
del /S /Q *.suo > NUL: 2> NUL:

REM ################################
REM # Reading folder names from:
REM #   - 'clean.folders.txt'
REM # and
REM #   removing temporary files:

FOR /F "eol=; delims==" %%i in (clean.folders.txt) do (
 echo Removing temporary files from '%%i'...
 rmdir /S /Q ".\%%i\Bin" 2> NUL:
 rmdir /S /Q ".\%%i\Obj" 2> NUL:
 rmdir /S /Q ".\%%i\Release" 2> NUL:
 rmdir /S /Q ".\%%i\Debug" 2> NUL:
 rmdir /S /Q ".\%%i\test-results" 2> NUL:
 del   /S /Q ".\%%i\*.pidb" 2> NUL: > NUL:
)

echo Removing TestResults...
rmdir /S /Q "TestResults" 2> NUL:

echo.
echo.
echo ###############################
echo #                             #
echo #           Finished          #
echo #                             #
echo ###############################
echo.
echo.

pause