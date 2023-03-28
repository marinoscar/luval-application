using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAI_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

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
            var result = await api.Completions.CreateCompletionAsync(prompt, max_tokens: maxTokens, temperature: Config.Temperature, model: Config.Model);
            foreach (var completion in result.Completions)
            {
                OnStreamEvent(completion.Text);
            }
            //await foreach (var token in api.Completions.StreamCompletionEnumerableAsync(prompt, max_tokens: maxTokens, temperature: Config.Temperature, model: Config.Model))
            //{
            //    response.Write(token);
            //    OnStreamEvent(token.ToString());
            //}
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

        protected async virtual Task<JObject> RunRequest(string prompt, string endpoint, SecureString apiKey, double maxTokens, JObject payload)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", new NetworkCredential("", apiKey).Password);

                var requestBody = new
                {
                    model = "text-davinci-003",
                    prompt = prompt,
                    max_tokens = maxTokens,
                    temperature = 0.7d
                };

                var jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

                var httpResponse = await httpClient.PostAsync(endpoint, new StringContent(jsonRequestBody, Encoding.UTF8, "application/json"));

                if (httpResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await httpResponse.Content.ReadAsStringAsync();

                }
                else
                    throw new Exception($"HTTP {httpResponse.StatusCode}: {httpResponse.ReasonPhrase}");

                return JsonConvert.SerializeObject(await httpResponse.Content.ReadAsStringAsync());
            }
        }
    }
}
