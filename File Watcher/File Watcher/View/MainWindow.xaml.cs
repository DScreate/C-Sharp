using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using File_Watcher.View;
using File_Watcher.ViewModel;

namespace File_Watcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private string _currentDB = "WatcherLog.db";
        //private readonly Connector vmConnector;
        private bool IsWatching;

        public MainWindow()
        {
            InitializeComponent();
            //vmConnector = new Connector();
            Closing += OnClosing;
            TextWriter writer = new SmallConsole(TbConsole);
            Console.SetOut(writer);
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (Connector.HasDBQueue())
            {
                // Configure the message box to be displayed
                string messageBoxText = "Do you want to save changes to the database?";
                string caption = "Exit Warning";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;

                // Display message box
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);
                if (result == MessageBoxResult.Yes)
                    WriteTempDB();
                else
                    EmptyTempDB();
            }
        }

        private void MainMenu_File_NewClick(object sender, RoutedEventArgs e)
        {

        }

        private void MainMenu_About_HelpClick(object sendt, RoutedEventArgs e)
        {
            string messageBoxText =
                "File has Controls for Loading and Saving a Database in addition to Access to the Query Interface\n" +
                "The Arrow Begins Watching. The Minus Ends Watching. The Down Arrow Will Write what has been Detected to the Database\n" +
                "Note: The Database will always be stored locally within the same directory as the exe file. This was done to avoid access permission issues";
            string caption = "Quick Help";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Question;

            MessageBox.Show(messageBoxText, caption, button, icon);
        }

        private void MainMenu_About_VersionClick(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "Version 1.1";
            string caption = "Version Number";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Question;

            MessageBox.Show(messageBoxText, caption, button, icon);
        }

        private void MainMenu_About_CreditsClick(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "All Work Original\nAuthor: Derek Sams";
            string caption = "Credits";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Question;

            MessageBox.Show(messageBoxText, caption, button, icon);
        }

        private void MainMenu_File_QueryClick(object sender, RoutedEventArgs e)
        {
            OpenDbForm();
        }

        private void ToolbarMenu_StartClick(object sender, RoutedEventArgs e)
        { 
            StartWatching();
        }

        private void ToolbarMenu_StopClick(object sender, RoutedEventArgs e)
        {

            StopWatching();
        }

        private void ToolBarMenu_WriteClick(object sender, RoutedEventArgs e)
        {
            WriteTempDB();
        }

        private void MainMenu_File_LoadClick(object sender, RoutedEventArgs e)
        {
            Connector.OpenFile();
        }

        // TODO: Implement this
        private void MainMenu_File_SaveClick(object sender, RoutedEventArgs e)
        {
            Connector.SaveFile();
        }

        private void BtnWatch_OnClick(object sender, RoutedEventArgs e)
        {
            if (Connector.IsRunning)
            {
                StopWatching();
            }
            else
            {
                StartWatching();
            }
        }

        private void ComboBoxItemDirChoice1_OnSelected(object sender, RoutedEventArgs e)
        {
            Connector.SelectDirectory();
        }

        private void ComboBoxItemDirChoice2_OnSelected(object sender, RoutedEventArgs e)
        { 
            Window win2 = new InputDirectory();
            win2.DataContext = this;
            win2.ShowDialog();
        }

        private void ComboBoxItemExtChoice1_OnSelected(object sender, RoutedEventArgs e)
        {
            Connector.UpdateFilter("*");
        }

        private void ComboBoxItemExtChoice2_OnSelected(object sender, RoutedEventArgs e)
        {
            Connector.UpdateFilter("*.txt");
        }

        private void ComboBoxItemExtChoice3_OnSelected(object sender, RoutedEventArgs e)
        {
            Connector.UpdateFilter("*.pdf");
        }

        private void ComboBoxItemExtChoice4_OnSelected(object sender, RoutedEventArgs e)
        {
            Connector.UpdateFilter("*.cs");
        }

        private void ComboBoxItemExtChoice5_OnSelected(object sender, RoutedEventArgs e)
        {
            Connector.UpdateFilter("*.bin");
        }

        private void ComboBoxItemExtChoice6_OnSelected(object sender, RoutedEventArgs e)
        {
            Window win4 = new GetSimpleInput();
            win4.DataContext = this;
            win4.ShowDialog();
            Connector.UpdateFilter("*"+GetSimpleInput.text);
        }

        private void StartWatching()
        {
            ButtonToolbarMenuStart.IsEnabled = false;
            ButtonToolbarMenuStartImage.Source = new BitmapImage(new Uri("../View/Images/arrow_right.ico", UriKind.RelativeOrAbsolute));
            ButtonToolbarMenuStopImage.Source = new BitmapImage(new Uri("../View/Images/minus red.png", UriKind.RelativeOrAbsolute));

            ComboBoxDirectoryInput.IsEnabled = false;
            ComboBoxExtensionInput.IsEnabled = false;

            ButtonToolbarMenuStop.IsEnabled = true;
            BtnWatchText.Text = "Stop";

            Connector.Start();
        }

        private void StopWatching()
        {
            ButtonToolbarMenuStart.IsEnabled = true;
            ButtonToolbarMenuStartImage.Source = new BitmapImage(new Uri("../View/Images/arrow_right green.png", UriKind.RelativeOrAbsolute));
            ButtonToolbarMenuStopImage.Source = new BitmapImage(new Uri("../View/Images/minus.ico", UriKind.RelativeOrAbsolute));

            ComboBoxDirectoryInput.IsEnabled = true;
            ComboBoxExtensionInput.IsEnabled = true;

            ButtonToolbarMenuStop.IsEnabled = false;
            BtnWatchText.Text = "Watch";


            Connector.Stop();
        }

        private void ButtonToolbarMenuLoad_OnClick(object sender, RoutedEventArgs e)
        {
            Connector.ConsoleRead();
        }

        private void OpenDbForm()
        {
            Window win2 = new Query_Database();
            win2.DataContext = this;
            win2.ShowDialog();
        }

        private void WriteTempDB()
        {
            Connector.SaveDB();
        }

        private void EmptyTempDB()
        {
            Connector.EmptyDBQueue();
        }
    }
}
