using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaweiSMS
{
    public static class Errors
    {
        private static readonly Dictionary<int, string> _codes = new Dictionary<int, string>()
        {
            {-1,     "system not available"},
            {100002, "not supported by firmware or incorrect API path"},
            {100003, "unauthorized"},
            {100004, "system busy"},
            {100005, "unknown error"},
            {100006, "invalid parameter"},
            {100009, "write error"},
            {103002, "unknown error"},
            {103015, "unknown error"},
            {108001, "invalid username"},
            {108002, "invalid password"},
            {108003, "user already logged in"},
            {108006, "invalid username or password"},
            {108007, "invalid username}, password}, or session timeout"},
            {110024, "battery charge less than 50%"},
            {111019, "no network response"},
            {111020, "network timeout"},
            {111022, "network not supported"},
            {113018, "system busy"},
            {114001, "file already exists"},
            {114002, "file already exists"},
            {114003, "SD card currently in use"},
            {114004, "path does not exist"},
            {114005, "path too long"},
            {114006, "no permission for specified file or directory"},
            {115001, "unknown error"},
            {117001, "incorrect WiFi password"},
            {117004, "incorrect WISPr password"},
            {120001, "voice busy"},
            {125001, "invalid token"}
        };

        public static string Get(int error)
        {
            string v;
            if (_codes.TryGetValue(error, out v)) return v;
            return "Error not in list, code is: " + error;
        }

    }
}
