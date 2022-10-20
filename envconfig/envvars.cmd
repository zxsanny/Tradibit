@ECHO OFF

echo Check admin rights...
net session >nul 2>&1
if %errorLevel% NEQ 0 (
	echo Please Run as Administrator!
	PAUSE > NUL
	exit
)
echo Cool! I'm admin, let's start!

rem This is test crypto keys. Set your own private keys separately
setx /M TradibitAesKey "tradibit.sd=kfjvhg sld*kjgn^ksajhf lasjdrg_sdkvjhn.key"
setx /M TradibitAesIv "tradibit.dsfbklhsdgfvk jlabsk,fjabhsdoil vbwefkgbwerklfhbdsfl"
pause