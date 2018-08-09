using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

namespace File_Watcher.Model
{
    public class DatabaseHandler
    {
        private static SQLiteConnection _sqliteConn;
        private static SQLiteCommand _sqliteCmd;

        private static FileSystemEventArgs lastEventArgs;
        private static DateTime lastChangeTime;

        private static Queue<SQLiteCommand> Commands = new Queue<SQLiteCommand>();

        public static SQLiteConnection SqliteConn
        {
            get => _sqliteConn;
            set => _sqliteConn = value;
        }

        public static SQLiteCommand SqliteCmd
        {
            get => _sqliteCmd;
            set => _sqliteCmd = value;
        }

        public static SQLiteDataReader SqLiteDataReader
        {
            get => _sqLiteDataReader;
            set => _sqLiteDataReader = value;
        }

        private static SQLiteDataReader _sqLiteDataReader;



        public static SQLiteConnection PullDb(string fileName)
        {

            string baseFolder = AppDomain.CurrentDomain.BaseDirectory;

            Console.WriteLine(@"Now opening " + fileName);
            SQLiteConnectionStringBuilder conString = new SQLiteConnectionStringBuilder();
            conString.DataSource = Path.Combine(baseFolder, Path.GetFileName(fileName) ?? "WatcherLog.db");
            conString.DefaultTimeout = 5000;
            conString.Version = 3;

            //conString.SyncMode = SynchronizationModes.Off;
            //conString.JournalMode = SQLiteJournalModeEnum.Default;
            //conString.PageSize = 65536;
            //conString.CacheSize = 16777216;
            //conString.FailIfMissing = false;
            //conString.ReadOnly = false;

            _sqliteConn = new SQLiteConnection(conString.ConnectionString);


            _sqliteConn.Open();

            _sqliteCmd = _sqliteConn.CreateCommand();

            _sqliteCmd.CommandText = "CREATE TABLE if not exists Events (name varchar(100), path varchar(100)" +
                                     ",ChangeType varchar(500),Time varchar(100),extension varchar(50));";

            _sqliteCmd.ExecuteNonQuery();

            return _sqliteConn;
        }

        public static void ReadDb()
        {

            _sqliteCmd.CommandText = "SELECT * FROM Events";

            _sqLiteDataReader = _sqliteCmd.ExecuteReader();

            while (_sqLiteDataReader.Read()) // Read() returns true if there is still a result line to read
            {
                // Print out the content of the text field:
                Console.Write(_sqLiteDataReader["Name"]);
                Console.Write("@");
                Console.Write(_sqLiteDataReader["Path"]);
                Console.Write(" ");
                Console.Write(_sqLiteDataReader["ChangeType"]);
                Console.Write(" ");
                Console.Write(_sqLiteDataReader["Time"]);
                Console.Write(" ");
                Console.WriteLine(_sqLiteDataReader["Time"]);
            }

            // We are ready, now lets cleanup and close our connection:
            _sqLiteDataReader.Close();
        }

        public static void OnChanged(object source, FileSystemEventArgs e)
        {

            if (lastEventArgs == null)
            {
                lastEventArgs = e;
                lastChangeTime = DateTime.Now;
            }
            else if (e.Name == lastEventArgs.Name && e.FullPath == lastEventArgs.FullPath &&
                     e.ChangeType == lastEventArgs.ChangeType && DateTime.Compare(lastChangeTime, DateTime.Now) < 0.01)
            {
                //Console.WriteLine(@"SPAM EVENT");
                return;
            }
            //if (!Path.HasExtension(e.FullPath))
            if (Path.GetExtension(e.Name).Equals(".db") || Path.GetExtension(e.Name).Equals(".db-journal")) return;
            //Console.WriteLine("====EXTENSION IS: " + Path.GetExtension(e.Name) + "====");

            // Specify what is done when a file is changed, created, or deleted.
            //accessLock = true;
            Console.WriteLine(e.Name + "@" + e.FullPath + " " + e.ChangeType + " " + DateTime.Now + " " + Path.GetExtension(e.Name));
            _sqliteCmd.CommandText = "INSERT INTO Events (name, path, ChangeType, Time, extension) VALUES ('" + e.Name + "', '"
                                     + e.FullPath + "', '" + e.ChangeType + "', '" + DateTime.Now + "', '" + Path.GetExtension(e.Name) + "');";

            Console.WriteLine();
            Commands.Enqueue(_sqliteCmd);

            //_sqliteCmd.ExecuteNonQuery();

        }

        public static void OnRenamed(object source, RenamedEventArgs e)
        {
            if (Path.GetExtension(e.Name).Equals(".db") || Path.GetExtension(e.Name).Equals(".db-journal")) return;
            //Console.WriteLine("====EXTENSION IS: " + Path.GetExtension(e.Name) + "====");

            // Specify what is done when a file is renamed.
            //accessLock = true;
            Console.WriteLine(e.Name + @"@{0} renamed to {1} {2} {3}", e.OldFullPath, e.FullPath, DateTime.Now, Path.GetExtension(e.Name));
            _sqliteCmd.CommandText = "INSERT INTO Events (name, path, ChangeType, Time, extension) VALUES ('" + e.Name + "', '"
                                     + e.FullPath + "', 'renamed from " + e.OldFullPath + "', '" + DateTime.Now + "', '" + Path.GetExtension(e.Name) + "');";
            Console.WriteLine();
            Commands.Enqueue(_sqliteCmd);
            //_sqliteCmd.ExecuteNonQuery();

        }

        public static void WriteDBFromQueue()
        {
            while (Commands.Count > 0)
            {
                SQLiteCommand cmd = Commands.Dequeue();
                cmd.ExecuteNonQuery();
            }
        }

        public static void ClearQueue()
        {
            Commands.Clear();
        }

        public static bool HasQueue()
        {
            return Commands.Count > 0;
        }

        public static void QueryByExtension(string extension)
        {
            _sqliteCmd.CommandText = "SELECT * FROM Events WHERE extension IS \"" + extension + "\"";
            _sqliteCmd.ExecuteNonQuery();
        }

        public static void FillDataGridByExtension(DataGrid dg, string extension)
        {
            if(extension == "*")
                _sqliteCmd.CommandText = "SELECT * FROM Events";
            else
            {
                _sqliteCmd.CommandText = "SELECT * FROM Events WHERE extension IS \"" + extension + "\"";
            }

            _sqliteCmd.ExecuteNonQuery();
            SQLiteDataAdapter sda = new SQLiteDataAdapter(_sqliteCmd);
            DataTable dt = new DataTable("Events");
            sda.Fill(dt);
            dg.ItemsSource = dt.DefaultView;
        }

        public static void ClearDatabase()
        {
            _sqliteCmd.CommandText = "CREATE TABLE if not exists Events (name varchar(100), path varchar(100)" +
                                     ",ChangeType varchar(500),Time varchar(100),extension varchar(50));";

            _sqliteCmd.ExecuteNonQuery();

            _sqliteCmd.CommandText = "DELETE FROM Events";
                _sqliteCmd.ExecuteNonQuery();


        }
    }
}