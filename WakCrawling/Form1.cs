using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WakCrawling
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void but_Login_Click(object sender, EventArgs e)
        {
            string loginUrl = "https://yeoncho.riroschool.kr/ajax.php";
            string postUrl = "https://yeoncho.riroschool.kr/portfolio.php?db=1502&t_doc=0";

            string data = $"app=user&mode=login&userType=1&id={input_id.Text}&pw={input_pw.Text}";
            byte[] buffer = Encoding.UTF8.GetBytes(data);

            CookieContainer cookies = new CookieContainer();

            try
            {
                WebResponse loginRes = GetHtml(loginUrl, buffer, cookies);
                if(loginRes.ContentLength != 88)
                {
                    MessageBox.Show("아이디 또는 비밀번호가 일치하지 않습니다.");
                    return;
                }
                WebResponse postRes = GetHtml(postUrl, null, cookies);

                Stream stream = postRes.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream, Encoding.UTF8, false);

                string html = streamReader.ReadToEnd();
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(html);

                HtmlNodeCollection descNodes = htmlDoc.DocumentNode.SelectNodes("//*[@id='container']/div/div[3]/table/tr/td[3]");
                RsAppend(descNodes);

                //textBox1.AppendText(html);
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
        }

        WebResponse GetHtml(string url, byte[] buffer, CookieContainer cookies)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "post";
            req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            req.CookieContainer = cookies;

            if (buffer != null)
            {
                req.GetRequestStream().Write(buffer, 0, buffer.Length);
            }

            WebResponse res = req.GetResponse();
            return res;
        }

        void RsAppend(HtmlNodeCollection nodes)
        {
            for (int i = 1; i < nodes.Count; i++)
            {
                string text = nodes[i].InnerText.Replace("  ", "");
                textBox1.AppendText(text + Environment.NewLine + Environment.NewLine);
            }
        }
    }

}
