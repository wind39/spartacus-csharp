#! /bin/bash

dist/ILRepack.exe /out:dist/Spartacus.dll bin/Debug/Spartacus.dll bin/Debug/FirebirdSql.Data.FirebirdClient.dll bin/Debug/Mono.Data.Sqlite.dll bin/Debug/Mono.Data.Tds.dll bin/Debug/MySql.Data.dll bin/Debug/Npgsql.dll bin/Debug/Oracle.ManagedDataAccess.dll
rm -f bin/Debug/*.dll
cp dist/Spartacus.dll bin/Debug/
