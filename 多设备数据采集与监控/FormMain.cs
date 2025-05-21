using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Forms;
using 多设备数据采集与监控.BLL;
using 多设备数据采集与监控.DAL;
using 多设备数据采集与监控.Model;

namespace 多设备数据采集与监控
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

        }
        bool IsSaving = false;
        bool IsCollectting = false;
        DeviceInfo deviceInfo;
        DataInfo dataInfo;
        SQLiteHelper sqlHelper;
        List<DeviceInfo> ts;
        DataCollect dataCollect;
        CancellationTokenSource uiTokenSource;
        CancellationTokenSource cts;
        AlarmHandler alarmHandler;
        private void FormMain_Load(object sender, EventArgs e)
        {
            alarmHandler = AlarmHandler.CreateAlarm();
            alarmHandler.OnAlarmTriggered += UIAlarm;
            sqlHelper = new SQLiteHelper();
            ts = null;
            ts = sqlHelper.Query<DeviceInfo>("select * from DeviceInfo", null);
            foreach (var item in ts)
            {
                this.listBox1.Items.Add(item);
                this.listBox1.DisplayMember = "DeviceName";
            }

        }
        private void Dev_add_Click(object sender, EventArgs e)
        {
            deviceInfo = new DeviceInfo();
            deviceInfo.DeviceName = this.text_Id.Text;
            deviceInfo.IP = this.text_IP.Text;
            deviceInfo.Port = Convert.ToInt32(this.text_port.Text);
            deviceInfo.SlaveId = int.Parse(this.text_sid.Text);
            deviceInfo.Pinglv = Convert.ToInt32(this.text_pl.Text);

            try
            {
                sqlHelper = new SQLiteHelper();
                string sql = "INSERT INTO DeviceInfo(DeviceName, IP, Port, SlaveId, Pinglv) VALUES (@DeviceName, @IP, @Port, @SlaveId, @Pinglv)";
                SQLiteParameter[] parameters = new SQLiteParameter[]
                {
            new SQLiteParameter("@DeviceName", deviceInfo.DeviceName),
            new SQLiteParameter("@IP", deviceInfo.IP),
            new SQLiteParameter("@Port", deviceInfo.Port),
            new SQLiteParameter("@SlaveId", deviceInfo.SlaveId),
            new SQLiteParameter("@Pinglv", deviceInfo.Pinglv)
                };

                int res = sqlHelper.ExecuteNonQuery(sql, parameters);
                if (res == 1)
                {
                    MessageBox.Show("添加成功！");
                    FormMain_Load(sender, e);
                    refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加失败:{ex}");
            }
        }

        private void refresh()
        {
            this.text_Id.Text = "";
            this.text_IP.Text = "";
            this.text_pl.Text = "";
            this.text_port.Text = "";
            this.text_sid.Text = "";
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataCollect == null)
                dataCollect = new DataCollect();

            if (!IsCollectting)
            {
                dataCollect.Start(ts);
                IsCollectting = true;
                this.button2.Text = "停止采集";
                MessageBox.Show("采集启动成功");
            }
            else
            {
                dataCollect.StopCollect();
                this.button2.Text = "开始采集";
            }
           
        }
        private async void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
                return;
            if (listBox1.SelectedItem is DeviceInfo device)
            {
                foreach (var item in ts)
                {
                    if (device.DeviceId == item.DeviceId)
                    {
                        cts = new CancellationTokenSource();
                        UpdateToUI(item.DeviceId);
                        this.label3.Text = $"设备：{item.DeviceName}";
                        MessageBox.Show($"点击查看实时数据");
                    }
                    else
                    {
                        cts?.Cancel();
                        await Task.Delay(2000);
                        cts?.Dispose();
                       
                        cts = new CancellationTokenSource();
                        UpdateToUI(device.DeviceId);
                    }
                }
            }
        }
     
        private void UpdateToUI(int deviceId)
        {
            try
            {
               
                bool booler = dataCollect.dictionaryData.TryGetValue(deviceId, out Channel<DataInfo> channel);
                if (!dataCollect.dictionaryData.ContainsKey(deviceId))
                {
                    MessageBox.Show("没有该设备的ID！");
                }
                else if (!booler)
                {
                    MessageBox.Show("通道不存在！");
                }
                else
                {
                    Task.Run(async () =>
                    {
                        while (await channel.Reader.WaitToReadAsync(cts.Token) && !cts.IsCancellationRequested)
                        {
                            DataInfo data = await channel.Reader.ReadAsync();
                            dataInfo = new DataInfo();
                            dataInfo = data;
                            this.Invoke(new MethodInvoker(() =>
                            {
                                this.XAxis.Text = $"X值:{data.DataXValue.ToString()}";
                                this.YAxis.Text = $"Y值:{data.DataYValue.ToString()}";
                                this.ZAxis.Text = $"Z值:{data.DataZValue.ToString()}";
                                this.chart.Series[0].Points.AddXY(DateTime.Now.ToString(), data.DataXValue.ToString()); 
                                this.chart.Series[1].Points.AddXY(DateTime.Now.ToString(), data.DataYValue.ToString()); 
                                this.chart.Series[2].Points.AddXY(DateTime.Now.ToString(), data.DataZValue.ToString()); 
                            }));
                        }
                    }, cts.Token);


                }
            }
            catch (NullReferenceException ex)
            {

                MessageBox.Show($"请先启动采集！{ex}");
            }
          

        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (!IsSaving)
            {
                this.timerData.Start();
                IsSaving = true;
                MessageBox.Show("开始保存到数据库！");
                this.button1.Text = "停止保存";
            }
            else
            {
                this.timerData.Stop();
                this.button1.Text = "存入数据库";
            }
           
        }
       

        private void timerData_Tick(object sender, EventArgs e)
        {
                sqlHelper = new SQLiteHelper();
                SQLiteParameter[] P = new SQLiteParameter[]
                {
                new SQLiteParameter("@deviceId", dataInfo.DeviceId),
                new SQLiteParameter("@createTime", DateTime.Now),
                new SQLiteParameter("@Dx", dataInfo.DataXValue),
                new SQLiteParameter("@Dy", dataInfo.DataYValue),
                new SQLiteParameter("@Dz", dataInfo.DataZValue)
                };

                sqlHelper.ExecuteNonQuery("INSERT INTO DataInfo(DeviceId, TimeStamp, DataXValue, DataYValue, DataZValue) VALUES (@deviceId, @createTime, @Dx, @Dy, @Dz)", P);
              
        }
        
        
        private void UIAlarm(AlarmInfo alarm)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                this.textBox1.AppendText($"{alarm.TimeStamp:yyyy-MM-dd HH:mm:ss},{alarm.DeviceId},{alarm.Axis},{alarm.Value},{alarm.Threshold}\r\n");
               
            }));
           
        }

        private void alarmSetting_Click(object sender, EventArgs e)
        {
            foreach (var item in ts)
            {
                alarmHandler.SetThreshold(item.DeviceId, new AlarmSetting()
                {
                    MinXYZ = int.Parse(this.textBox3.Text),
                    MaxXYZ = int.Parse(this.textBox2.Text)
                });
            }
            MessageBox.Show("设置成功！");
        }

        private void halarmHistory_Click(object sender, EventArgs e)
        {
            this.dataGridView1.AutoGenerateColumns = true;
            this.dataGridView1.DataSource = alarmHandler.LoadAlarmCsv();
        }
       
    }
}