using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Client.Pages;

public partial class LoginPage : UserControl
{
    private readonly HttpClient _httpClient = SharedHttpClient.client;
    private record AuthResponse(int userId);

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

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        // Clear previous error
        HideError();

        // Validate input
        if (string.IsNullOrWhiteSpace(UsernameInput.Text))
        {
            ShowError("Username is required.");
            return;
        }

        if (string.IsNullOrWhiteSpace(PasswordInput.Text))
        {
            ShowError("Password is required.");
            return;
        }

        string username = UsernameInput.Text;
        string password = PasswordInput.Text;

        try
        {
            int? id = await PostLoginAsJson(username, password);

            if (id is null)
            {
                ShowError("Invalid credentials.");
                return;
            }

            // Raise login success event
            LoginSuccess?.Invoke(this, new LoginSuccessEventArgs
            {
                UserId = id.Value,
                Username = username
            });
        }
        catch(HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            ShowError("Invalid credential. Please check your user name and passord and retry.");
        }
        catch (Exception ex)
        {
            ShowError($"Login error: {ex.Message}");
        }
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        // Clear previous messages
        HideError();
        HideSuccess();

        // Validate input
        if (string.IsNullOrWhiteSpace(UsernameInput.Text))
        {
            ShowError("Username is required.");
            return;
        }

        if (string.IsNullOrWhiteSpace(PasswordInput.Text))
        {
            ShowError("Password is required.");
            return;
        }

        string username = UsernameInput.Text;
        string password = PasswordInput.Text;

        try
        {
            bool success = await PostRegisterAsJson(username, password);

            if (success)
            {
                // Show success message
                ShowSuccess("Registration successful! You can now login.");

                // Wait a bit to show the success message
                await Task.Delay(2000);

                // Clear the form
                UsernameInput.Text = string.Empty;
                PasswordInput.Text = string.Empty;
                HideSuccess();
            }
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            ShowError("Username already in use. Please choose another one.");
        }
        catch (Exception ex)
        {
            ShowError($"Registration error: {ex.Message}");
        }
    }

    private void ShowError(string message)
    {
        ErrorMessage.Text = message;
        ErrorMessage.IsVisible = true;
    }

    private void HideError()
    {
        ErrorMessage.IsVisible = false;
        ErrorMessage.Text = string.Empty;
    }

    private void ShowSuccess(string message)
    {
        SuccessMessage.Text = message;
        SuccessMessage.IsVisible = true;
    }

    private void HideSuccess()
    {
        SuccessMessage.IsVisible = false;
        SuccessMessage.Text = string.Empty;
    }

    private async Task<bool> PostRegisterAsJson(string userName, string password)
    {
        var credentials = new { Name = userName, Password = password };
        var response = await _httpClient.PostAsJsonAsync("auth/register", credentials);
        
        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            throw new HttpRequestException("Username already in use", null, System.Net.HttpStatusCode.Conflict);
        }
        
        response.EnsureSuccessStatusCode();
        return true;
    }
    
    private async Task<int?> PostLoginAsJson(string userName, string password)
    {
        var credentials = new { Name = userName, Password = password };
        var response = await _httpClient.PostAsJsonAsync("auth", credentials);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<AuthResponse>();

        return json?.userId;
    }
}