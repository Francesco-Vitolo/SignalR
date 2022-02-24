using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Message
    {
        public string text;
        public DateTime time;
        public string sender;
        public string groupname;
        public string connID;

        public Message(string text, DateTime time, string sender, string groupname,string connID)
        {
            this.text = text;
            this.time = time;
            this.sender = sender;
            this.groupname = groupname;
            this.connID = connID;
        }

        public override string ToString()
        {
            return $"\n{sender}\n{text}\n{time:HH:mm}";
        }
    }
}
