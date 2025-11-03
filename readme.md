# ChatHub Client - Avalonia

A Simple real-time chat application client built with Avalonia UI and SignalR.

## 🚀 Features

- **User Authentication**: Register and login functionality
- **Real-time Messaging**: Send and receive messages instantly using SignalR
- **User Presence**: See who's online and get notified when users connect/disconnect
- **Message History**: View previous messages when joining the chat
- **Cross-platform**: Works on Windows, Linux, and macOS

## 📋 Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- A running ChatHub server (default: `http://localhost:3000`)

## 🛠️ Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/Malcovys/Simple-chathub-Client-Avalonia.git
   cd Simple-chathub-Client-Avalonia/Client
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Build the project**:
   ```bash
   dotnet build
   ```

## ▶️ Running the Application

1. **Start the ChatHub server** (if not already running):
   - Ensure the server is running on `http://localhost:3000`
   - Update `SharedHttpClient.cs` if your server uses a different URL

2. **Run the client**:
   ```bash
   dotnet run
   ```

## 📖 How to Use

### First Time Users

1. **Registration**:
   - Launch the application
   - Enter a unique username
   - Enter a password
   - Click the **Register** button
   - Wait for the success message: "Registration successful! You can now login."

2. **Login**:
   - Enter your username
   - Enter your password
   - Click the **Login** button

### Using the Chat

1. **View Connected Users**:
   - The left panel shows all currently connected users
   - Your username appears at the top with "(ID: X)"

2. **Send Messages**:
   - Type your message in the text box at the bottom
   - Click the **Send** button or press Enter
   - Your message will appear in the chat and be sent to all connected users

3. **Receive Messages**:
   - Messages from other users appear in real-time in the center panel
   - Format: `Username: Message content`

4. **Notifications**:
   - See when users join: "Username is connected."
   - See when users leave: "Username has been disconnected."

## 🔧 Configuration

### Server URL

To change the server URL, edit `SharedHttpClient.cs`:

```csharp
public static class SharedHttpClient
{
    public static readonly HttpClient client = new()
    {
        BaseAddress = new Uri("http://your-server-url:port")
    };
}
```

## 📦 Dependencies

- **Avalonia**: Cross-platform UI framework
- **Microsoft.AspNetCore.SignalR.Client**: Real-time communication

Install dependencies:
```bash
dotnet add package Microsoft.AspNetCore.SignalR.Client
```

## 🐛 Troubleshooting

### "Connection error" message
- Ensure the ChatHub server is running
- Check that the server URL in `SharedHttpClient.cs` is correct
- Verify your firewall settings

### "Username already in use"
- Choose a different username
- The username must be unique across all connected users

### "Invalid credentials"
- Double-check your username and password
- Usernames and passwords are case-sensitive

## 👤 Author

**Malcovys**
- GitHub: [@Malcovys](https://github.com/Malcovys)
- Repository: [Simple-chathub-Client-Avalonia](https://github.com/Malcovys/Simple-chathub-Client-Avalonia)