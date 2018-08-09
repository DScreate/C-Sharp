using System;
using System.IO;
using System.Windows;
using static File_Watcher.ViewModel.Connector;

namespace File_Watcher.View
{
    /// <summary>
    /// Interaction logic for Input_Directory.xaml
    /// </summary>
    public partial class InputDirectory
    {        
        public InputDirectory()
        {
            InitializeComponent();
        }

        private void ButtonDirectoryAccept_OnClick(object sender, RoutedEventArgs e)
        {
            var temp = TextBoxDirectoryPath.Text;
            if (Directory.Exists(temp))
            { 
                WatchDirectory = temp;
                LabelDirectoryError.Visibility = Visibility.Hidden;
            }
            else
                LabelDirectoryError.Visibility = Visibility.Visible;
        }
    }
}
