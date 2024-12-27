using OpenAI;
using OpenAI.Chat;

namespace LazyMoon.Class.Service
{
    public class OpenAIService
    {
        private readonly string apiKey = string.Empty;

        private readonly ChatClient? openAiClient;

        public List<ChatMessage> ChatMessages { get; private set; } = [];
        public OpenAIService(IConfiguration configuration)
        {
            if (configuration.GetRequiredSection("OpenAI:ApiKey").Value is string configurationApiKey)
            {
                apiKey = configurationApiKey;
                openAiClient = new ChatClient("gpt-4o-mini", apiKey);
            }
        }


        public async Task<string> SendMessageAsync(string message)
        {
            if (openAiClient is null)
            {
                return string.Empty;
            }

            ChatMessages.Add(ChatMessage.CreateUserMessage(message));
            var result = await openAiClient.CompleteChatAsync(ChatMessages);
            var getResponse = result.Value.Content[0].Text;

            ChatMessages.Add(ChatMessage.CreateAssistantMessage(getResponse));

            return getResponse;
        }

        public async Task<string> SendImageAsync(MemoryStream memoryStream, string message)
        {
            if (openAiClient is null)
            {
                return string.Empty;
            }
            byte[] imageBytes = memoryStream.ToArray();
            var image = BinaryData.FromStream(new MemoryStream(imageBytes));
            var part = ChatMessageContentPart.CreateImagePart(image, "image/png", ChatImageDetailLevel.Low);
            ChatMessages.Add(ChatMessage.CreateUserMessage(message));
            ChatMessages.Add(ChatMessage.CreateUserMessage(part));
            var result = await openAiClient.CompleteChatAsync(ChatMessages);
            var getResponse = result.Value.Content[0].Text;
            ChatMessages.Add(ChatMessage.CreateAssistantMessage(getResponse));
            return getResponse;
        }
    }
}
