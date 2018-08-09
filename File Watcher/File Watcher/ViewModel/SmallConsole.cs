using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace File_Watcher.ViewModel
{
    public class SmallConsole : TextWriter
    {
        private readonly TextBlock _output = null; //TextBlock used to show Console's output.

        public SmallConsole(TextBlock output)
        {
            _output = output;
        }

        public override void Write(char value)
        {
            base.Write(value);

            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(
                    () => _output.Text += value.ToString(), DispatcherPriority.Normal);
            }
            else
            {

                _output.Text += value.ToString(); 
            }
        

        }


        public override Encoding Encoding => System.Text.Encoding.UTF8;
    }
}