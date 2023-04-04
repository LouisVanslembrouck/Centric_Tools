using System;
using System.Collections.Generic;
using System.IO;
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
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using static System.Collections.Specialized.BitVector32;
using System.IO.Packaging;
using System.Net.NetworkInformation;


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

            Log("Application started.");
        }

        // Create required directories and files.
        public string outputDir = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Output");
        public string logfile = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Log.txt");
        public static string basePath = @"C:\Program Files(x86)\Centric Retail Solutions\";
        public static string JSONBasePath = @"C:\Centric\backup\transactionprocessing\";
        public static string POSLogBasePath = @"C:\Centric\backup\OBP\";

        // 256 bit encryption key.
        public static string Secretkey = "eThWmZq4t7w!z%C&F)J@NcRfUjXn2r5u";
        // regular expression to match one or more digits
        public static string pattern = @"\d+";


        // Connect to the remote host to download the desired logfiles.
        public void DownloadFile(object sender, RoutedEventArgs e)
        {

            string RemoteHost = HostnameInput.Text;

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
                Log("Created output directory.");
            }

            try
            {

                // Set up session options
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = RemoteHost,
                    UserName = DecryptCredentials(GetCredentials(RemoteHost),Secretkey).Item1,
                    Password = DecryptCredentials(GetCredentials(RemoteHost), Secretkey).Item2
                };

                // Create session instance
                Session session = new Session();

                try
                {
                    // Connect
                    session.Open(sessionOptions);
                    Log($"Connected to {RemoteHost}.");

                    // Download file
                    session.GetFiles(RemoteHost, outputDir).Check();
                    MessageBox.Show("File downloaded successfully.");
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                }
                finally
                {
                    // Disconnect
                    session.Dispose();
                    Log($"Closed the connection to {RemoteHost}.");
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }


        // Log an input string to the logfile.
        public void Log(string input)
        {

            if (!File.Exists(logfile))
            {
                File.Create(logfile);
                Log("Created logfile.");
            }

            using (var w = new StreamWriter(logfile))
            {

                try
                {
                    var newLine = input.ToString();
                    w.WriteLineAsync(string.Concat(DateTime.Now, ": ", newLine));
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }


        // Get the password based on the hostname.
        public static string GetCredentials(string hostname)
        {

            // if hoogvliet, use centric_dev credential
            // otherwise use adminxxxx
            if (hostname.StartsWith("hvwnl"))
            {
                string user = "CENTRIC_DEV";
                string pwd = "syc2hvnl*ved";

                return EncryptCredential(user, pwd, Secretkey);

            } else
            {
                // Use regex to extract numbers from hostname.
                MatchCollection matches = Regex.Matches(hostname, pattern);
                string result = "";

                foreach (Match match in matches)
                {
                    result += match.Value;
                }

                string user = "root";
                string pwd = string.Concat("admin",result.Substring(0,4));

                return EncryptCredential(user, pwd, Secretkey);
            }
        }


        // Encrypt the credentials.
        public static string EncryptCredential(string username, string password, string key)
        {
            byte[] encryptedBytes;
            byte[] plainBytes = Encoding.UTF8.GetBytes(username + ":" + password);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.GenerateIV();

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                }
            }

            return Convert.ToBase64String(encryptedBytes);
        }


        // Decrypt the credentials.
        public static Tuple<string,string> DecryptCredentials(string EncryptedCredentials, string key)
        {
            byte[] encryptedBytes = Convert.FromBase64String(EncryptedCredentials);
            byte[] plainBytes;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.GenerateIV();

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                }
            }

            string[] parts = Encoding.UTF8.GetString(plainBytes).Split(':');
            return new Tuple<string, string>(parts[0], parts[1]);
        }
    }
}
