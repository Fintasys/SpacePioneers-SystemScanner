using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace SpacePioneersBot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            cid = "78087";

        }

        string cid = "";

        WebBrowser webBrowser2 = new WebBrowser();

        private void btn_start_Click(object sender, EventArgs e)
        {
            run();
        }

        private void run()
        {
            while (true)
            {

                // Inject 
                injectScript();
                int x_ = 0;
                int y_ = 0;

                for (int x = 1; x < 100; x++)
                {
                    for (int y = 1; y < 100; y++)
                    {
                        // Navigate to System
                        if (x_ == 0 && y_ == 0)
                            webBrowser1.Document.InvokeScript("init");

                        // Current Coords
                        x_ = x;
                        y_ = y;

                        // Parse
                        //MessageBox.Show(webBrowser1.Document.GetElementById("obj_systemviewer").InnerText);

                        // Move Next Koord
                        webBrowser1.Document.InvokeScript("nextSystem");

                        waiting(3 + new Random().Next(1, 3));
                       
                    }
                }
                
            }
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

        
    }
}
