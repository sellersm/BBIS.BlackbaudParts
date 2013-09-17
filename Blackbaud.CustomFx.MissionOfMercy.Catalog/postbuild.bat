if %computername%==CHS6CHRISWHI02 goto :CHRISWH
if %computername%==ANOTHER_DEVELOPERS_MACHINE_NAME goto :ANOTHER_DEVELOPERS_POSTBUILD
GOTO :ALWAYS

:CHRISWH
set target_folder=C:\Blackbaud\Instances\OCM_293\NetCommunity
set source_folder=D:\SkyDrive\Projects\Emerging Markets\Clients\21195_Mission of Mercy\Source code\Blackbaud.CustomFx.MissionOfMercy.Catalog

xcopy "%source_folder%\bin\debug\Blackbaud.CustomFx.MissionOfMercy.Catalog.dll" "%target_folder%\Bin\" /y /d

set target_folder=C:\Blackbaud\Instances\OCM_293\bbappfx\vroot\bin\custom
xcopy "%source_folder%\bin\debug\Blackbaud.CustomFx.MissionOfMercy.Catalog.dll" "%target_folder%\" /y /d

::to my deploy folder
set target_folder=C:\Users\chriswh\Desktop\Mom Deploy\NetCommunity
set source_folder=D:\SkyDrive\Projects\Emerging Markets\Clients\21195_Mission of Mercy\Source code\Blackbaud.CustomFx.MissionOfMercy.Catalog

xcopy "%source_folder%\bin\debug\Blackbaud.CustomFx.MissionOfMercy.Catalog.dll" "%target_folder%\Bin\" /y /d

set target_folder=C:\Users\chriswh\Desktop\Mom Deploy\bbappfx\vroot\bin\custom
xcopy "%source_folder%\bin\debug\Blackbaud.CustomFx.MissionOfMercy.Catalog.dll" "%target_folder%\" /y /d

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