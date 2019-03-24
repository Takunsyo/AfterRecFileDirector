using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace RVMCore
{
    public class Database : IDisposable
    {
        public const string DATABASENAME = "arfdrecorded";
        public const string TABLE_RECORDED = "recorded";
        private readonly Dictionary<string, string> alterTypes = new Dictionary<string, string>()
            {
                { "id" ,"CHAR(24) NOT NULL"},
                { "time" ,"BIGINT(20) UNSIGNED NULL DEFAULT NULL"},
                { "name" ,"TEXT NULL COLLATE 'utf8_unicode_ci'"},
                { "path" ,"TEXT NULL COLLATE 'utf8_unicode_ci'"},
                { "isuploaded" ,"TINYINT(1) UNSIGNED NULL DEFAULT '0'"},
                { "showonuploader" ,"TINYINT(1) UNSIGNED NULL DEFAULT '1'"},
                { "uploadid" ,"CHAR(128) NULL DEFAULT NULL COLLATE 'utf8_unicode_ci'"},
                { "upprogress" ,"FLOAT UNSIGNED NULL DEFAULT NULL"},
                { "initialfoldername" ,"TEXT NULL COLLATE 'utf8_unicode_ci'"}
            };
        private string serverAddr { get; }
        private string uid { get; }
        private string pwd { get; }
        private int port { get; }

        private MySqlConnection baseConnection;
        /* TODO add support for other databases.*/

        public Database()
        {
            var setting = SettingObj.Read();
            if (!setting.DataBase.Equals("mysql", StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidOperationException("DB setting is wrone!");
            this.serverAddr = setting.DataBase_Addr;
            this.uid = setting.DataBase_User;
            this.pwd = setting.DataBase_Pw;
            this.port = setting.DataBase_Port?? 3306;
            baseConnection = new MySqlConnection($"Server={serverAddr};Uid={uid};Pwd={pwd};Database={DATABASENAME};Port={port};SslMode=Preferred;");

            if (ValidDatabase())
            {
                if (!ValidTable())
                    throw new EntryPointNotFoundException("Invalid DataBase has been used on this session.");
                else
                {
                    baseConnection.Open();
                }
            }
            else if (!CreateDatabase())
                throw new EntryPointNotFoundException("Invalid DataBase has been used on this session.");
            else
            {
                baseConnection.Open();
            }
        }

        public Database(string serverAddr, string uid, string pwd)
        {
            this.serverAddr = serverAddr;
            this.uid = uid;
            this.pwd = pwd;
            this.port = 3306;
            baseConnection = new MySqlConnection($"Server={serverAddr};Uid={uid};Pwd={pwd};Database={DATABASENAME};Port={port};SslMode=Preferred;");

            if (ValidDatabase())
            { 
                if (!ValidTable())
                    throw new EntryPointNotFoundException("Invalid DataBase has been used on this session.");
                else
                {
                    baseConnection.Open();
                }
            }
            else if(!CreateDatabase())
                throw new EntryPointNotFoundException("Invalid DataBase has been used on this session.");
            else
            {
                baseConnection.Open();
            }
        }

        public Database(string serverAddr, string uid, string pwd, int port)
        {
            this.serverAddr = serverAddr;
            this.uid = uid;
            this.pwd = pwd;
            this.port = port;
            baseConnection = new MySqlConnection($"Server={serverAddr};Uid={uid};Pwd={pwd};Database={DATABASENAME};Port={port};SslMode=Preferred;");

            if (ValidDatabase())
            {
                if (!ValidTable())
                    throw new EntryPointNotFoundException("Invalid DataBase has been used on this session.");
                else
                {
                    baseConnection.Open();
                }
            }
            else if (!CreateDatabase())
                throw new EntryPointNotFoundException("Invalid DataBase has been used on this session.");
            else
            {
                baseConnection.Open();
            }
        }

        private bool ValidDatabase()
        {
            var mCon = new MySqlConnection($"Server={serverAddr};Uid={uid};Pwd={pwd};Port={port};SslMode=Preferred;");
            var cmd = new MySqlCommand($@"SHOW DATABASES;", mCon);
            try
            {
                mCon.Open();//Open connection for creating database.
                var result = cmd.ExecuteReader();
                while (result.Read())
                {
                    if (result.GetString(0).Equals(DATABASENAME, StringComparison.CurrentCultureIgnoreCase))
                        return true;
                }
            }
            catch (MySqlException e)
            {
                "Database Error:{0}".ErrorLognConsole(e.Message);
                return false;
            }
            catch (Exception e)
            {
                "Unknow error occured when Validing database : {0}".ErrorLognConsole(e.Message);
                return false;
            }
            finally
            {
                mCon.Close();
                cmd.Dispose();
            }
            return false;
        }

        private bool ValidTable()
        {
            var mCon = new MySqlConnection($"Server={serverAddr};Uid={uid};Pwd={pwd};Database={DATABASENAME};Port={port};SslMode=Preferred;");
            var cmd = new MySqlCommand($@"SHOW TABLES;", mCon);
            try
            {
                mCon.Open();//Open connection for creating database.
                var result = cmd.ExecuteReader();
                bool valid = false;
                while (result.Read())
                {
                    if (result.GetString(0).Equals(TABLE_RECORDED, StringComparison.CurrentCultureIgnoreCase))
                        valid = true;
                }
                if (!valid) return false;
                result.Dispose();
                cmd.Dispose();
                // valid colums.
                cmd = new MySqlCommand($"SHOW COLUMNS FROM `{TABLE_RECORDED}`", mCon);
                result = cmd.ExecuteReader();
                Dictionary<string, bool> hasCols = new Dictionary<string, bool>()
                {
                    { "id" ,false},
                    { "time" ,false},
                    { "name" ,false},
                    { "path" ,false},
                    { "isuploaded" ,false},
                    { "showonuploader" ,false},
                    { "uploadid" ,false},
                    { "upprogress" ,false},
                    { "initialfoldername" ,false}
                };
                Dictionary<string, bool> matchCols = new Dictionary<string, bool>()
                {
                    { "id" ,false},
                    { "time" ,false},
                    { "name" ,false},
                    { "path" ,false},
                    { "isuploaded" ,false},
                    { "showonuploader" ,false},
                    { "uploadid" ,false},
                    { "upprogress" ,false},
                    { "initialfoldername" ,false}
                };
                Dictionary<string, string> types = new Dictionary<string, string>()
                {
                    { "id" ,"CHAR(24)"},
                    { "time" ,"BIGINT(20) UNSIGNED"},
                    { "name" ,"TEXT"},
                    { "path" ,"TEXT"},
                    { "isuploaded" ,"TINYINT(1) UNSIGNED"},
                    { "showonuploader" ,"TINYINT(1) UNSIGNED"},
                    { "uploadid" ,"CHAR(128)"},
                    { "upprogress" ,"FLOAT UNSIGNED"},
                    { "initialfoldername" ,"TEXT"}
                };
                while (result.Read())
                {
                    var key = result.GetString("Field");
                    if (hasCols.ContainsKey(key))
                    {
                        hasCols[key] = true;
                        var type = result.GetString("Type");
                        if (types[key].Equals(type.TrimEnd(), StringComparison.CurrentCultureIgnoreCase))
                            matchCols[key] = true;
                    }
                }
                cmd.Dispose();
                result.Dispose();
                foreach (var i in hasCols)
                {
                    if (!i.Value)
                    {
                        cmd = new MySqlCommand($"ALTER TABLE `{TABLE_RECORDED}` " +
                            $"ADD COLUMN `{i.Key}` {alterTypes[i.Key]};", mCon);
                        var mResult = cmd.ExecuteNonQuery();
                        if (mResult <= 0)
                        {
                            "Could not add colunm {1} on table {0}".ErrorLognConsole(nameof(TABLE_RECORDED), i.Key);
                            return false;
                        }
                        else
                        {
                            "Table '{0}' added!".InfoLognConsole(nameof(TABLE_RECORDED));
                        }
                        cmd.Dispose();
                    }
                }
                foreach (var i in matchCols)
                {
                    if (!i.Value)
                    {
                        cmd = new MySqlCommand($"ALTER TABLE `{TABLE_RECORDED}` " +
                            $"CHANGE COLUMN `{i.Key}` `{i.Key}` {alterTypes[i.Key]};", mCon);
                        var mResult = cmd.ExecuteNonQuery();
                        if (mResult <= 0)
                        {
                            "Could not change table {0} on colunm {1}".ErrorLognConsole(nameof(TABLE_RECORDED), i.Key);
                            return false;
                        }
                        else
                        {
                            "Table '{0}' Updated!".InfoLognConsole(nameof(TABLE_RECORDED));
                        }
                        cmd.Dispose();
                    }
                }
                return true;
            }
            catch (MySqlException e)
            {
                "Database Error:{0}".ErrorLognConsole(e.Message);
                return false;
            }
            catch (Exception e)
            {
                "Unknow error occured when Validing tables : {0}".ErrorLognConsole(e.Message);
                return false;
            }
            finally
            {
                cmd.Dispose();
                mCon.Close();
            }
        }

        private bool CreateDatabase()
        {
            var mCon = new MySqlConnection($"Server={serverAddr};Uid={uid};Pwd={pwd};Port={port};SslMode=Preferred;");
            var cmd = new MySqlCommand($@"CREATE DATABASE IF NOT EXISTS `{DATABASENAME}`;", mCon);
            try
            {
                mCon.Open();//Open connection for creating database.
                var result = cmd.ExecuteNonQuery();
                if (result < 1)
                {//Database allready there or failed to create somehow.
                    "Unable to create database, database allready there, program will proceed.".InfoLognConsole();
                }
                cmd.Dispose();
                cmd = new MySqlCommand($"USE `{DATABASENAME}`;", mCon);
                result = cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = new MySqlCommand($"CREATE TABLE IF NOT EXISTS `{TABLE_RECORDED}` (" +
                                                        $"`id` {alterTypes["id"]}," +
                                                        $"`time` {alterTypes["time"]}," +
                                                        $"`name` {alterTypes["name"]}," +
                                                        $"`path` {alterTypes["path"]}," +
                                                        $"`isuploaded` {alterTypes["isuploaded"]}," +
                                                        $"`showonuploader` {alterTypes["showonuploader"]}," +
                                                        $"`uploadid` {alterTypes["uploadid"]}," +
                                                        $"`upprogress` {alterTypes["upprogress"]}," +
                                                        $"`initialfoldername` {alterTypes["initialfoldername"]}," +
                                                        "PRIMARY KEY (`id`)," +
                                                        "UNIQUE INDEX `id` (`id`)" +
                                                    ")"+
                                                    "COLLATE='utf8_general_ci'" +
                                                    "ENGINE=InnoDB" +
                                                    ";", mCon);
                result = cmd.ExecuteNonQuery();
                if (result < 0)
                    "Could not create table {0}".InfoLognConsole(nameof(TABLE_RECORDED));
                else
                    "Table '{0}' Created!".InfoLognConsole(nameof(TABLE_RECORDED));
            }
            catch (MySqlException e)
            {
                "Database Error:{0}".ErrorLognConsole(e.Message);
                return false;
            }
            catch (Exception e)
            {
                "Unknow error occured when creating database : {0}".ErrorLognConsole(e.Message);
                return false;
            }
            finally
            {
                mCon.Close();
                cmd.Dispose();
            }
            return true;
        }

        /// <summary>
        /// Generate a 24 char long base64 uniqe ID.
        /// </summary>
        public static string GenerateID()=> 
            Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        /// <summary>
        /// Generate a 24 char long base64 uniqe ID from a exist <see cref="Guid"/> object.
        /// </summary>
        public static string GenerateID(Guid id) =>
            Convert.ToBase64String(id.ToByteArray());

        public string AddDataItem(string filePath, string name, long time) =>
            AddDataItem(filePath, name, time, Guid.NewGuid());

        public string AddDataItem(string filePath, string name, long time, string oldFolderName) =>
            AddDataItem(filePath, name, time, Guid.NewGuid(),oldFolderName);

        public string AddDataItem(string filePath,string name,long time ,Guid ID, string oldFolderName = null)
        {
            filePath = filePath.Replace(@"\", @"\\").Replace("'", "\\'");
            name = name.Replace("'", "\\'");
            if (!oldFolderName.IsNullOrEmptyOrWhiltSpace()) oldFolderName = oldFolderName.Replace("'", "\\'");
            string mID = GenerateID(ID);
            var cmd = new MySqlCommand($"INSERT INTO `{TABLE_RECORDED}` " +
                $"(`id`,  `time`,  `name`,  `path`" + (oldFolderName.IsNullOrEmptyOrWhiltSpace() ? "": ", `initialfoldername`") + ") " +
                $"VALUES ('{mID}', '{time}', '{name}', '{filePath}'"+ (oldFolderName.IsNullOrEmptyOrWhiltSpace() ? "" : $", '{oldFolderName}'") + ");", baseConnection);
            try
            {
                var result = cmd.ExecuteNonQuery();
                if (result < 1)
                {
                    "Unable to create record for file '{0}'".ErrorLognConsole(filePath);
                    return null;
                }
                else
                {
                    "Data recorded [{0}]`{1}`".InfoLognConsole(mID, filePath);
                    return mID;
                }
            }
            catch (MySqlException e)
            {
                "Database Error:{0}".ErrorLognConsole(e.Message);
                return null;
            }
            catch (Exception e)
            {
                "Unknow error occured when updating database : {0}".ErrorLognConsole(e.Message);
                return null;
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public bool SetUploadStatus(string id, bool removeFromView, bool status = true)
        {
            var cmd = new MySqlCommand($"UPDATE `{TABLE_RECORDED}` SET " +
                       $"`isuploaded`='{(status ? 1 : 0)}', " +
                       $"`showonuploader`='{(removeFromView ? 1 : 0)}' " +
                       $"WHERE  `id`='{id}';", baseConnection);
            try
            {
                
                var result = cmd.ExecuteNonQuery();
                if (result < 1)
                {
                    "Unable to update record 'id=`{0}`'".ErrorLognConsole(id);
                    return false;
                }
                else
                {
                    "Data updated [id=`{0}`]".InfoLognConsole(id);
                    return true;
                }
            }
            catch (MySqlException e)
            {
                "Database Error:{0}".ErrorLognConsole(e.Message);
                return false;
            }
            catch (Exception e)
            {
                "Unknow error occured when updating database : {0}".ErrorLognConsole(e.Message);
                return false;
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public bool SetUploadStatus(string id, string uploadID, bool status = false)
        {
            var cmd = new MySqlCommand($"UPDATE `{TABLE_RECORDED}` SET " +
                       $"`isuploaded`='{(status ? 1 : 0)}', " +
                       $"`uploadid`='{uploadID}' " +
                       $"WHERE  `id`='{id}';", baseConnection);
            try
            {                
                var result = cmd.ExecuteNonQuery();
                if (result < 1)
                {
                    "Unable to update record 'id=`{0}`'".ErrorLognConsole(id);
                    return false;
                }
                else
                {
                    "Data updated [id=`{0}`]".InfoLognConsole(id);
                    return true;
                }
            }
            catch (MySqlException e)
            {
                "Database Error:{0}".ErrorLognConsole(e.Message);
                return false;
            }
            catch (Exception e)
            {
                "Unknow error occured when updating database : {0}".ErrorLognConsole(e.Message);
                return false;
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public bool SetUploadStatus(string id, float upProgress, bool status = false)
        {
            var cmd = new MySqlCommand($"UPDATE `{TABLE_RECORDED}` SET " +
                    $"`isuploaded`='{(status ? 1 : 0)}', " +
                    $"`upprogress`='{upProgress}' " +
                    $"WHERE  `id`='{id}';", baseConnection);
            try
            {                
                var result = cmd.ExecuteNonQuery();
                if (result < 1)
                {
                    "Unable to update record 'id=`{0}`'".ErrorLognConsole(id);
                    return false;
                }
                else
                {
                    "Data updated [id=`{0}`]".InfoLognConsole(id);
                    return true;
                }
            }
            catch (MySqlException e)
            {
                "Database Error:{0}".ErrorLognConsole(e.Message);
                return false;
            }
            catch (Exception e)
            {
                "Unknow error occured when updating database : {0}".ErrorLognConsole(e.Message);
                return false;
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public bool SetUploadStatus(string id, bool status)
        {
            var cmd = new MySqlCommand($"UPDATE `{TABLE_RECORDED}` SET " +
                    $"`isuploaded`='{(status ? 1 : 0)}', " +
                    $"WHERE  `id`='{id}';", baseConnection);
            try
            {
                var result = cmd.ExecuteNonQuery();
                if (result < 1)
                {
                    "Unable to update record 'id=`{0}`'".ErrorLognConsole(id);
                    return false;
                }
                else
                {
                    "Data updated [id=`{0}`]".InfoLognConsole(id);
                    return true;
                }
            }
            catch (MySqlException e)
            {
                "Database Error:{0}".ErrorLognConsole(e.Message);
                return false;
            }
            catch (Exception e)
            {
                "Unknow error occured when updating database : {0}".ErrorLognConsole(e.Message);
                return false;
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public static void TestMethod(string serverAddr, string uid, string pwd, int port = 3306)
        {
            var mCon = new MySqlConnection($"Server={serverAddr};Uid={uid};Pwd={pwd};Port={port};Database={DATABASENAME};SslMode=Preferred;");
            mCon.Open();

            var cmd = new MySqlCommand($"SELECT `id`,  `time`,  `name`,  `path`,  `isuploaded`,  `showonuploader`, `uploadid`, `upprogress` FROM `{TABLE_RECORDED}` ORDER BY `time` ASC LIMIT 1000",mCon);
            var rowCount = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(rowCount);
            while (rowCount.Read())
            {
                Console.WriteLine(rowCount["name"]);
            }
            mCon.Close();
        }

        public void LoadData(ref DataSet table,DateTime from,DateTime to,bool loadAll = false)
        {
            //var mCon = new MySqlConnection($"Server={serverAddr};Uid={uid};Pwd={pwd};Port={port};Database={DATABASENAME};SslMode=Preferred;");
            var uFrom = MirakurunWarpper.MirakurunService.GetUNIXTimeStamp(from);
            var uTo = MirakurunWarpper.MirakurunService.GetUNIXTimeStamp(to);
            var cmd = new MySqlCommand($"SELECT `id`,  `time`,  `name`,  `path`,  `isuploaded`,  `showonuploader`, `uploadid`, `upprogress` " +
                    $"FROM `{TABLE_RECORDED}` " +
                    $"WHERE `time` BETWEEN '{uFrom}' AND '{uTo}'" + 
                    (loadAll ? "" : "AND `showonuploader` = '1' ") +
                    $"ORDER BY `time` ASC LIMIT 1000", baseConnection);
            try
            {
                using(var adapter = new MySqlDataAdapter(cmd))
                {
                    table.Clear();
                    adapter.Fill(table);
                }
            }
            catch (MySqlException e)
            {
                "Database Error:{0}".ErrorLognConsole(e.Message);
                return;
            }
            catch (Exception e)
            {
                "Unknow error occured when reading database : {0}".ErrorLognConsole(e.Message);
                return;
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public IEnumerable<RmtFile> LoadData(bool loadAll = false)
        {
            var mCon = new MySqlConnection($"Server={serverAddr};Uid={uid};Pwd={pwd};Port={port};Database={DATABASENAME};SslMode=Preferred;");
            var cmd = new MySqlCommand($"SELECT `id`,  `time`,  `name`,  `path`,  `isuploaded`,  `showonuploader`, `uploadid`, `upprogress`, `initialfoldername` " +
                    $"FROM `{TABLE_RECORDED}` " +
                    (loadAll ? "" : "WHERE `showonuploader` = '1' AND `isuploaded` = '0'") +
                    $"ORDER BY `time` ASC LIMIT 1000", mCon);
            MySqlDataReader result = null;
            try
            {
                mCon.Open();
                result = cmd.ExecuteReader();
            }
            catch (MySqlException e)
            {
                "Database Error:{0}".ErrorLognConsole(e.Message);
                yield break;
            }
            catch (Exception e)
            {
                "Unknow error occured when reading database : {0}".ErrorLognConsole(e.Message);
                yield break;
            }
            string root = SettingObj.Read().StorageFolder;
            while (result.Read())
            {
                var tmp = result["initialfoldername"];
                var item = new RmtFile(System.IO.Path.Combine(root,(string)result["path"]), !(tmp ==DBNull.Value), (tmp == DBNull.Value ? string.Empty:tmp.ToString()), true);
                item.ID = (string)result["id"];
                yield return item;
            }
            cmd.Dispose();
            mCon.Close();
        }

        public string GetFileUploadID(string id)
        {
            //var mCon = new MySqlConnection($"Server={serverAddr};Uid={uid};Pwd={pwd};Port={port};Database={DATABASENAME};SslMode=Preferred;");
            var cmd = new MySqlCommand($"SELECT `uploadid` " +
                    $"FROM `{TABLE_RECORDED}` " +
                    $"WHERE `id` = '{id}';", baseConnection);
            try
            {
                var result = cmd.ExecuteReader();
                while (result.Read())
                {
                    var tmp = result["uploadid"];
                    return (tmp == DBNull.Value ? string.Empty : tmp.ToString());
                }
            }
            catch (MySqlException e)
            {
                "Database Error:{0}".ErrorLognConsole(e.Message);
                return null;
            }
            catch (Exception e)
            {
                "Unknow error occured when reading database : {0}".ErrorLognConsole(e.Message);
                return null;
            }
            finally
            {
                cmd.Dispose();
            }
            return null;
        }

        public bool? GetFileUploadStatus(string id, out string uploadID)
        {
            //var mCon = new MySqlConnection($"Server={serverAddr};Uid={uid};Pwd={pwd};Port={port};Database={DATABASENAME};SslMode=Preferred;");
            var cmd = new MySqlCommand($"SELECT `uploadid`,  `isuploaded` " +
                    $"FROM `{TABLE_RECORDED}` " +
                    $"WHERE `id` = '{id}';", baseConnection);
            uploadID = "";
            try
            {
                var result = cmd.ExecuteReader();
                while (result.Read())
                {
                    bool tmp = result.GetInt16("isuploaded") == 1;
                    var tmp2 = result["uploadid"];
                    uploadID = (tmp2 == DBNull.Value ? string.Empty : tmp2.ToString());
                    if (tmp)
                    {
                        return true;
                    }
                    else if (!uploadID.IsNullOrEmptyOrWhiltSpace())
                    {
                        return null;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (MySqlException e)
            {
                "Database Error:{0}".ErrorLognConsole(e.Message);
                return null;
            }
            catch (Exception e)
            {
                "Unknow error occured when reading database : {0}".ErrorLognConsole(e.Message);
                return null;
            }
            finally
            {
                cmd.Dispose();
            }
            return null;
        }

        public bool SuperLogonCheck(string passWord)
        {
            var cmd = new MySqlCommand($"SELECT COUNT(1) Password_is_OK " +
                $"FROM mysql.user WHERE user='root' " +
                $"AND password=PASSWORD('{passWord}');", baseConnection);
            try
            {
                var result = cmd.ExecuteReader();
                while (result.Read())
                {
                    return result["Password_is_OK"].ToString() == 1.ToString();
                }
                return false;
            }
            catch (MySqlException e)
            {
                "Database Error:{0}".ErrorLognConsole(e.Message);
                return false;
            }
            catch (Exception e)
            {
                "Unknow error occured when updating database : {0}".ErrorLognConsole(e.Message);
                return false;
            }
            finally
            {
                cmd.Dispose();
            }
        }

        public bool DeleteRecord(string id)
        {
            var cmd = new MySqlCommand($"DELETE FROM `{TABLE_RECORDED}` WHERE  `id`='{id}';", baseConnection);
            try
            {
                var result = cmd.ExecuteNonQuery();
                if (result < 1)
                {
                    "Unable to remove record 'id=`{0}`'".ErrorLognConsole(id);
                    return false;
                }
                else
                {
                    "Data removed [id=`{0}`]".InfoLognConsole(id);
                    return true;
                }
            }
            catch (MySqlException e)
            {
                "Database Error:{0}".ErrorLognConsole(e.Message);
                return false;
            }
            catch (Exception e)
            {
                "Unknow error occured when updating database : {0}".ErrorLognConsole(e.Message);
                return false;
            }
            finally
            {
                cmd.Dispose();
            }
        }

        #region IDisposable Support
        public void Dispose()
        {
            baseConnection.Close();
            baseConnection.Dispose();
        }
        #endregion
    }
}
