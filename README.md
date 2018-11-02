# FbNetCoreTypeProvider

Just a test project for testing out .Net Core with [Firebird](https://firebirdsql.org/) &amp; [SQLProvider](http://fsprojects.github.io/SQLProvider/)

see also http://fsprojects.github.io/SQLProvider/core/netstandard.html

## running

- generate load scripts for initidb.fsx `.paket\paket.exe generate-load-scripts -f netcoreapp2.1 -t fsx -g main`
- init the db by running `fsi initdb.fsx`.
- run with `dotnet run`

No errors should occur.