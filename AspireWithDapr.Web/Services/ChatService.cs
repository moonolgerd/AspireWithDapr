using Microsoft.Extensions.AI;

namespace AspireWithDapr.Web.Services;

public class ChatService(IChatClient chatClient, ILogger<ChatService> logger)
{
    public async Task<string> SendMessageAsync(string message)
    {
        try
        {
            var chatMessages = new List<ChatMessage>
            {
                new(ChatRole.System, "You are a helpful AI assistant using the phi-3.5-mini model."),
                new(ChatRole.User, message)
            };

            var options = new ChatOptions
            {
                Tools = [],
                Temperature = 0.7f,
            };

            var response = await chatClient.GetResponseAsync(chatMessages);
            
            return response.Text ?? "I apologize, but I couldn't generate a response.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing chat message: {Message}", message);
            return "Sorry, I encountered an error while processing your request.";
        }
    }

    public async IAsyncEnumerable<string> SendMessageStreamAsync(string message)
    {
        var chatMessages = new List<ChatMessage>
        {
            new(ChatRole.System, "You are a helpful AI assistant using the phi-3.5-mini model."),
            new(ChatRole.User, message)
        };

        await foreach (var update in chatClient.GetStreamingResponseAsync(chatMessages))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                yield return update.Text;
            }
        }
    }
}