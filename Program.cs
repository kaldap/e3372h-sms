using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaweiSMS
{
    class Program
    {
        static void Main(string[] args)
        {
            string s;
            var smsSender = new SMS_E3372h(new Uri("http://192.168.8.1/"));
            //smsSender.SwitchMode(SMS_E3372h.Mode.Modem, out s); // Switch to AT
            var rep = smsSender.SendSms("+420792432416", "[{< Ahoj , testuju to z API! :-) >)]", out s);
            if (rep.HasValue)
                Console.WriteLine(Errors.Get(rep.Value));
            else
                Console.WriteLine("Request OK!");
            Console.WriteLine("Reply:\r\n" + s);
        }
    }
}
