using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Collections;
using System.IO;
using System.Json;

namespace SpacePioneersBot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        StringBuilder data = new StringBuilder();
        string cid = "";

        WebBrowser webBrowser2 = new WebBrowser();

        private void btn_start_Click(object sender, EventArgs e)
        {
            cid = txt_cid.Text;
            btn_start.Enabled = false;
            run();
        }

        private void run()
        {

                data.Append("{ \"data\": [");

                // Inject 
                int x_ = 0;
                int y_ = 0;

                for (int x = int.Parse(txt_x.Text); x < 100; x++)
                {
                    for (int y = int.Parse(txt_y.Text); y < 100; y++)
                    {
                        // Current Coords
                        x_ = x;
                        y_ = y;

                        lbl_x.Text = "X: " + x.ToString();
                        lbl_y.Text = "Y: " + y.ToString();

                        // Send Request
                        long tcv = DateTime.Now.Millisecond;
                        webBrowser2.Url = new System.Uri("http://sp2.looki.de/index.php?page=newsysview&cid=" + cid + "&ppx=" + x + "&ppy=" + y + "&tcv=" + tcv + "&_=_846_62", System.UriKind.Absolute);
                        waiting(1);

                        // Send JSON to Rest API
                        string json = webBrowser2.DocumentText;
                        txtLog.Text = FormatJson(json);
                        if (!json.Contains("err_notvisible"))
                        {
                            if (json != "")
                            {
                                if(json.IndexOf("syslist\":[]") == -1) {

                                    try
                                    {
                                        int index = json.IndexOf(",\"tflist\":");
                                        if (index != -1)
                                            json = json.Substring(0, index) + "}";
                                        JsonValue.Parse(json);
                                        //Console.WriteLine(json);
                                        data.Append(json + ",");
                                    }
                                    catch (FormatException ex)
                                    {
                                        //Invalid json format
                                        Console.WriteLine(ex.Message);
                                    }
                                    
                                //data += json + ",";
                                //waiting(0 + new Random().Next(1, 2));
                               }
                            }
                        }
                       
                    }
                }

                data.Remove(data.Length - 1, 1);
                data.Append("]}");
                txtLog.Text = data.ToString();

                using (StreamWriter writetext = new StreamWriter("universe.txt"))
                {
                    writetext.WriteLine(data.ToString());
                }

                sendPost(data.ToString());

                MessageBox.Show("Finished!");
        }


     


        private void injectScript()
        {
            HtmlElement head = webBrowser1.Document.GetElementsByTagName("head")[0];
            HtmlElement script1 = webBrowser1.Document.CreateElement("script");
            script1.SetAttribute("text", System.IO.File.ReadAllText(@"script.js"));
            head.AppendChild(script1);
        }

        private void waiting(int seconds)
        {
            DateTime Tthen = DateTime.Now;
            do
            {
                Application.DoEvents();
            } while (Tthen.AddSeconds(seconds) > DateTime.Now);      
        }


        private void sendPost(string json)
        {
            //var request = (HttpWebRequest)WebRequest.Create("http://localhost/SystemScanner/save-system.php");
            var request = (HttpWebRequest)WebRequest.Create("http://teamulmdroid.bplaced.net/spacepioneers/save-system.php");

            var data = Encoding.ASCII.GetBytes(json);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            string resp = "";
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var encoding = Encoding.GetEncoding(response.CharacterSet);

                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream, encoding))
                    resp = reader.ReadToEnd();
            }

        }


        private const string INDENT_STRING = "    ";
        public static string FormatJson(string str)
        {
            var indent = 0;
            var quoted = false;
            var sb = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, ++indent).ToList().ForEach(item => sb.Append(INDENT_STRING));
                        }
                        break;
                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, --indent).ToList().ForEach(item => sb.Append(INDENT_STRING));
                        }
                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        var index = i;
                        while (index > 0 && str[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, indent).ToList().ForEach(item => sb.Append(INDENT_STRING));
                        }
                        break;
                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                            sb.Append(" ");
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            btn_clear.Enabled = false;
            var request = (HttpWebRequest)WebRequest.Create("http://teamulmdroid.bplaced.net/spacepioneers/delete-system.php");

            string json = "{'delete': 'true'}";

            var data = Encoding.ASCII.GetBytes(json);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            string resp = "";
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var encoding = Encoding.GetEncoding(response.CharacterSet);

                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream, encoding))
                    resp = reader.ReadToEnd();

                btn_clear.Enabled = true;
            }
        }

        
    }
}
