using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Web;

namespace HuaweiSMS
{
    public class SMS_E3372h : ISMSSender
    {
        private Uri _host;
        private string _session;
        private string _token;

        public SMS_E3372h(Uri host)
        {
            _host = host;
        }

        public int? SendSms(string number, string message, out string reply)
        {
            reply = null;
            message = HttpUtility.HtmlEncode(message);
            using (MemoryStream stream = new MemoryStream())
            {
                XmlElement req, phones, tmp;
                var doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
                doc.AppendChild(req = doc.CreateElement("request"));
                req.AppendChild(tmp = doc.CreateElement("Index"));
                tmp.InnerText = "-1";
                req.AppendChild(phones = doc.CreateElement("Phones"));
                phones.AppendChild(tmp = doc.CreateElement("Phone"));
                tmp.InnerText = number;
                req.AppendChild(tmp = doc.CreateElement("Sca"));
                tmp.InnerText = "";
                req.AppendChild(tmp = doc.CreateElement("Content"));
                tmp.InnerText = message;
                req.AppendChild(tmp = doc.CreateElement("Length"));
                tmp.InnerText = message.Length.ToString();
                req.AppendChild(tmp = doc.CreateElement("Reserved"));
                tmp.InnerText = "1";
                req.AppendChild(tmp = doc.CreateElement("Date"));
                tmp.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                using (var writer = new XmlTextWriter(stream, new UTF8Encoding(false)))
                {
                    doc.Save(writer);
                    stream.Flush();
                    message = Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
                }
            }

            var res = GetSessionAndToken();
            if (res.HasValue) return res;
            return SendAuthenticatedRequest("/api/sms/send-sms", message, out reply);
        }

        public int? SwitchMode(Mode mode, out string reply)
        {
            reply = null;
            var res = GetSessionAndToken();
            if (res.HasValue) return res;
            return SendAuthenticatedRequest("/api/device/mode", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><request><mode>" + mode + "</mode></request>", out reply);
        }

        private int? SendAuthenticatedRequest(string relUrl, string content, out string respString)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(_host, relUrl));
            request.Headers.Add("__RequestVerificationToken", _token);
            request.Headers.Add("_ResponseSource", "Broswer");
            request.Headers.Add(HttpRequestHeader.Cookie, "SessionID=" + _session);
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.ContentType = "text/xml";
            request.Accept = "*/*";            
            request.KeepAlive = true;
            request.ServicePoint.Expect100Continue = false;

            request.Method = "POST";
            request.ContentLength = bytes.Length;

            var stream = request.GetRequestStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();

            WebResponse response = request.GetResponse();
            try
            {
                respString = null;
                using (Stream dataStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(dataStream))
                    respString = reader.ReadToEnd();

                var doc = new XmlDocument();
                doc.LoadXml(respString);

                if (doc.DocumentElement.Name.ToLower() == "error")
                    return int.Parse(doc.SelectSingleNode("/error/code/text()").InnerText);
            }
            finally
            {
                response.Close();
            }
            return null;
        }

        private int? GetSessionAndToken()
        {
            WebRequest request = WebRequest.Create(new Uri(_host, "/api/webserver/SesTokInfo"));
            WebResponse response = request.GetResponse();

            try
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    var doc = new XmlDocument();
                    doc.Load(dataStream);

                    if (doc.DocumentElement.Name.ToLower() == "error")
                        return int.Parse(doc.SelectSingleNode("/error/code/text()").InnerText);

                    _session = doc.SelectSingleNode("/response/SesInfo/text()").InnerText;
                    _token = doc.SelectSingleNode("/response/TokInfo/text()").InnerText;
                }
            }
            finally
            {
                response.Close();                
            }
            return null;
        }

        public enum Mode { Modem = 0, Debug = 1, HiLink = 2 }
    }
}
