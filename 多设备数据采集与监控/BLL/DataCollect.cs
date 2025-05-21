using EasyModbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using 多设备数据采集与监控.DAL;
using 多设备数据采集与监控.Model;

namespace 多设备数据采集与监控.BLL
{
    class DataCollect
    {
        DataInfo dataInfo;
        bool IsRunning;
        AlarmHandler alarmHandler;
       public  Dictionary<int, Channel<DataInfo>> dictionaryData { get; set; }
        public void Start(List<DeviceInfo> devices)
        {
            dictionaryData = new Dictionary<int, Channel<DataInfo>>();
            IsRunning = true;
            alarmHandler= AlarmHandler.CreateAlarm();
            foreach (var item in devices)
            {
              //  System.Windows.Forms.MessageBox.Show($"{item.DeviceName}设备开始采集数据！");
                ModbusClient tcpClient = new ModbusClient();
                Channel<DataInfo> channel = Channel.CreateUnbounded<DataInfo>();
                dictionaryData[item.DeviceId] = channel;
                 Task.Run(async () =>
                {
                    tcpClient.Connect(item.IP, item.Port);
                  //  System.Windows.Forms.MessageBox.Show($"{item.DeviceName}设备连接成功！");
                    while (IsRunning)
                    {
                        try
                        {
                            int[] datas = tcpClient.ReadHoldingRegisters(0, 3);
                            dataInfo = new DataInfo
                            {
                                DataXValue = datas[0],
                                DataYValue = datas[1],
                                DataZValue = datas[2],
                                DeviceId = item.DeviceId,
                                TimeStamp = DateTime.Now
                             };
                                 alarmHandler.Check(dataInfo, alarmHandler.deviceThresholds);
                                await channel.Writer.WriteAsync(dataInfo);
                                await Task.Delay(item.Pinglv);
                            
                        }
                        catch (Exception ex)
                        {
                            System.Windows.Forms.MessageBox.Show($"{item.DeviceId}号设备采集错误:" + ex);
                        }
                       
                    }
                 }
                );
               
            }
        }

        public void StopCollect()
        {
             IsRunning = false;
        }
       
    }
}
