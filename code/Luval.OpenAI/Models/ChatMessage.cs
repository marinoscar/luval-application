using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.OpenAI.Models
{
    public class ChatMessage : BaseChatObject
    {
        public ChatMessage() : base()
        {
        }
        public string ClientText { get; set; }
        public string Response { get; set; }
    }
}
