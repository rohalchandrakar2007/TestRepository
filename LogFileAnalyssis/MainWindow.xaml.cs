using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Forms;

using System.IO;

namespace LogFileAnalyssis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class Request
    {
        System.Windows.Forms.OpenFileDialog openLogFile = new System.Windows.Forms.OpenFileDialog();
        
        //openLogFile.FileName = "Document"; // Default file name
        //openLogFile.DefaultExt = ".txt"; // Default file extension
        //openLogFile.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension
        public String userName;
        public String visitingPath;
        public String pathTraversed;
        public String timeStamp;
        public String pageLastVisited;
        public int sucessRate;
        
        public String url;                               //from where the client has been redirected to the current page
        public String userAgent;
        public String logFormatType="common";                     // "combined" or "common" 
        public String pageRequestMethod;                   // GET or POST
       

        public Request(String line)
        {
            String[] s = line.Split();
            userName = s[0];
            visitingPath = s[1];
            pathTraversed = s[2];
            timeStamp = s[3] +" "+ s[4];
            pageLastVisited = s[6];
            pageRequestMethod = s[5].TrimStart('\"');           
            sucessRate = Convert.ToUInt16(s[8]);
            if (s[11] != null)
            {
                url = s[10];
                userAgent = s[11];
                logFormatType = "combined";
            }
            else
            {
                url = "";
                userAgent = "";
                logFormatType = "common";
            }
        }

    }


    public partial class MainWindow : Window
    {

        public String filePathString = "";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
           
        }


        //  sample Request format : 199.72.81.55 - - [01/Jul/1995:00:00:01 -0400] "GET /history/apollo/ HTTP/1.0" 200 6245

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (!filePathString.Equals(""))
            {
                StreamReader reader = new StreamReader(filePathString);
                List<Request> requestList = new List<Request>();
                int countLines = 0;
                string line;
                Thread loadingAnimation = new Thread(new ThreadStart(CustomAnimation));
                while ((line = reader.ReadLine()) != null)
                {
                    countLines++;
                    requestList.Add(new Request(line));
                    statusBar.Content = countLines;  // Write to console.
                }
                reader.Close();
                loadingAnimation.Abort();
            }
            else 
            {
                System.Windows.MessageBox.Show("Select any File for Analysis...");
            }

        }

        private void CustomAnimation()
        {
            
        }

        private void bOpenFileDialog_Click(object sender, RoutedEventArgs e)
        {
            Stream checkStream = null;
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Multiselect = false;
            //openFileDialog.InitialDirectory = "c:\\";
            //if you want filter only .txt file
            //dlg.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            //if you want filter all files            
            openFileDialog.Filter = "All Files | *.*";
            if ((bool)openFileDialog.ShowDialog())
            {
                try
                {
                    if ((checkStream = openFileDialog.OpenFile()) != null)
                    {
                        //MyImage.Source = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Absolute));
                        filePath.Text = openFileDialog.FileName;
                        filePathString = openFileDialog.FileName;
                       
                    }
                }
                catch (Exception ex)
                {
                   System.Windows.MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            else
            {
                   System.Windows.MessageBox.Show("Problem occured, try again later");
            }
        }
    }
}
