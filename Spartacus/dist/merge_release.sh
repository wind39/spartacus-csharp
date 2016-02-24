#! /bin/bash

dist/ILRepack.exe /out:dist/Spartacus.dll bin/Release/Spartacus.dll bin/Release/FirebirdSql.Data.FirebirdClient.dll bin/Release/Mono.Data.Sqlite.dll bin/Release/Mono.Data.Tds.dll bin/Release/MySql.Data.dll bin/Release/Npgsql.dll bin/Release/Oracle.ManagedDataAccess.dll bin/Release/Mono.Security.dll
rm -f bin/Release/*.dll
cp dist/Spartacus.dll bin/Release/
