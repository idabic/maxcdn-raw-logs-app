using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaxCDN;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace maxcdn_dotnet_csharp_raw_logs
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            monthCalendar1.SelectionStart = DateTime.Today.AddDays(-4);
            monthCalendar1.SelectionEnd = DateTime.Today.AddDays(-4);
            monthCalendar2.SelectionStart = DateTime.Today;
            monthCalendar2.SelectionEnd = DateTime.Today;            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            string append = "";
            if (textBox1.Text != "") { append += "&status=" + textBox1.Text; }
            if (textBox2.Text != "") { append += "&cache_status=" + textBox2.Text; }
            if (textBox3.Text != "") { append += "&hostname=" + textBox3.Text; }
            var requestTimeout = 30;
            var alias = Properties.Settings.Default._alias;
            var consumer_key = Properties.Settings.Default._consumer_key;
            var consumer_secret = Properties.Settings.Default._consumer_secret;
            var api = new MaxCDN.Api(alias, consumer_key, consumer_secret, requestTimeout);
            var start = monthCalendar1.SelectionStart.Date.Year + "-" + monthCalendar1.SelectionStart.Date.Month + "-" + monthCalendar1.SelectionStart.Date.Day;
            var end = monthCalendar2.SelectionEnd.Year + "-" + monthCalendar2.SelectionStart.Date.Month + "-" + monthCalendar2.SelectionStart.Date.Day;
            var res = api.Get("/v3/reporting/logs.json?limit=1000&start=" + start + "&end=" + end + append);
            string result = Convert.ToString(JObject.Parse(res));
            dynamic jsondes = JsonConvert.DeserializeObject(result);
            
            var list = jsondes.records;
            JArray items = (JArray)jsondes["records"];
            int count = items.Count;

            for (int i = 0; i < count; i++)
            {
                string time = list[i].time;
                string location = list[i].client_city + ", " + list[i].client_country;
                string host = list[i].hostname;
                string uri = list[i].uri;
                string referer = list[i].referer;
                string status = list[i].status;
                string cachestatus = list[i].cache_status;
                ListViewItem line = new ListViewItem(new[] { time, location, host, uri, referer, status, cachestatus });
                this.listView1.Items.Add(line);
            }
            label4.Text = count.ToString();
        }
    }
}
