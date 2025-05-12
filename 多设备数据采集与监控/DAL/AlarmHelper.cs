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
        public event Func<int, string, DateTime, AlarmInfo> AlarmEvent;
        public AlarmHelper()
        {
             alarmInfo = new AlarmInfo();
        }

        public void Invoker(int DeviceId, string AlarmInfomation, DateTime TimeStamp)
        {

            AlarmEvent.Invoke(DeviceId, AlarmInfomation, TimeStamp);
        }
      

       
    }
}
