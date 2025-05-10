using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 多设备数据采集与监控
{
    public partial class Device_Add: Form
    {
        private FormMain a;
        public Device_Add(FormMain formMain)
        {
            InitializeComponent();
            a = formMain;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                a.add_device(this.textBox4.Text.ToString(), textBox1.Text, Convert.ToInt16(textBox2.Text),
                Convert.ToInt16(textBox3.Text));
                this.Close();
            }
            catch (Exception)
            {

                MessageBox.Show("添加设备字符有误！");
            }
            
        }
        
    }
}
