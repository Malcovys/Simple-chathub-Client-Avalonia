using System;
using System.Net.Http;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Client;

public partial class LoginPage : UserControl
{
    private readonly HttpClient _httpClient = SharedHttpClient.client;

    public event EventHandler<LoginSuccessEventArgs>? LoginSuccess;
    public class LoginSuccessEventArgs : EventArgs
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
    }

    public LoginPage()
    {
        InitializeComponent();
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        string username = UsernameInput.Text ?? "";
        string password = PasswordInput.Text ?? "";
        
        if (string.IsNullOrWhiteSpace(username))
        {
            // TODO: Show error message
            return;
        }

        // TODO: Validate credentials with server
        // For now, just use a dummy user ID
        int userId = 1;
        
        // Raise login success event
        LoginSuccess?.Invoke(this, new LoginSuccessEventArgs 
        { 
            UserId = userId, 
            Username = username 
        });
    }
}