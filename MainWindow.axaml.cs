using Avalonia.Controls;
using Client.Pages;

namespace Client;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Subscribe to login event using the instance from XAML
        var loginPage = this.FindControl<LoginPage>("LoginPage");
        if (loginPage != null)
        {
            loginPage.LoginSuccess += OnLoginSuccess;
        }
    }

    private void OnLoginSuccess(object? sender, LoginPage.LoginSuccessEventArgs e)
    {
        // Create ChatPage with user credentials
        var chatPage = new ChatPage(e.UserId, e.Username);
        
        // Hide login and show chat
        var loginPage = this.FindControl<LoginPage>("LoginPage");
        var chatPageContainer = this.FindControl<ContentControl>("ChatPageContainer");
        
        if (loginPage != null && chatPageContainer != null)
        {
            loginPage.IsVisible = false;
            chatPageContainer.Content = chatPage;
            chatPageContainer.IsVisible = true;
        }
    }
}