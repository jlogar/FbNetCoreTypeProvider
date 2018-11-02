open FirebirdSql.Data.FirebirdClient
open FSharp.Data.Sql
open FSharp.Data.Sql.Common

let [<Literal>] LocalDb = __SOURCE_DIRECTORY__ + @"\test.FDB"
let [<Literal>] ConnectionString = @"Data Source=localhost;initial catalog=" + LocalDb + @";user id=SYSDBA;password=masterkey;Dialect=3;"
type HR = SqlDataProvider<
            Common.DatabaseProviderTypes.FIREBIRD,
            ConnectionString,
            OdbcQuote = OdbcQuoteCharacter.DOUBLE_QUOTES,
            UseOptionTypes = true
            >

[<EntryPoint>]
let main argv =
    let csb = new FbConnectionStringBuilder()
    csb.Dialect <- 3
    csb.Charset <- "UTF8"
    csb.Database <- LocalDb
    csb.UserID <- "SYSDBA"
    csb.Password <- "masterkey"
    csb.ServerType <- FbServerType.Default
    csb.Role <- ""
    use connection = new FbConnection(csb.ConnectionString)
    connection.Open()
    let cmd = connection.CreateCommand()
    cmd.CommandText<-"SELECT * FROM test"
    let reader = cmd.ExecuteReader()
    printfn "local; HasRows: %A" reader.HasRows
    reader.Read() |> ignore
    let id = reader.GetInt32(0)
    printfn "local; id: %A" id
    reader.Close()
    connection.Close()
    printfn "local connectivity seems ok."

    let ctx = HR.GetDataContext()
    let elDevs = query {
                        for a in ctx.Dbo.Test do
                        select a
                    } |> Seq.toList
    elDevs |> List.map (fun x -> x.Name) |> List.iter (printfn "SQLProvider; name: %A")
    printfn "local connectivity using SqlProvider auch so."
    
    0 // return an integer exit code
