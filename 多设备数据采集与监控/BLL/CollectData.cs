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
        List<DataInfo> list = new List<DataInfo>();
        ModbusClient modbusClient;
        public void StartCollectDatas(List<Device> deviceList)
        {
            DataInfo data = new DataInfo();
             foreach(var device in deviceList)
            {
                CancellationTokenSource cts = new CancellationTokenSource();

                Task.Run(async () =>
                {
                    try
                    {
                            modbusClient = new ModbusClient(device.IP, device.Port);
                            data.CollectTime = DateTime.Now;
                            data.SlaveId = device.DeviceId;
                            data.DeviceState = true;
                            modbusClient.Connect();
                        
                        while (true)
                        {
                            await Task.Delay(1000);
                            int[] datas = modbusClient.ReadHoldingRegisters(0, 3);
                            data.DataXValue = datas[0];
                            data.DataYValue = datas[1];
                            data.DataZValue = datas[2];

                            lock (this)
                            {
                                list.Add(data);
                                if (list.Count > 100)
                                {
                                    list.RemoveAt(0);
                                }
                               
                            }
                        }
                      

                    }
                        catch (Exception)
                        {

                            MessageBox.Show($"连接设备失败，看不到XYZ轴值，请检查IP和端口！", "错误连接提示", MessageBoxButtons.OK);

                        }
                }, cts.Token);
               
            }
        }

        public List<DataInfo> GetDataAsync()
        {
            return list;
        }

        public void CloseTCP()
        {
            modbusClient.Disconnect();
        }
    }

    
}
