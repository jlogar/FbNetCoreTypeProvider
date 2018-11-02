#load ".paket/load/netcoreapp2.1/main.group.fsx"
#I __SOURCE_DIRECTORY__

open FirebirdSql.Data.FirebirdClient
open System.IO
open System

let [<Literal>] TestDbFile = "test.fdb"

Environment.CurrentDirectory <- Path.GetFullPath(__SOURCE_DIRECTORY__)

let dbFile = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, TestDbFile))

printfn "WARNING: assuming a local Firebird server running."

let csb = new FbConnectionStringBuilder()
csb.Dialect <- 3
csb.Charset <- "UTF8"
csb.Database <- dbFile
csb.UserID <- "SYSDBA"
csb.Password <- "masterkey"
csb.Role <- ""
let connectionString = csb.ConnectionString

//problems here with bad program image means you're not running this with a 64bit fsi
//(the embedded FB server we're using here is 64bit only)
if File.Exists dbFile then
    FbConnection.ClearAllPools();
    FbConnection.DropDatabase(connectionString);
FbConnection.CreateDatabase(connectionString, true)

let createTableSql = @"CREATE TABLE test (ID INTEGER NOT NULL PRIMARY KEY, NAME VARCHAR(255))"

let exec sql =
    use connection = new FbConnection(connectionString)
    connection.Open()
    use tx = connection.BeginTransaction()
    let executeCmd text =
        printfn "%s" text
        let cmd = new FbCommand(text, connection, tx)
        cmd.ExecuteNonQuery()
    executeCmd sql |> ignore
    tx.Commit()
    connection.Close()

exec createTableSql
exec "INSERT INTO test VALUES (1, 'name 1')"
exec "INSERT INTO test VALUES (2, 'name 2')"

printfn "done."
