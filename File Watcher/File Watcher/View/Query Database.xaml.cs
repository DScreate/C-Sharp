using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using File_Watcher.ViewModel;

namespace File_Watcher.View
{
    /// <summary>
    /// Interaction logic for Query_Database.xaml
    /// </summary>
    public partial class Query_Database : Window
    {
        public Query_Database()
        {
            InitializeComponent();
        }

        private void ButtonQuerySubmit_OnClick(object sender, RoutedEventArgs e)
        {
            if(ComboBoxQueryInput.Text=="Directory")
                Connector.FillDataGrid(QueryDataGrid, "");
            else if (ComboBoxQueryInput.Text == "Other")
            {
                Connector.FillDataGrid(QueryDataGrid, GetSimpleInput.text);
            }
            else
                Connector.FillDataGrid(QueryDataGrid, ComboBoxQueryInput.Text);
        }

        private void ButtonClearDb_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation",
                System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
                Connector.ClearDb();
        }

        private void ComboBoxItemOther_OnSelected(object sender, RoutedEventArgs e)
        {
            Window win2 = new GetSimpleInput();
            win2.DataContext = this;
            win2.ShowDialog();
        }
    }
}
