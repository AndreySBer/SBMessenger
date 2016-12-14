using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
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

        public static void AddMessage(Message m, string current_user)
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
                      "CREATE TABLE IF NOT EXISTS Messages("
                      + "current_user_id TEXT, "
                      + "message_id TEXT PRIMARY KEY,"
                      + "sender_id TEXT, "
                      + "time INTEGER,"
                      + "state TEXT,"
                      + "content_type INT,"
                      + "encrypted INT,"
                      + "data BLOB);"

                      + "INSERT INTO Messages(current_user_id, message_id,sender_id,time,state,content_type,encrypted,data) "
                      + "VALUES (@user_id, @mes_id, @sender_id, strftime('%s', @time), @state, @cont_type, @encr, @data );";


                    cmd.CommandText = sql_command;

                    cmd.Parameters.AddWithValue("@user_id", current_user);
                    cmd.Parameters.Add(new SQLiteParameter("@mes_id", m.MessageId));
                    cmd.Parameters.Add(new SQLiteParameter("@sender_id", m.UserName));
                    cmd.Parameters.Add("@time", DbType.DateTime).Value = m.time;
                    cmd.Parameters.Add(new SQLiteParameter("@state", m.State));
                    SQLiteParameter param = new SQLiteParameter("@cont_type", DbType.Int32);
                    param.Value = (int)m.Type;
                    cmd.Parameters.Add(param);
                    param = new SQLiteParameter("@encr", DbType.Int32);
                    param.Value = m.Encrypted ? 1 : 0;
                    cmd.Parameters.Add(param);
                    param = new SQLiteParameter("@data", DbType.Binary);
                    param.Value = m.Data;
                    cmd.Parameters.Add(param);
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SQLiteException e) { Debug.WriteLine(e); }
                }
            }
        }

        public static Dictionary<string, List<Message>> GetSavedMessages(string current_user)
        {
            Dictionary<string, List<Message>> result = null;
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
                    cmd.CommandText = "SELECT * "
                        + "FROM Messages "
                        //+ "WHERE current_user_id=\""+current_user+"\""
                        //+ "ORDER BY time ASC;";
                        + ";";
                    try
                    {
                        SQLiteDataReader r = cmd.ExecuteReader();
                        
                        while (r.Read())
                        {
                            if (result == null)
                            {
                                result = new Dictionary<string, List<Message>>();
                            }
                            string sender = r["sender_id"].ToString();
                            if (!result.ContainsKey(sender))
                            {
                                result.Add(sender, new List<Message>());
                            }
                            result[sender].Add(new Message(r["message_id"].ToString(),
                                sender,
                                new DateTime(Convert.ToInt32(r["time"]), DateTimeKind.Utc),
                                (MessageContentType)Convert.ToInt32(r["content_type"]),
                                (Convert.ToInt32(r["encrypted"]) == 1),
                                (byte[])(r["data"])
                                )
                            );
                        }
                        r.Close();
                    }
                    catch (SQLiteException e ) { Debug.WriteLine(e); }
                }
            }
            return result;
        }
    }
}
