using System.Windows;

namespace File_Watcher.View
{
    /// <summary>
    /// Interaction logic for GetSimpleInput.xaml
    /// </summary>
    public partial class GetSimpleInput : Window
    {

        public static string text = "";

        public GetSimpleInput()
        {
            InitializeComponent();
        }

        private void ButtonDirectoryAccept_OnClick(object sender, RoutedEventArgs e)
        {
            text = TextBoxInput.Text;
        }
    }
}
