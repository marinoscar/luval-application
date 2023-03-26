using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.OpenAI.Models
{
    public class ChatHistory : BaseChatObject
    {
        public ChatHistory() : base()
        {
            History = new List<ChatMessage>();
        }

        public string Summary { get; set; }
        public List<ChatMessage> History { get; set; }
    }
}
