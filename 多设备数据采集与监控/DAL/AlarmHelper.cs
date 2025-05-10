using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 多设备数据采集与监控.Model;

namespace 多设备数据采集与监控.DAL
{
   public class AlarmHelper
    {
        AlarmInfo alarmInfo;
        public event Action AlarmEvent;
        public AlarmHelper()
        {
             alarmInfo = new AlarmInfo();
        }

        public void Invoker()
        {

            AlarmEvent.Invoke();
        }
      public AlarmInfo CollectAlarm(int DeviceId,string AlarmInfomation, DateTime TimeStamp)
        {
            alarmInfo.DeviceId = DeviceId;
            alarmInfo.AlarmInfomation = AlarmInfomation;
            alarmInfo.TimeStamp = TimeStamp;
            alarmInfo.AlarmState = true;

            return alarmInfo;

        }

        public string ShowAlarm()
        {
            string info = $"设备_{alarmInfo.AlarmId}在{alarmInfo.TimeStamp}发生了:{alarmInfo.AlarmInfomation}，状态:{alarmInfo.AlarmState}";
            return info;
        }
    }
}
