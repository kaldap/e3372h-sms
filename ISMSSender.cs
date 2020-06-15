using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaweiSMS
{
    public interface ISMSSender
    {
        int? SendSms(string number, string message, out string reply);
    }
}
