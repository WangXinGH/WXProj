using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using 多设备数据采集与监控.BLL;
using 多设备数据采集与监控.DAL;
using 多设备数据采集与监控.Model;

namespace 多设备数据采集与监控
{
    public partial class FormMain: Form
    {
        public FormMain()
        {
            InitializeComponent();
          
        }
        List<DataInfo> dataInfoLists;
        List<Device> deviceList = new List<Device>();


        private void button1_Click(object sender, EventArgs e)
        {
            Device_Add device = new Device_Add(this);
            device.Show();
            
        }
        public void add_device(string name,string ip,int port,int slaveId)
        {
            Device device = new Device();
            device.Name = name;
            device.IP = ip;
            device.Port = port;
            device.SlaveId = slaveId;
           
            deviceList.Add(device);
            this.listBox1.Items.Add(device.Name);
           
        }
        CollectData collect = new CollectData();
        private void timer_data_Tick(object sender, EventArgs e)
        {
           
            foreach (var dataInfo in dataInfoLists)
                {
              
                this.chart.Series[0].Points.AddXY(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), dataInfo.DataXValue);
                        this.chart.Series[1].Points.AddXY(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), dataInfo.DataYValue);
                        this.chart.Series[2].Points.AddXY(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), dataInfo.DataZValue);
                        this.label18.Text = dataInfo.DataXValue.ToString();
                        this.label19.Text = dataInfo.DataYValue.ToString();
                        this.label20.Text = dataInfo.DataZValue.ToString();
                    
                }
            ; 
        }
      

        private void button2_Click(object sender, EventArgs e)
        {
            collect.StartCollectDatas(deviceList);
            dataInfoLists = collect.GetDataAsync();
            try
            {
                this.timer_data.Interval = Convert.ToInt16(this.textBox5.Text);
                this.timer_data.Enabled = true;
                this.timer_data.Start();

            }
            catch (Exception)
            {

                MessageBox.Show($"请输入采集频率！");

            }
        }
        
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

                foreach (var device in deviceList)
            {
                if (this.listBox1.SelectedItem!=null)
                {
                    
               
                if (this.listBox1.SelectedItem.ToString() == device.Name)
                    {

                        this.textBox4.Text = device.Name;
                        this.textBox6.Text = device.IP;
                        this.textBox2.Text = device.Port.ToString();
                        this.textBox3.Text = device.SlaveId.ToString();

                        if (device.ConnState == true)
                        {
                            this.label10.ForeColor = Color.Green;
                        }
                    }
                    else
                    {
                        MessageBox.Show("选择一项");
                    }
                }
                
            }
           
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        { 
            this.label10.ForeColor = Color.Red;
         
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
           

        }
    }
}
