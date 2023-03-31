using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinSCP;
using static System.Collections.Specialized.BitVector32;

namespace LogCollector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            // Get the remote file path and local file path from the text boxes
            string remoteFilePath = remoteFilePathTextBox.Text;
            string localFilePath = localFilePathTextBox.Text;

            try
            {
                // Set up session options
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = "example.com",
                    UserName = "username",
                    Password = "password"
                };

                // Create session instance
                Session session = new Session();

                try
                {
                    // Connect
                    session.Open(sessionOptions);

                    // Download file
                    session.GetFiles(remoteFilePath, localFilePath).Check();
                    MessageBox.Show("File downloaded successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    // Disconnect
                    session.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
