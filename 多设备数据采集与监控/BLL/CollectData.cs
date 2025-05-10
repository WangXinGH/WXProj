using EasyModbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using 多设备数据采集与监控.DAL;
using 多设备数据采集与监控.Model;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace 多设备数据采集与监控.BLL
{
    class CollectData
    {




        public List<DataInfo> StartCollectDatas(List<Device> deviceList)
        {

            List<DataInfo> list = new List<DataInfo>();
            List<Task> tasks = new List<Task>();
            foreach (var device in deviceList)
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        DataInfo data = new DataInfo();

                        ModbusClient modbusClient = new ModbusClient(device.IP, device.Port);
                        data.CollectTime = DateTime.Now;
                        data.SlaveId = device.DeviceId;
                        data.DeviceState = true;
                        modbusClient.Connect();
                        int[] datas = modbusClient.ReadHoldingRegisters(0, 3);
                        if (datas[0]>90| datas[1]>90 |datas[2]>90)
                        {
                            AlarmHelper alarm = new AlarmHelper();
                            alarm.CollectAlarm(device.SlaveId, "X>90或Y>90或Z>90", DateTime.Now);
                            alarm.Invoker();
                        }
                        data.DataXValue = datas[0];
                        data.DataYValue = datas[1];
                        data.DataZValue = datas[2];
                        modbusClient.Disconnect();
                        lock (this)
                        {
                            list.Add(data);
                        }
                    }
                    catch (Exception ex)
                    {
                        
                        MessageBox.Show($"连接设备失败，看不到XYZ轴值，请检查IP和端口！", "错误连接提示", MessageBoxButtons.OK);
                       
                    }
                  
                  
                }, cts.Token));
               
            }
            return list;
        }
    }

    
}
