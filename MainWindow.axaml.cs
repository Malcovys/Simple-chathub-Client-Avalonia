using System;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Interactivity;
using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Collections.Generic;

namespace Client;

public partial class MainWindow : Window
{
    private HubConnection? connection;
    private static HttpClient httpClient = new()
    {
        BaseAddress = new Uri("http://localhost:3000")
    };

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void connectButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:3000/ChatHub?userId=1")
                .Build();

            connection.On<int, string>("ReceiveMessage", (userId, content) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    messagesList.Items.Add($"User {userId}: {content}");
                });
            });

            connection.On<int>("UserConnected", (userId) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    messagesList.Items.Add($"✓ User {userId} connected");
                });
            });

            connection.On<int>("UserDisconnected", (userId) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    messagesList.Items.Add($"✗ User {userId} disconnected");
                });
            });

            await connection.StartAsync();
            
            messagesList.Items.Add("✓ Connection started");
            connectButton.IsEnabled = false;
            sendButton.IsEnabled = true;
        }
        catch (Exception ex) // PAS HubException, juste Exception!
        {
            // L'exception contient le message de HubException dans ex.Message
            messagesList.Items.Add($"❌ Connection error: {ex.Message}");
            
            // Pour le debug complet:
            Debug.WriteLine($"Exception type: {ex.GetType().Name}");
            Debug.WriteLine($"Full exception: {ex}");
        }
    }

    private async void sendButton_Click(object sender, RoutedEventArgs e)
    {
        if (connection?.State != HubConnectionState.Connected)
        {
            messagesList.Items.Add("❌ Not connected to server");
            return;
        }

        try
        {
            await connection.InvokeAsync("SendMessage", userTextBox.Text, messageTextBox.Text);
            messageTextBox.Text = string.Empty; // Vider le champ après envoi
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            messagesList.Items.Add($"❌ Send error: {ex.Message}");
        }
    }

    private async void sendRequestButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await GetAllFromJsonConnectedUsers();
        }
        catch (Exception ex)
        {
            messagesList.Items.Add($"❌ Request error: {ex.Message}");
            Console.WriteLine(ex);
        }
    }

    private async Task GetAllFromJsonConnectedUsers()
    {
        try
        {
            var users = await httpClient.GetFromJsonAsync<List<User>>("users/connected");
            
            messagesList.Items.Add("=== Connected Users ===");
            if (users != null && users.Count > 0)
            {
                foreach (var user in users)
                {
                    messagesList.Items.Add($"• {user.Name} (ID: {user.Id})");
                }
            }
            else
            {
                messagesList.Items.Add("No users connected");
            }
        }
        catch (HttpRequestException ex)
        {
            messagesList.Items.Add($"❌ HTTP error: {ex.Message}");
        }
        catch (OperationCanceledException ex) when (ex.InnerException is TimeoutException tex)
        {
            messagesList.Items.Add($"❌ Timeout: {tex.Message}");
        }
    }
}

public record class User(int? Id, string Name);