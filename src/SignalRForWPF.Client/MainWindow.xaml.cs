// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The main window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SignalRForWPF.Client
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    using Microsoft.AspNetCore.SignalR.Client;

    using SignalRForWPF.Shared;

    /// <inheritdoc cref="INotifyPropertyChanged"/>
    /// <summary>
    /// The main window.
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged"/>
    public partial class MainWindow : INotifyPropertyChanged
    {
        /// <summary>
        /// The client identifier.
        /// </summary>
        private readonly string clientId = BitConverter
            .ToString(
                new SHA256Managed().ComputeHash(
                    Encoding.UTF8.GetBytes(new Random().NextDouble().ToString(CultureInfo.InvariantCulture))))
            .Replace("-", string.Empty).ToLowerInvariant();

        /// <summary>
        /// The connection.
        /// </summary>
        private readonly HubConnection connection;

        /// <summary>
        /// The received text.
        /// </summary>
        private string receivedText;

        /// <summary>
        /// The sent text.
        /// </summary>
        private string sentText;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
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

        /// <summary>
        /// Handles the property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the received text.
        /// </summary>
        public string ReceivedText
        {
            get => this.receivedText;
            set
            {
                this.receivedText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the sent text.
        /// </summary>
        public string SentText
        {
            get => this.sentText;
            set
            {
                this.sentText = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Handles the property changed event.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Handles the send button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
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