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
    public MainWindow()
    {
        InitializeComponent();
        
        // Subscribe to login event
        LoginPage.LoginSuccess += OnLoginSuccess;
    }

    private void OnLoginSuccess(object? sender, LoginPage.LoginSuccessEventArgs e)
    {
        // Create ChatPage with user credentials
        var chatPage = new ChatPage(e.UserId, e.Username);
        
        // Hide login and show chat
        LoginPage.IsVisible = false;
        ChatPageContainer.Content = chatPage;
        ChatPageContainer.IsVisible = true;
    }
}