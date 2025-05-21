using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 多设备数据采集与监控.Model
{
    class AlarmInfo
    {
        public int DeviceId { get; set; }
        public string Axis { get; set; }
        public int Value { get; set; }
        public int Threshold { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
