taskkill /im streamdeck.exe /f
taskkill /im ultimatestreammgr.streamdeck.exe /f
timeout /t 2

del *.streamDeckPlugin 
rmdir com.ultimatestreammgr.integration.sdPlugin\ /s /q
xcopy ..\bin\Debug com.ultimatestreammgr.integration.sdPlugin\ /E /Y
DistributionTool.exe -b -i com.ultimatestreammgr.integration.sdPlugin -o .\

rmdir %APPDATA%\Elgato\StreamDeck\Plugins\com.ultimatestreammgr.integration.sdPlugin  /s /q
START "" "C:\Program Files\Elgato\StreamDeck\StreamDeck.exe"
START "" com.ultimatestreammgr.integration.streamDeckPlugin
exit 0