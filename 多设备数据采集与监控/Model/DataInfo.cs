using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 多设备数据采集与监控.Model
{
    class DataInfo
    {
        public int DataId { get; set; }
        public string DeviceName { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public int SlaveId { get; set; }
       
        public bool DeviceState { get; set; } = false;

        public int DataXValue { get; set; }
        public int DataYValue { get; set; }
        public int DataZValue { get; set; }

        public DateTime CollectTime { get; set; }
        public DateTime CreateAt { get; set; }

    }
}
