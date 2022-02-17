// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestHub.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The test hub class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SignalRForWPF.Server;

/// <summary>
/// The test hub class.
/// </summary>
public class TestHub : Hub
{
    /// <summary>
    /// Sends a message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    public async Task SendMessage(Message message)
    {
        Console.WriteLine($"Received message {message.MessageText} from client {message.User}");
        await this.Clients.All.SendAsync("ReceiveMessage", message);
    }
}
