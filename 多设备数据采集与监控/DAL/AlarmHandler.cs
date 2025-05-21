using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 多设备数据采集与监控.Model;

namespace 多设备数据采集与监控.DAL
{
    class AlarmHandler
    {
        private static readonly Lazy<AlarmHandler> alarmHandler = new Lazy<AlarmHandler>(() => new AlarmHandler());
        public Dictionary<int, AlarmSetting> deviceThresholds = new Dictionary<int, AlarmSetting>();
        public event Action<AlarmInfo> OnAlarmTriggered;
        private readonly string Path = "AlarmInfo/alarm.csv";

        private AlarmHandler() { }
        
        public static AlarmHandler CreateAlarm()
        {
            return alarmHandler.Value;
        }
        public void SetThreshold(int deviceId, AlarmSetting setting)
        {
            deviceThresholds[deviceId] = setting;
            if (!File.Exists(Path))
            {
                File.WriteAllText(Path, "时间,设备ID,轴,值,阈值\n", Encoding.UTF8);
            }
        }

        public void Check(DataInfo data, Dictionary<int, AlarmSetting> deviceThresholds)
        {
            if (!deviceThresholds.TryGetValue(data.DeviceId, out var setting)) return;
               
            if (data.DataXValue < setting.MinXYZ || data.DataXValue > setting.MaxXYZ)
                RaiseAlarm(data.DeviceId, "X", data.DataXValue, data.DataXValue < setting.MinXYZ ? setting.MinXYZ : setting.MaxXYZ, data.TimeStamp);

            if (data.DataYValue < setting.MinXYZ || data.DataYValue > setting.MaxXYZ)
                RaiseAlarm(data.DeviceId, "Y", data.DataYValue, data.DataYValue < setting.MinXYZ ? setting.MinXYZ : setting.MaxXYZ, data.TimeStamp);

            if (data.DataZValue < setting.MinXYZ || data.DataZValue > setting.MaxXYZ)
                RaiseAlarm(data.DeviceId, "Z", data.DataZValue, data.DataZValue < setting.MinXYZ ? setting.MinXYZ : setting.MaxXYZ, data.TimeStamp);

        }

        private void RaiseAlarm(int deviceId, string axis, int value, int threshold, DateTime timestamp)
        {
            var alarm = new AlarmInfo
            {
                DeviceId = deviceId,
                Axis = axis,
                Value = value,
                Threshold = threshold,
                TimeStamp = timestamp
            };
            AlarmToCsv(alarm);
            
            OnAlarmTriggered?.Invoke(alarm);
        }

        private void AlarmToCsv(AlarmInfo alarm)
        {
            lock (this)
            {
                string info = $"{alarm.TimeStamp.ToString("yyyy-MM-dd:HH:mm:ss")},{alarm.DeviceId},{alarm.Axis},{alarm.Value},{alarm.Threshold}\n";
                File.AppendAllText(Path, info, Encoding.UTF8);
            }
           
        }
        public DataTable LoadAlarmCsv()
        {
            string filePath = "AlarmInfo/alarm.csv";
            DataTable table = new DataTable();
            table.Columns.Add("时间", typeof(string));
            table.Columns.Add("设备ID", typeof(int));
            table.Columns.Add("轴", typeof(string));
            table.Columns.Add("值", typeof(double));
            table.Columns.Add("阈值", typeof(double));

            var lines = File.ReadAllLines(filePath, Encoding.UTF8).Skip(1); 
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                table.Rows.Add(parts[0], int.Parse(parts[1]), parts[2], double.Parse(parts[3]), double.Parse(parts[4]));
            }
            return table;
        }
    }
}
