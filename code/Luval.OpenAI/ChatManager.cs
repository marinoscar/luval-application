using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Luval.OpenAI
{
    public class ChatManager
    {
        public ChatConfig Config { get; private set; }

        public event EventHandler<ChatEventArgs> StreamEvent;

        public ChatManager(ChatConfig config)
        {
            Config = config;
        }

        public async Task<string> RunPrompt(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt)) throw new ArgumentNullException(nameof(prompt));

            var api = Config.CreateAPI();
            OnStreamEvent(prompt, true);
            var response = new StringWriter();
            var maxTokens = ComputeTokens(prompt);
            await foreach (var token in api.Completions.StreamCompletionEnumerableAsync(prompt, max_tokens: maxTokens, temperature: Config.Temperature, model: Config.Model))
            {
                response.Write(token);
                OnStreamEvent(token.ToString());
            }
            return response.ToString();
        }

        public int ComputeTokens(string prompt, int maxTokens = 4097)
        {
            var tokens = (int)(prompt.Length / 2.9);
            if (tokens > maxTokens) throw new ArgumentOutOfRangeException(nameof(prompt), "The prompt exceeds the max number of tokens allowed");

            var requestTokens = (int)((maxTokens - tokens) * 0.9);

            return requestTokens;
        }

        protected virtual void OnStreamEvent(string message, bool isPrompt = false)
        {
            EventHandler<ChatEventArgs> eventHandler = StreamEvent;
            if (eventHandler != null) eventHandler(this, new ChatEventArgs() { IsPrompt = isPrompt, Message = message });

        }



    }
}
