using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 多设备数据采集与监控.Model
{
    class DeviceInfo
    {
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public int SlaveId { get; set; }
        public int Pinglv { get; set; }
        public bool DeviceEnable { get; set; } = false;
        

    }
}
