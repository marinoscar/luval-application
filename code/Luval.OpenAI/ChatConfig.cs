using OpenAI_API;
using OpenAI_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Luval.OpenAI
{
    public class ChatConfig
    {
        public ChatConfig()
        {
            Temperature = 0.7d;
            Model = Model.ChatGPTTurbo;
        }

        public SecureString ApiKey { get; set; }
        public double Temperature { get; set; }
        public Model Model { get; set; }

        public string GetKey() { return new NetworkCredential("", ApiKey).Password; }
        public OpenAIAPI CreateAPI() { 
            return new OpenAIAPI(GetKey());
        }

    }
}
