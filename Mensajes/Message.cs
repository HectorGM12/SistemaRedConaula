using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
namespace Mensajes
{  
    [Serializable]
    public class Message
    {
        public Types type;
        public string message;

        public Message(Types type,string message)
        {
            this.type = type;
            this.message = message;
        }
        public Message(Types type)
        {
            this.type = type;
        }
    }
}
