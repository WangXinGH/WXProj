using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
            System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();
            timer2.Interval = 1000;
            timer2.Tick += Timer2_Tick;
            timer2.Start();
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            Random random = new Random();
            int a = random.Next(1, 100);
            int b = random.Next(1, 100);
            int c = random.Next(1, 100);

            this.chart.Series[0].Points.AddXY(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), a);
            this.chart.Series[1].Points.AddXY(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), b);
            this.chart.Series[2].Points.AddXY(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), c);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Device_Add device = new Device_Add(this);
            device.Show();
            
        }
        List<Device> deviceList = new List<Device> { };
        List<DataInfo> dataInfoLists = null;
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

        private void timer_data_Tick(object sender, EventArgs e)
        {
           
            foreach (var dataInfo in dataInfoLists)
            {
                if (this.listBox1.SelectedItem.ToString() == dataInfo.DeviceName)
                {
                   
                    this.label18.Text = dataInfo.DataXValue.ToString();
                    this.label19.Text = dataInfo.DataYValue.ToString();
                    this.label20.Text = dataInfo.DataZValue.ToString();
                }
            }
        }
      

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                this.timer_data.Interval = Convert.ToInt16(this.textBox5.Text);
                CollectData collect = new CollectData();
                dataInfoLists = collect.StartCollectDatas(deviceList);
                this.timer_data.Enabled = true;
                this.timer_data.Start();
            }
            catch (Exception)
            {

                MessageBox.Show($"请输入采集频率！");
            }
          
        }
        public void update_chart()
        {
            Task.Run(() => { 
            CollectData collect = new CollectData();
            List<DataInfo> dataInfoLists = collect.StartCollectDatas(deviceList);
            foreach (var dataInfo in dataInfoLists)
            {
                this.chart.Series[0].Points.AddXY(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),dataInfo.DataXValue);
                this.chart.Series[1].Points.AddXY(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),dataInfo.DataYValue);
                this.chart.Series[2].Points.AddXY(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),dataInfo.DataZValue);
            }

            });

        }

      
       public async Task showAlarm()
        {
            
        }

      
        private void FormMain_Load(object sender, EventArgs e)
        {
           
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var device in deviceList)
            {
                if (this.listBox1.SelectedItem.ToString() ==device.Name)
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
            }
           
            this.Invoke(new MethodInvoker(() => {


                update_chart();


            }));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        { 
            this.label10.ForeColor = Color.Red;
            AlarmHelper alarm = new AlarmHelper();
         
               this.textBox1.Text =  alarm.ShowAlarm();

              
           
        }
    }
}
