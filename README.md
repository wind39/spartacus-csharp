![Spartacus Icon](https://raw.githubusercontent.com/wind39/spartacus/master/Spartacus/icon/spartacus_128x128.png)
[![Stories in Ready](https://badge.waffle.io/wind39/spartacus.svg?label=ready&title=Ready)](http://waffle.io/wind39/spartacus)

Spartacus is a business library written in C#, focused on:
  - Access many types of databases in a fast, easy to use and generic way;
  - Render reports from a unique XML markup to PDF, using own database objects;
  - Really fast import and export spreadsheets (CSV, DBF and XLSX), with no need of Excel or any other piece of software;
  - Easy to work with cryptography of files and messages.

NuGet package: https://www.nuget.org/packages/Spartacus

Examples of how to use Spartacus are here: https://github.com/wind39/spartacus/wiki

Spartacus contains this managed libraries:
  - Mono.Data.Sqlite (http://www.mono-project.com/docs/database-access/providers/sqlite/)
  - Mono.Data.Tds (http://www.mono-project.com/docs/database-access/providers/sqlclient/)
  - Oracle ODP.NET, Managed Driver (https://www.nuget.org/packages/Oracle.ManagedDataAccess/)
  - Firebird .NET Provider (http://www.firebirdsql.org/en/net-provider/)
  - Npgsql (https://github.com/npgsql/npgsql)
  - MySQL Connector/NET (http://dev.mysql.com/downloads/connector/net/1.0.html)
  
Spartacus reuse code from this C# projects:
  - Mono open source implementation of Microsoft's .NET Framework (https://github.com/mono/mono)
  - PDFjet Open Source Edition (http://pdfjet.com/os/edition.html)
  - EPPlus (http://epplus.codeplex.com/)
  - SejExcel, by Jose Segarra (https://github.com/jsegarra1971/SejExcelExport)
  - ZipStorer, by Jaime Olivares (https://github.com/jaime-olivares/zipstorer)
  - FastDBF, by Ahmed Lacevic (https://github.com/SocialExplorer/FastDBF)
  - MultiColumnCombobox, by Nish Nishant (http://www.codeproject.com/Articles/19781/A-data-bound-multi-column-combobox)

Spartacus optionally use this native library:
  - SQLite (https://www.sqlite.org/)

Spartacus DLL is built with Mono C# Compiler (http://www.mono-project.com/docs/about-mono/languages/csharp/), merged with ILRepack (https://github.com/gluck/il-repack) and packaged with NuGet (https://www.nuget.org/).

Spartacus is successfully used by this projects:
  - OmniDB (http://www.omnidb.com.br)

I also would like to thank the following people and companies for help testing and suggesting improvements:
  - Rafael Thofehrn Castro (http://www.rafaelthca.com.br)
  - Planning Service Transfer Pricing (http://www.planningservice.com.br)
  - Adsistem Sistemas Administrativos (http://www.adsistem.com.br)
