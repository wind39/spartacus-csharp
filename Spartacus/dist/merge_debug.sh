#! /bin/bash

dist/ILRepack.exe /union /lib:lib /out:dist/Spartacus.dll bin/Debug/Spartacus.dll lib/*.dll
rm -f bin/Debug/*.dll
cp dist/Spartacus.dll bin/Debug/
