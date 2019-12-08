taskkill /im streamdeck.exe /f
taskkill /im ultimatestreammgr.streamdeck.exe /f
timeout /t 2

del *.streamDeckPlugin 
rmdir com.ultimatestreammgr.integration.sdPlugin\ /s /q
xcopy ..\bin\Debug com.ultimatestreammgr.integration.sdPlugin\ /E /Y
DistributionTool.exe -b -i com.ultimatestreammgr.integration.sdPlugin -o .\
IF %ERRORLEVEL% NEQ 1 (exit 1 ) 
timeout /t 2
rmdir %APPDATA%\Elgato\StreamDeck\Plugins\com.ultimatestreammgr.integration.sdPlugin  /s /q
timeout /t 2
START "" "C:\Program Files\Elgato\StreamDeck\StreamDeck.exe"
timeout /t 2
START "" com.ultimatestreammgr.integration.streamDeckPlugin
timeout /t 2
exit 0