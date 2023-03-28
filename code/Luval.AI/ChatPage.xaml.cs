using Luval.AI.ViewModels;
using System.Security;

namespace Luval.AI
{
    public partial class MainPage : ContentPage
    {

        public ChatViewModel ChatViewModel { get; private set; }

        public MainPage(SecureString apiKey)
        {
            InitializeComponent();

            ChatViewModel = new ChatViewModel(apiKey);
            ChatViewModel.StreamingStarted += ChatViewModel_StreamingStarted;
            ChatViewModel.StreamingEnded += ChatViewModel_StreamingEnded;
            ChatViewModel.StreamingException += ChatViewModel_StreamingException;

            BindingContext = ChatViewModel;
        }

        private void ChatViewModel_StreamingException(object sender, ExceptionEventArgs e)
        {
            var task = Shell.Current.DisplayAlert("Error", e.Message, "OK");
            task.Wait();
        }

        private void ChatViewModel_StreamingEnded(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ChatViewModel_StreamingStarted(object sender, EventArgs e)
        {
           
        }
    }
}