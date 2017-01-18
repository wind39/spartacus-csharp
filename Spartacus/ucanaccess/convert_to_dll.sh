#! /bin/bash

rm -f UCanAccess.jar UCanAccess.dll

/opt/java/jdk1.8.0_60/bin/jar -xvf commons-lang-2.6.jar
/opt/java/jdk1.8.0_60/bin/jar -xvf commons-logging-1.1.1.jar
/opt/java/jdk1.8.0_60/bin/jar -xvf hsqldb.jar
/opt/java/jdk1.8.0_60/bin/jar -xvf jackcess-2.1.6.jar
/opt/java/jdk1.8.0_60/bin/jar -xvf ucanaccess-4.0.0.jar

/opt/java/jdk1.8.0_60/bin/jar -cvf UCanAccess.jar .

rm -rf com org net META-INF

../ikvm/ikvmc.exe -target:library UCanAccess.jar
