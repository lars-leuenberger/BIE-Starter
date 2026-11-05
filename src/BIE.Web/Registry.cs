using BIE.Core;
using Microsoft.Data.Sqlite;
using System.Text.Json;

namespace BIE.Web;

public sealed class Registry
{
    readonly string cs;

    public Registry(string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        cs = new SqliteConnectionStringBuilder
        {
            DataSource = path,
            Pooling = false
        }.ToString();
        using var c = Open();
        using var q = c.CreateCommand();
        q.CommandText = "CREATE TABLE IF NOT EXISTS cases(id INTEGER PRIMARY KEY AUTOINCREMENT,payload TEXT NOT NULL)";
        q.ExecuteNonQuery();
    }

    SqliteConnection Open()
    {
        var c = new SqliteConnection(cs);
        c.Open();
        return c;
    }

    public Case Add(Entry e)
    {
        var v = EntryPolicy.Evaluate(e);
        using var c = Open();
        using var q = c.CreateCommand();
        q.CommandText = "INSERT INTO cases(payload) VALUES($p); SELECT last_insert_rowid();";
        q.Parameters.AddWithValue("$p", JsonSerializer.Serialize(new Case(0, e, v)));
        return new((long)q.ExecuteScalar()!, e, v);
    }

    public List<Case> All()
    {
        using var c = Open();
        using var q = c.CreateCommand();
        q.CommandText = "SELECT id,payload FROM cases ORDER BY id DESC";
        using var r = q.ExecuteReader();
        var list = new List<Case>();
        while (r.Read())
            list.Add(JsonSerializer.Deserialize<Case>(r.GetString(1))! with
            {
                Id = r.GetInt64(0)
            });
        return list;
    }
}
