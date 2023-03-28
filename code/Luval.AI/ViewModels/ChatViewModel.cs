using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Luval.OpenAI;
using Luval.OpenAI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Luval.AI.ViewModels
{
    public partial class ChatViewModel : ObservableObject
    {
        public ChatViewModel(SecureString apiKey)
        {
            ChatConfig = new ChatConfig()
            {
                ApiKey = apiKey,
            };
            ChatManager = new ChatManager(ChatConfig);
            ChatManager.StreamEvent += ChatManager_StreamEvent;
            Messages = new ObservableCollection<ChatMessage>();
        }

        public event EventHandler<EventArgs> StreamingStarted;
        public event EventHandler<EventArgs> StreamingEnded;
        public event EventHandler<ChatEventArgs> StreamingEvent;
        public event EventHandler<ExceptionEventArgs> StreamingException;


        private void ChatManager_StreamEvent(object sender, ChatEventArgs e)
        {
            if (e == null) return;
            if (!IsRunning) OnStreamingStarted();
            //if (Response != null) Response.Content += e.Message;

            StreamingEvent?.Invoke(this, e);
        }

        protected virtual void OnStreamingStarted()
        {
            StreamingStarted?.Invoke(this, new EventArgs());
            IsRunning = true;
        }

        protected virtual void OnStreamingEnded()
        {
            StreamingEnded?.Invoke(this, new EventArgs());
            IsRunning = false;
        }

        protected virtual void OnStreamingException(string message, Exception exception)
        {
            StreamingException?.Invoke(this, new ExceptionEventArgs() { Message = message, Exception = exception });
            IsRunning = false;
        }

        #region Properties

        public ChatConfig ChatConfig { get; private set; }
        public ChatManager ChatManager { get; private set; }

        #endregion

        #region Model Properties

        [ObservableProperty]
        ObservableCollection<ChatMessage> messages;

        [ObservableProperty]
        ChatMessage response;

        [ObservableProperty]
        string query;

        [ObservableProperty]
        bool isRunning;

        #endregion

        [RelayCommand]
        void Submit()
        {
            if (string.IsNullOrWhiteSpace(Query)) return;
            if (IsRunning) return;

            var response = default(string);

            Messages.Add(new ChatMessage() { IsResponse = false, Content = Query });
            Task.Delay(new TimeSpan(1000000));
            Response = new ChatMessage() { IsResponse = true, Content = response };
            Messages.Add(Response);
            try
            {
                IsRunning = true;
                response = ChatManager.RunPrompt(Query).Result;
                Response.Content = response;
            }
            catch (Exception ex)
            {
                OnStreamingException("Unable to run the command", ex);
            }
            finally
            {
                OnStreamingEnded();
            }

            if (string.IsNullOrWhiteSpace(response)) return;
        }
    }
}
