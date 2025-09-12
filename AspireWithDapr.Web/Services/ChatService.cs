using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace AspireWithDapr.Web.Services;

public class ChatService(IChatClient chatClient, ILogger<ChatService> logger)
{

    public async Task<string> SendMessageAsync(string message)
    {
        try
        {
            var chatMessages = new List<ChatMessage>
            {
                new(ChatRole.System, "You are a helpful AI assistant using the phi-4-mini model. You have access to Redis data through MCP tools. When users ask about data or caching, you can query Redis to provide accurate information."),
                new(ChatRole.User, message)
            };

            var options = new ChatOptions
            {
                Tools = [.. await GetRedisMcp()],
                Temperature = 0.7f
            };

            var response = await chatClient.GetResponseAsync(chatMessages, options);

            return response.Text ?? "I apologize, but I couldn't generate a response.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing chat message: {Message}", message);
            return "Sorry, I encountered an error while processing your request.";
        }
    }

    private static async Task<IList<McpClientTool>> GetRedisMcp()
    {
        var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
        {
            Name = "Redis MCP Server",
            Command = "uvx",
            Arguments = ["--from", "redis-mcp-server@latest", "redis-mcp-server", "--url", "redis://localhost:6379/0"],
            EnvironmentVariables = new Dictionary<string, string?>
            {
                ["REDIS_PWD"] = "mypassword"
            }
        });
        var client = await McpClientFactory.CreateAsync(clientTransport);
        var tools = await client.ListToolsAsync();
        return tools;
    }

    public async IAsyncEnumerable<string> SendMessageStreamAsync(string message)
    {
        var chatMessages = new List<ChatMessage>
        {
            new(ChatRole.System, "You are a helpful AI assistant using the phi-4-mini model. You have access to Redis data through MCP tools."),
            new(ChatRole.User, message)
        };

        
        var options = new ChatOptions
        {
            Tools = [.. await GetRedisMcp()],
            Temperature = 0.7f
        };

        await foreach (var update in chatClient.GetStreamingResponseAsync(chatMessages, options))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                yield return update.Text;
            }
        }
    }
}