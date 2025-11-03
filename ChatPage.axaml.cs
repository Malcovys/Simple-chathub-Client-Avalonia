using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Microsoft.AspNetCore.SignalR.Client;

namespace Client;

public partial class ChatPage : UserControl
{  
    private record class User(int? Id, string Name);
    private record class Message(int UserId, string Content);

    private HubConnection? _connection;
    private readonly HttpClient _httpClient = SharedHttpClient.client;

    private readonly int _userId;
    private readonly string _userName;

    private List<User> _connectedUsers = new List<User>();
    private List<Message> _messages = new List<Message>();

    public ChatPage(int userId, string userName)
    {
        _userId = userId;
        _userName = userName;

        InitializeComponent();
        
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            InitializeWebSocketConnection();
            await LoadConnectedUsers();
            await LoadOldMessages();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Initialization error: {ex}");
            PushNotification($"❌ Initialization error: {ex.Message}");
        }
    }

    private async Task LoadConnectedUsers()
    {
        UsersList.Items.Add($"• You (ID: {_userId})");
        await GetAllFromJsonConnectedUsers();
    }

    private async Task LoadOldMessages()
    {
        await GetAllFromJsonMessages();
    }

    private void PushMessage(Message message)
    {
        var sender = _connectedUsers.FirstOrDefault(u => u.Id == message.UserId);
        var displayName = sender != null ? sender.Name : message.UserId.ToString();
        Dispatcher.UIThread.Post(() => MessagesList.Items.Add($"{displayName}: {message.Content}."));
    }

    private void PushCurrentUserMessage(string content)
    {
        MessagesList.Items.Add($"You: {content}.");
    }

    private void PushNotification(string notification)
    {
        MessagesList.Items.Add(notification);
    }

    private void PushConnectedUser(User user)
    {
        UsersList.Items.Add($"• {user.Name} (ID: {user.Id})");
    }

    private async void SendButton_Click(object sender, RoutedEventArgs e)
    {
        if (MessageInput.Text is null) return;

        try
        {
            await PostMessageAsJson(MessageInput.Text);
            PushCurrentUserMessage(MessageInput.Text);
            MessageInput.Text = string.Empty;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            PushNotification($"❌ Send error: {ex.Message}");
        }
    }

    private void OnReceiveMessage(Message message)
    {
        _messages.Add(message);
        PushMessage(message);
    }

    private void OnNewUserConnected(User user)
    {
        _connectedUsers.Add(user);
        PushConnectedUser(user);
        Dispatcher.UIThread.Post(() => PushNotification($"{user.Name} is connected."));
    }
    
    private void OnHasUserDesconnected(int userId)
    {
        var disconnectedUser = _connectedUsers.FirstOrDefault((usr) => usr.Id == userId);

        if (disconnectedUser is null)
            return;
            
        _connectedUsers.Remove(disconnectedUser);
        Dispatcher.UIThread.Post(() => PushNotification($"{disconnectedUser.Name} has been disconnected."));
    }

    private async Task PostMessageAsJson(string content)
    {
        var message = new { UserId = _userId, Content = content };
        var response = await _httpClient.PostAsJsonAsync("messages", message);
        response.EnsureSuccessStatusCode();
    }
    
    private async Task GetAllFromJsonMessages()
    {
        var messages = await _httpClient.GetFromJsonAsync<List<Message>>("messages");
        messages?.ForEach(_messages.Add);

        if (messages != null && messages.Count > 0)
        {   
            foreach (var message in messages)
            {
                var sender = _connectedUsers.FirstOrDefault(u => u.Id == message.UserId);
                var displayName = sender != null ? sender.Name : message.UserId.ToString();   
                MessagesList.Items.Add($"{displayName}: {message.Content}.");
            }
        }
    }

    private async Task GetAllFromJsonConnectedUsers()
    {
        var users = await _httpClient.GetFromJsonAsync<List<User>>("users/connected");
        users?.ForEach(_connectedUsers.Add);

        if (users != null && users.Count > 0)
        {
            foreach (var user in users)
            {
                PushConnectedUser(user);
            }
        }
    }
    
    private async void InitializeWebSocketConnection()
    {
        try
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"http://localhost:3000/ChatHub?userId={_userId}")
                .Build();

            _connection.On<Message>("ReceiveMessage", OnReceiveMessage);

            _connection.On<User>("UserConnected", OnNewUserConnected);

            _connection.On<int>("UserDisconnected", OnHasUserDesconnected);

            await _connection.StartAsync();

            PushNotification("You are online.");
        }
        catch (Exception ex)
        {
            PushNotification($"❌ Connection error: {ex.Message}.");

            Debug.WriteLine($"Exception type: {ex.GetType().Name}");
            Debug.WriteLine($"Full exception: {ex}");
        }
    }
}