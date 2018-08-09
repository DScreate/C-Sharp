using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Controls;
using File_Watcher.Model;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace File_Watcher.ViewModel
{
    public static class Connector
    {
        private static string _dbPath = "..\\EventLogs\\WatcherLog.db";
        public static String DbPath
        {
            get => _dbPath;
            set
            {
                _dbPath = value;
                Console.WriteLine(@"New database path is: " + _dbPath);
            }
        }

        private static string _watchDirectory = @"C:\";
        private static Watcher _watcher;

        public static String WatchDirectory
        {
            get => _watchDirectory;
            set
            {
                _watchDirectory = value;
                Console.WriteLine(@"Now watching: " + _watchDirectory);
            }
        }

        public static bool IsRunning = false;

        public static void MakeWatcher()
        {
            _watcher = new Watcher(DbPath, WatchDirectory);

        }
        public static void Start()
        {
            MakeWatcher();
            _watcher.Start();
            IsRunning = true;
        }

        public static void Stop()
        {
            _watcher.Stop();
            //_watcher = null;
            IsRunning = false;
        }

        public static void SelectDirectory()
        {

            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                InitialDirectory = WatchDirectory,
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                WatchDirectory = dialog.FileName;
            }
        }

        public static void OpenFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                DefaultExt = ".db",
                Filter = "SQLLite Database (*.db)|*.db",
                // This is just to provide a convenience for the user, it sets the directory the loader opens up as the directory with the db in it
                // InitialDirectory = WatchDirectory
            };
            if (openFileDialog.ShowDialog() == true)
            {
                DbPath = openFileDialog.FileName;
            }
        }

        public static void SaveDB()
        {
            DatabaseHandler.WriteDBFromQueue();
        }

        public static void EmptyDBQueue()
        {
            DatabaseHandler.ClearQueue();
        }

        public static bool HasDBQueue()
        {
            return DatabaseHandler.HasQueue();
        }

        // TODO: Implement this!
        public static void SaveFile()
        {
            var saveFileDialog = new SaveFileDialog() { Filter = "SQLLite Database (*.db)|*.db" };
            if (saveFileDialog.ShowDialog() == true)
            {
                DbPath = saveFileDialog.FileName;
            }
        }

        public static void ConsoleRead()
        {
            if (DatabaseHandler.SqliteConn == null)
                DatabaseHandler.SqliteConn = DatabaseHandler.PullDb(Path.GetFileName(DbPath));
            DatabaseHandler.ReadDb();
        }

        public static void ClearDb()
        {
            if (DatabaseHandler.SqliteConn == null)
                DatabaseHandler.SqliteConn = DatabaseHandler.PullDb(Path.GetFileName(DbPath));
            DatabaseHandler.ClearDatabase();
        }

        public static void QueryByExtension(string extension = "*")
        {
            if (DatabaseHandler.SqliteConn == null)
                DatabaseHandler.SqliteConn = DatabaseHandler.PullDb(Path.GetFileName(DbPath));
            DatabaseHandler.QueryByExtension(extension);
        }

        public static void FillDataGrid(DataGrid dg, string extension)
        {
            if (DatabaseHandler.SqliteConn == null)
                DatabaseHandler.SqliteConn = DatabaseHandler.PullDb(Path.GetFileName(DbPath));
            
            DatabaseHandler.FillDataGridByExtension(dg, extension);
        }

        public static void UpdateFilter(String filter)
        {
            if(_watcher == null)
                MakeWatcher();
            Console.WriteLine(@"Now watching files with extension of " + filter);
            _watcher.Filter = filter;
        }
    }
}

