#! /bin/bash

./ILRepack.exe /union /lib:lib /out:Spartacus.dll ../bin/Debug/Spartacus.dll ../lib/*.dll
rm -f ../bin/Debug/*.dll
cp Spartacus.dll ../bin/Debug/
