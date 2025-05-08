using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 多设备数据采集与监控.Model;

namespace 多设备数据采集与监控
{
    public partial class FormMain: Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Device_Add device = new Device_Add(this);
            device.Show();
            
        }
        public void add_device(string name,string ip,int port,int slaveId, int pinglv)
        {
            Device device = new Device();
            device.Name = name;
            device.IP = ip;
            device.Port = port;
            device.SlaveId = slaveId;
            device.Pinglv = pinglv;

            
            this.listBox1.Items.Add(device.Name);
        }
    }
}
