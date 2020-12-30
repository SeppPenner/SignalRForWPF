using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using SignalRForWPF.Client.Properties;
using SignalRForWPF.Shared;

namespace SignalRForWPF.Client
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly string clientId = BitConverter
            .ToString(
                new SHA256Managed().ComputeHash(
                    Encoding.UTF8.GetBytes(new Random().NextDouble().ToString(CultureInfo.InvariantCulture))))
            .Replace("-", string.Empty).ToLowerInvariant();

        private readonly HubConnection connection;

        private string receivedText;

        private string sentText;

        public MainWindow()
        {
            this.DataContext = this;

            this.InitializeComponent();

            this.connection = new HubConnectionBuilder().WithUrl("https://localhost:5001/testHub").Build();

            this.connection.StartAsync();

            this.connection.Closed += async error =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await this.connection.StartAsync();
            };

            this.connection.On<Message>(
                "ReceiveMessage",
                message =>
                {
                    this.Dispatcher.Invoke(
                        () => { this.ReceivedText += $"{message.User}: {message.MessageText}{Environment.NewLine}"; });
                });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ReceivedText
        {
            get => this.receivedText;
            set
            {
                this.receivedText = value;
                this.OnPropertyChanged();
            }
        }

        public string SentText
        {
            get => this.sentText;
            set
            {
                this.sentText = value;
                this.OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void SendButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                await this.connection.InvokeAsync(
                    "SendMessage",
                    new Message { MessageText = this.SentText, User = this.clientId });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}