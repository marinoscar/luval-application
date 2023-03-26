using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.OpenAI
{
    public class ChatEventArgs : EventArgs
    {
        public string Message { get; set; }
        public bool IsPrompt { get; set; }
    }
}
