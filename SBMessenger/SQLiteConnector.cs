using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace SBMessenger
{
    class SQLiteConnector
    {
        static string connectionString = "Data Source=messenger.db; Version=3;";
        public static Credentials checkCredentials()
        {
            Credentials result = null;
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SQLiteException) { }
                if (conn.State == ConnectionState.Open)
                {
                    SQLiteCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT user_id, passw, ip, port FROM Credentials ORDER BY timestamp DESC LIMIT 1;";
                    try
                    {
                        SQLiteDataReader r = cmd.ExecuteReader();
                        string line = String.Empty;
                        while (r.Read())
                        {
                            result = new Credentials()
                            {
                                Login = r["user_id"].ToString(),
                                Password = r["passw"].ToString(),
                                Url = r["ip"].ToString(),
                                Port = Convert.ToInt32(r["port"])
                            };
                        }
                        r.Close();
                    }
                    catch (SQLiteException) { }
                }
            }
            return result;
        }
        public static void AddCredentials(Credentials c, DateTime d)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SQLiteException) { }
                if (conn.State == ConnectionState.Open)
                {
                    SQLiteCommand cmd = conn.CreateCommand();
                    string sql_command =
                      "CREATE TABLE IF NOT EXISTS Credentials("
                      + "user_id TEXT PRIMARY KEY, "
                      + "passw TEXT, "
                      + "ip TEXT, "
                      + "port INTEGER, "
                      + "timestamp INTEGER);"

                      + "INSERT INTO Credentials(user_id, passw, ip, port, timestamp) "
                      + "VALUES (@user_id, @passw, @ip, @port, strftime('%s', @timestamp));";


                    cmd.CommandText = sql_command;

                    cmd.Parameters.AddWithValue("@user_id", c.Login);
                    cmd.Parameters.Add(new SQLiteParameter("@passw", c.Password));
                    cmd.Parameters.Add(new SQLiteParameter("@ip", c.Url));
                    SQLiteParameter param = new SQLiteParameter("@port", DbType.Int32);
                    param.Value = (int)c.Port;
                    cmd.Parameters.Add(param);
                    cmd.Parameters.Add("@timestamp", DbType.DateTime).Value = d;
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SQLiteException) { }
                }
            }
        }
    }
}
