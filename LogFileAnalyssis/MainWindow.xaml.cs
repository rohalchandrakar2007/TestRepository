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
using Finisar.SQLite;
using System.IO;
using System.Data;
using System.Collections;

namespace LogFileAnalyssis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class Session
    {
        public int sessionId;
        public List<int> requestIdlist = new List<int>();

        /* features variables for the session class */
        public long totalNoOfPagesRequestedInSession;
        public long noOfImagePagesRequestedInSession;
        public long noOfBinaryDocumentsRequestedInSession;
        public long noOfBinaryExeFileRequestedInSession;
        public long noOfHTMLFileRequestedInSession;
        public long noOfAsciiFilerequestedInSession;
        public long noOfCompressedFileRequestedInSession;
        public long noOfMultimediaFileRequestedInSession;
        public long noOfOtherFileFormatRequestedInSession;
        public DateTime totalTimeOfTheSession;
        public DateTime avgTimeBetweenTowHTMLRequests;
        public DateTime standardDeviationOfTimeBetweenRequests;
        public long noOfPagesRequestedInNightTime;
        public long noOfRequestReapted;
        public long onOfrequestesWithErrors;
        public long noOfRequestWithGETMethod;
        public long noOfRequestWithPOSTMethod;
        public long noOfRequestWithOtherMethod;
        public long widthOfTheTraversal;
        public long depthOfTheTraversal;
        public bool isMultipleIPSEssion;
        public bool isMultiAgentSession;

        /* Features for classifiying the Session */
        public bool isRobotstxtVisited;
        public long noOfRequestWithHEADMethod;
        public long noOfRequestWithUnassignedReferer;

        public Session(int id)
        {
            sessionId = id;
            totalNoOfPagesRequestedInSession = 0;
            noOfImagePagesRequestedInSession = 0;
            noOfBinaryDocumentsRequestedInSession = 0;
            noOfBinaryExeFileRequestedInSession = 0;
            noOfHTMLFileRequestedInSession = 0;
            noOfAsciiFilerequestedInSession = 0;
            noOfCompressedFileRequestedInSession = 0;
            noOfMultimediaFileRequestedInSession = 0;
            noOfOtherFileFormatRequestedInSession = 0;
            totalTimeOfTheSession = 0;
            avgTimeBetweenTowHTMLRequests = 0;
            standardDeviationOfTimeBetweenRequests = 0;
            noOfPagesRequestedInNightTime = 0;
            noOfRequestReapted = 0;
            onOfrequestesWithErrors = 0;
            noOfRequestWithGETMethod = 0;
            noOfRequestWithPOSTMethod = 0;
            noOfRequestWithOtherMethod = 0;
            widthOfTheTraversal = 0;
            depthOfTheTraversal = 0;
            isMultipleIPSEssion = false;
            isMultiAgentSession = false;
            isRobotstxtVisited = false;
            noOfRequestWithHEADMethod = 0;
            noOfRequestWithUnassignedReferer = 0;
        }
    }

    public class Request
    {
        System.Windows.Forms.OpenFileDialog openLogFile = new System.Windows.Forms.OpenFileDialog();
        
        //openLogFile.FileName = "Document"; // Default file name
        //openLogFile.DefaultExt = ".txt"; // Default file extension
        //openLogFile.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension
        public String userName;
        public String visitingPath;
        public String pathTraversed;
        public DateTime timeStamp;
        public String pageLastVisited;
        public String sucessRate;
        
        public String url;                               //from where the client has been redirected to the current page
        public String userAgent;
        public String logFormatType="common";                     // "combined" or "common" 
        public String pageRequestMethod;                   // GET or POST

        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DB;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();


        public Request(String line)
        {
            String[] s = line.Split();
            if (s.Length > 8)
            {
                userName = s[0];
                visitingPath = s[1];
                pathTraversed = s[2];
                timeStamp = DateTime.Parse(dateParseBritishFormat(s[3]));// +" " + s[4];
                
                pageLastVisited = s[6];
                pageRequestMethod = s[5].TrimStart('\"');
                sucessRate = s[8];
                if (s.Length > 11)
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
              //  Add();
            }
        }
        private String dateParse(String dateTime)
        {
            //dateTime = dateTime.TrimStart();
            if (dateTime.Contains("Jan"))
                return "01/"+dateTime.Substring(1,2)+dateTime.Substring(7,5)+" "+dateTime.Substring(13,8);
            if (dateTime.Contains("Feb"))
                return "02/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Mar"))
                return "03/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Apr"))
                return "04/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("May"))
                return "05/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Jun"))
                return "06/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Jul"))
                return "07/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Aug"))
                return "08/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Sep"))
                return "09/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Oct"))
                return "10/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Sep"))
                return "11/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Dec"))
                return "12/" + dateTime.Substring(1, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            return dateTime.Substring(4,2) + dateTime.Substring(1, 2) + dateTime.Substring(6, 5) + " " + dateTime.Substring(12, 8);
        }
        private String dateParseBritishFormat(String dateTime)
        {
            //dateTime = dateTime.TrimStart();
            if (dateTime.Contains("Jan"))
                return dateTime.Substring(1, 3) + "01" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Feb"))
                return dateTime.Substring(1, 3) + "02" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Mar"))
                return dateTime.Substring(1, 3) + "03" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Apr"))
                return dateTime.Substring(1, 3) + "04" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("May"))
                return dateTime.Substring(1, 3) + "05" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Jun"))
                return dateTime.Substring(1, 3) + "06" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Jul"))
                return dateTime.Substring(1, 3) + "07" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Aug"))
                return dateTime.Substring(1, 3) + "08" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Sep"))
                return dateTime.Substring(1, 3) + "09" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Oct"))
                return dateTime.Substring(1, 3) + "10" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Sep"))
                return dateTime.Substring(1, 3) + "11" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            if (dateTime.Contains("Dec"))
                return dateTime.Substring(1, 3) + "12" + dateTime.Substring(7, 5) + " " + dateTime.Substring(13, 8);
            return dateTime.Substring(1, 3) + dateTime.Substring(4, 2) + dateTime.Substring(7, 5) + " " + dateTime.Substring(12, 8);
        }
        private void SetConnection()
        {
            sql_con = new SQLiteConnection
                ("Data Source=RequestTable;Version=3;New=False;Compress=True;");
        }
        private void ExecuteQuery(string txtQuery)
        {
            try
            {
                SetConnection();
                sql_con.Open();
                sql_cmd = sql_con.CreateCommand();
                sql_cmd.CommandText = txtQuery;
                sql_cmd.ExecuteNonQuery();
                sql_con.Close();
            }
            catch(Exception e)
            {}
            }
        private void LoadData()
        {
            //SetConnection();
            //sql_con.Open();
            //sql_cmd = sql_con.CreateCommand();
            //string CommandText = "select id, desc from mains";
            //DB = new SQLiteDataAdapter(CommandText, sql_con);
            //DS.Reset();
            //DB.Fill(DS);
            //DT = DS.Tables[0];
            //Grid.DataSource = DT;
            //sql_con.Close();
        }
        private void Add()
{
    int i = 9;
    String sa = "Hi";
    string txtSQLQuery = "insert into  test values ('"+i.ToString()+"','"+sa+"')";
ExecuteQuery(txtSQLQuery);            
}
    }


    public partial class MainWindow : Window
    {

        public String filePathString = "";
        Hashtable sessionNametoIdhashtable = new Hashtable();
        List<Session> session = new List<Session>();
        List<Request> requestList = new List<Request>();
         int countLines = 0;
                int sessionId =0,tempInt=0;
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
                
               
                string line;
                Thread loadingAnimation = new Thread(new ThreadStart(CustomAnimation));
                while ((line = reader.ReadLine()) != null)
                {
                    //if(countLines==0)
                    //statusBar.Content = line;
                    String[] s = line.Split();
                    requestList.Add(new Request(line));
                   
                    if (!sessionNametoIdhashtable.Contains(requestList[countLines].userName + " " + requestList[countLines].userAgent))
                    {
                        sessionNametoIdhashtable.Add(requestList[countLines].userName + " " + requestList[countLines].userAgent , sessionId);
                        session.Add(new Session(sessionId));
                        session[sessionId].requestIdlist.Add(countLines);
                        sessionId++;
                    }
                    else
                    {
                        session[(int)sessionNametoIdhashtable[requestList[countLines].userName + " " + requestList[countLines].userAgent]].requestIdlist.Add(countLines);
                    }
                    lable_tatalPages_Copy1.Content = sessionId;

                    countLines++;
                }
                for (int i = 0; i < sessionId - 1;i++ )
                {
                    tempInt = tempInt + session[i].requestIdlist.Count;
                }
                lable_tatalPages_Copy.Content = tempInt.ToString();
                
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
        private void UpdateSessionClassVariables()
        {
            DateTime tFirstHTMLReq, tLastHTMLReq;
            /* Updating no of page request part */
            session[sessionId].totalNoOfPagesRequestedInSession++;
            if (requestList[countLines].pageLastVisited.Contains(".gif") || requestList[countLines].pageLastVisited.Contains(".jpeg") || requestList[countLines].pageLastVisited.Contains(".jpg") || requestList[countLines].pageLastVisited.Contains(".png"))
            {
                session[sessionId].noOfImagePagesRequestedInSession++;
                
            }
            else if(requestList[countLines].pageLastVisited.Contains(".ps")||requestList[countLines].pageLastVisited.Contains(".pdf"))
            {
                session[sessionId].noOfBinaryDocumentsRequestedInSession++;
                
            }
            else if (requestList[countLines].pageLastVisited.Contains(".cgi") || requestList[countLines].pageLastVisited.Contains(".exe"))
            {
                session[sessionId].noOfBinaryExeFileRequestedInSession++;
                
            }
            else if (requestList[countLines].pageLastVisited.Contains("robots.txt"))
            {
                session[sessionId].isRobotstxtVisited=true;
                
            }
            else if (requestList[countLines].pageLastVisited.Contains(".htm") || requestList[countLines].pageLastVisited.Contains(".html"))
            {
                session[sessionId].noOfHTMLFileRequestedInSession++;
                if (session[sessionId].noOfHTMLFileRequestedInSession == 1)
                    tFirstHTMLReq = requestList[countLines].timeStamp;
                else
                    tLastHTMLReq = requestList[countLines].timeStamp;
                // average time in minutes//
                session[sessionId].avgTimeBetweenTowHTMLRequests = ((tLastHTMLReq - tFirstHTMLReq))/(session[sessionId].noOfHTMLFileRequestedInSession);
            }
            else if (requestList[countLines].pageLastVisited.Contains(".txt") || requestList[countLines].pageLastVisited.Contains(".c") || requestList[countLines].pageLastVisited.Contains(".java"))
            {
                session[sessionId].noOfAsciiFilerequestedInSession++;
                
            }
            else if (requestList[countLines].pageLastVisited.Contains(".zip") || requestList[countLines].pageLastVisited.Contains(".gz") || requestList[countLines].pageLastVisited.Contains(".rar"))
            {
                session[sessionId].noOfCompressedFileRequestedInSession++;
                
            }
            else if (requestList[countLines].pageLastVisited.Contains(".wav") || requestList[countLines].pageLastVisited.Contains(".mpg") || requestList[countLines].pageLastVisited.Contains(".mpeg") || requestList[countLines].pageLastVisited.Contains(".avi") || requestList[countLines].pageLastVisited.Contains(".mp4") || requestList[countLines].pageLastVisited.Contains(".3gp") || requestList[countLines].pageLastVisited.Contains(".flv"))
            {
                session[sessionId].noOfMultimediaFileRequestedInSession++;
                
            }
            else 
            {
                session[sessionId].noOfOtherFileFormatRequestedInSession++;
                
            }
            /* Updating the time period */
            if (requestList[countLines].timeStamp.Hour <= 7 && requestList[countLines].timeStamp.ToString().Contains("AM"))
            {
                session[sessionId].noOfPagesRequestedInNightTime++;
            }
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
        private bool isNewSession(String tempSessionName)
        {
            return true;
        }
    }
}
