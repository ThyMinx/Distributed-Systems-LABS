using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected HubConnection connection;
        public MainWindow()
        {
            connection = new HubConnectionBuilder().WithUrl("http://localhost:60008/ChatHub").Build();//.WithUrl("http://192.168.x.xxx:5000/ChatHub").Build();

            connection.On<string, string>("GetMessage", //This says when the connection receives a message from the hub with two strings as parameters and the method name “GetMessage” 
                new Action<string, string>((username, message) => //This says create a newaction, which takes two strings which we name username and message and use the delegate
                GetMessage(username, message))); //Finally, we specify the method (that takes the two strings) that we will use in this instanceto handle the GetMessage procedure call.

            InitializeComponent();
        }

        private void GetMessage(string username, string message)
        {
            this.Dispatcher.Invoke(() =>
            {
                var chat = $"{username}: {message}";
                MessageList.Items.Add(chat);
            });
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await connection.StartAsync();
                MessageList.Items.Add("Connection opened");
            }
            catch
            {
                MessageList.Items.Add("Connection failed");
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            //This code tries to invoke a procedure called “BroadcastMessage” on the hub, sending two strings.
            try
            {
                await connection.InvokeAsync("BroadcastMessage", UsernameText.Text, MessageText.Text);
            }
            catch (Exception ex)
            {
                MessageList.Items.Add(ex.Message);
            }
        }
    }
}
