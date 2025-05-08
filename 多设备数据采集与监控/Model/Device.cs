using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 多设备数据采集与监控.Model
{
    class Device
    {
       public string Name { get; set; }
       public string IP { get; set; }
        public int Port { get; set; }
        public int SlaveId { get; set; }
        public int Pinglv { get; set; }
        public bool ConnState { get; set; } = false;
        public bool AlarmState { get; set; } = false;

    }
}
