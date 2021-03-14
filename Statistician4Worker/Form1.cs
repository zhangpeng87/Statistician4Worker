using Newtonsoft.Json;
using Statistician4Worker.Models;
using Statistician4Worker.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Statistician4Worker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.dgvReport.AutoGenerateColumns = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private string[] LoadProject()
        {
            Console.WriteLine($"Start LoadProject()...{ Thread.CurrentThread.ManagedThreadId }");
            IList<标段> result = null;

            using (StreamReader file = File.OpenText(@"D:\LogFile\Statistician4Worker\Project.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                result = (IList<标段>)serializer.Deserialize(file, typeof(IList<标段>));
            }

            Console.WriteLine("Ended LoadProject()...");
            return result.OrderBy(e => e.排序数字).Select(e => e.标段名称).ToArray();
        }

    }
}
