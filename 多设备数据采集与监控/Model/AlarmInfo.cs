using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 多设备数据采集与监控.Model
{
    public class AlarmInfo
    {
        public int AlarmId { get; set; }

        public int DeviceId { get; set; }
        public string AlarmInfomation { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool AlarmState { get; set; } = false;
       
    }
}
