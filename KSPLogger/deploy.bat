rem
set H=R:\KSP_1.1.4_dev
echo %H%

set d=%H%
if exist %d% goto one
mkdir %d%
:one
set d=%H%\Gamedata
if exist %d% goto two
mkdir %d%
:two
set d=%H%\Gamedata\KSPLogger
if exist %d% goto three
mkdir %d%
:three

copy bin\Debug\KSPLogger.dll %H%\Gamedata\KSPLogger
copy KSPLogger.cfg %H%\Gamedata\KSPLogger
copy KSPLogger.version %H%\Gamedata\KSPLogger
copy MiniAVC.dll %H%\Gamedata\KSPLogger