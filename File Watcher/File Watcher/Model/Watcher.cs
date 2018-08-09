using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using File_Watcher.Annotations;

namespace File_Watcher.Model
{
    public class Watcher : INotifyPropertyChanged
    {
        private string _dbPath;
        private bool _isRunning;
        private static string _filter = "*";

        public String Filter
        {
            get => _filter;
            set => SetField(ref _filter, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetField(ref _isRunning, value);
        }

        public SQLiteConnection SqliteConn
        {
            get => _sqliteConn;
            set => SetField(ref _sqliteConn, value);
        }

        public FileSystemWatcher FileWatcher
        {
            get => _watcher;
            set => SetField(ref _watcher, value);
        }

        private string _watchDirectory;
        private SQLiteConnection _sqliteConn;
        private FileSystemWatcher _watcher;

        public Watcher(string dbPath, string directory)
        {
            _dbPath = dbPath;
            _watchDirectory = directory;
        }
        public void Start()
        {
            Console.WriteLine(@"Now trying to start. dbPath is: " + _dbPath);
            SetupConnection(_dbPath);
            Run();
        }

        public void Stop()
        {
            // Add event handlers.
            _watcher.Changed -= DatabaseHandler.OnChanged;
            _watcher.Created -= DatabaseHandler.OnChanged;
            _watcher.Deleted -= DatabaseHandler.OnChanged;
            _watcher.Renamed -= DatabaseHandler.OnRenamed;

            // Begin watching.
            _watcher.EnableRaisingEvents = false;

            IsRunning = false;
        }

        private void SetupConnection(string fileName)
        {
            _sqliteConn = DatabaseHandler.PullDb(fileName);
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private void Run()
        {
            Console.WriteLine("Now watching {0} and subdirectories with filter {1}", _watchDirectory, _filter);

            // Create a new FileSystemWatcher and set its properties.
            _watcher = new FileSystemWatcher
            {
                Path = _watchDirectory,
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite |
                               NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Filter = _filter
            };
            

            // Watch all files and directories
            _watcher.IncludeSubdirectories = true;

            // Add event handlers.
            _watcher.Changed += DatabaseHandler.OnChanged;
            _watcher.Created += DatabaseHandler.OnChanged;
            _watcher.Deleted += DatabaseHandler.OnChanged;
            _watcher.Renamed += DatabaseHandler.OnRenamed;

            // Begin watching.
            _watcher.EnableRaisingEvents = true;


            //SQLiteDataReader sqlite_datareader;
            IsRunning = true;


        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}