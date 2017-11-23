using System;
using System.Net;

namespace SnmpManager
{
    public class SnmpRadioManagerDummy : ISnmpRadioManager
    {
        private readonly Random _rand = new Random(1);

        public bool OpenSession(IPAddress deviceIp, short port, string community, int version, string username, string authPass, string privPass)
        {
            return true;
        }

        public bool Set(string attributeName, object value, int timeout)
        {
            return true;
        }

        public object Get(string attributeName, int timeout)
        {
            return _rand.Next(0,100);
        }

        public bool IsOpen()
        {
            return false;
        }
    }
}
