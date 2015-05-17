Spartacus is a multi-purpose library written in C#, focused on:
  - Access many types of databases in a generic way;
  - Communication between client and server;
  - Easy to work with cryptography of files and messages;
  - Really fast import and export spreadsheets (CSV and XLSX), with no need of Excel or any other piece of software;
  - Compress and uncompress zip files;
  - Render reports from a unique XML markup to PDF, using own database objects.

Examples of how to use Spartacus are here: https://github.com/wind39/spartacus/wiki

Please note that Spartacus is not API ready yet, something may change until it becomes stable.
Some development effort is tracked in this Trello board: https://trello.com/b/qIQ5id41/spartacus

Spartacus links this libraries:
  - Mono.Data.Sqlite (http://www.mono-project.com/docs/database-access/providers/sqlite/)
  - Firebird .NET Provider (http://www.firebirdsql.org/en/net-provider/)
  - MySQL Connector/NET (http://dev.mysql.com/downloads/connector/net/1.0.html)
  - Npgsql (https://github.com/npgsql/npgsql)
  - Mono Oracle.Data.OracleClient (http://www.mono-project.com/docs/database-access/providers/oracle/)
  
Spartacus reuse code from this projects:
  - Mono open source implementation of Microsoft's .NET Framework (https://github.com/mono/mono)
  - PDFjet Open Source Edition (http://pdfjet.com/os/edition.html)
  - EPPlus (http://epplus.codeplex.com/)
  - SejExcel, from Jose Segarra (https://github.com/jsegarra1971/SejExcelExport)
  - ZipStorer, from Jaime Olivares (http://zipstorer.codeplex.com)

To be able to work with old DBF files, Spartacus uses binaries from this project:
  - dBASE Reader and Converter (http://sourceforge.net/projects/dbf)

I also would like to thank the following people and companies for help testing and suggesting improvements:
  - Rafael Thofehrn Castro (http://www.inf.ufpr.br/rtc10/page.html)
  - Planning Service Transfer Pricing (http://www.planningservice.com.br)
  - Adsistem Sistemas Administrativos (http://www.adsistem.com.br)
