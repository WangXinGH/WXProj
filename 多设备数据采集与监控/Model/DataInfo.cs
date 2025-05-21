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
        public int DeviceId { get; set; }
        public DateTime TimeStamp { get; set; }
        public int DataXValue { get; set; }
        public int DataYValue { get; set; }
        public int DataZValue { get; set; }
    }
}
