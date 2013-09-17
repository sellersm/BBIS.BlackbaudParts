if %computername%==CHS6JOSEPHSTY02 goto :JOSEPHST
if %computername%==CHS6CHRISWHI02 goto :CHRISWH
if %computername%==ANOTHER_DEVELOPERS_MACHINE_NAME goto :ANOTHER_DEVELOPERS_POSTBUILD
GOTO :ALWAYS

:JOSEPHST
set target_folder=%infinity_291%\NetCommunity
set source_folder=D:\dev\training\mom-bbis\Blackbaud.CustomFx.ChildSponorship.WebParts

xcopy "%source_folder%\bin\Blackbaud.CustomFx.ChildSponorship.WebParts.dll" "%target_folder%\Bin\" /y /d
::make sure the target folders exist
if not exist "%target_folder%\custom\displays" mkdir "%target_folder%\custom\displays"
if not exist "%target_folder%\custom\editors" mkdir "%target_folder%\custom\editors"

xcopy "%source_folder%\*display*.ascx" "%target_folder%\Custom\displays" /y /d
xcopy "%source_folder%\*edit*.ascx" "%target_folder%\Custom\editors"  /y /d
GOTO :ALWAYS

:CHRISWH
set target_folder=C:\Blackbaud\Instances\OCM_293\NetCommunity
set source_folder=D:\SkyDrive\Projects\Emerging Markets\Clients\21195_Mission of Mercy\Source code\Blackbaud.CustomFx.WrappedParts.WebParts

xcopy "%source_folder%\bin\Blackbaud.CustomFx.WrappedParts.WebParts.dll" "%target_folder%\Bin\" /y /d
::make sure the target folders exist
if not exist "%target_folder%\custom" mkdir "%target_folder%\custom"

xcopy "%source_folder%\*.ascx" "%target_folder%\custom" /y /d

set target_folder=C:\Users\chriswh\Desktop\Mom Deploy\NetCommunity

xcopy "%source_folder%\bin\Blackbaud.CustomFx.WrappedParts.WebParts.dll" "%target_folder%\Bin\" /y /d
::make sure the target folders exist
if not exist "%target_folder%\custom" mkdir "%target_folder%\custom"

xcopy "%source_folder%\*.ascx" "%target_folder%\custom" /y /d
GOTO :ALWAYS

:ANOTHER_DEVELOPERS_POSTBUILD
::copied from original postbuild event, presumably appropriate for Jim Hart's local deploy
copy "D:\MyCode\MissionOfMercy\Blackbaud.CustomFx.ChildSponorship.WebParts\bin\*.*" "C:\Program Files (x86)\Blackbaud\NetCommunity\Bin\"
copy "D:\MyCode\MissionOfMercy\Blackbaud.CustomFx.ChildSponorship.WebParts\*.ascx" "C:\Program Files (x86)\Blackbaud\NetCommunity\Custom\ChildSponsorship\"
copy "D:\MyCode\MissionOfMercy\Blackbaud.CustomFx.ChildSponorship.WebParts\*.ashx" "C:\Program Files (x86)\Blackbaud\NetCommunity\Custom\ChildSponsorship\"
GOTO :ALWAYS

:ALWAYS
::put things here that should always run, for any user
goto :END

:END