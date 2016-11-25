#! /bin/bash

./ILRepack.exe /union /ndebug /lib:lib /out:Spartacus.dll ../bin/Release/Spartacus.dll ../lib/*.dll
rm -f ../bin/Release/*.dll
cp Spartacus.dll ../bin/Release/