#! /bin/bash

dist/ILRepack.exe /union /ndebug /lib:lib /out:dist/Spartacus.dll bin/Release/Spartacus.dll lib/*.dll
rm -f bin/Release/*.dll
cp dist/Spartacus.dll bin/Release/