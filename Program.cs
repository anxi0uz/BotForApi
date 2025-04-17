using bot.Models;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
namespace botForApi
{
    
    internal class Program
    {
        public static HttpClient apiClient = new HttpClient();

        private static string apiKey = "7463830147:AAGfWrkwWwx_3OCLnI2gyeiJQS0ePWpIauU";
        static TelegramBotClient client;
        private static string urlApi = "http://localhost:5201";
        
        static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            client = new TelegramBotClient(apiKey, cancellationToken: cts.Token);
            var me = await client.GetMe();
            Console.WriteLine($"id: {me}");
            

            client.StartReceiving(Client_OnMessage, Error);
            Console.ReadKey();
            cts.Cancel();
        }

        private static async Task Error(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        private static async Task Client_OnMessage(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var message = update.Message;

            switch (update.Type)
            {
                case UpdateType.Message:
                    if (message.Text == null) return;
                    if (message.Text != null && message.Type == MessageType.Text)
                        Console.WriteLine($"Пришло сообщение в {DateTime.Now} от @{message.From.Username}: текст сообщения - {message.Text}");
                    if (message.Text == "/start")
                        await client.SendMessage(message.Chat.Id, "привет");
                    switch (message.Text)
                    {
                        case "Ок":
                            var listbutton = new InlineKeyboardMarkup(new[]
                            {
                        new [] {InlineKeyboardButton.WithCallbackData("получить","give")},
                        new [] {InlineKeyboardButton.WithCallbackData("отправить","send")}
                    });
                            await client.SendMessage(message.Chat.Id, "Вы написали ок", replyMarkup: listbutton);
                            break;
                    }
                    break;
                case UpdateType.CallbackQuery:
                    var callback = update.CallbackQuery;
                    string responseMessage = string.Empty;
                    switch (callback.Data)
                    {
                        case "give":
                            var response = await apiClient.GetAsync($"{urlApi}/Student");
                            var content = await response.Content.ReadAsStringAsync();
                            var student = JsonSerializer.Deserialize<List<Studdent>>(content);
                            responseMessage = $"Студенты: {string.Join("\n",student.Select(s=>s.fio))}";
                            break;
                        case "send":
                            responseMessage = "Отправляем";
                            break;
                        default:
                            responseMessage = "Ппп";
                            break;
                    }
                    await client.SendMessage(callback.Message.Chat.Id, responseMessage);
                    await client.AnswerCallbackQuery(callback.Id);
                    break;
            }
        }
        public async Task post(Studdent student)
        {
            var studentJson = JsonSerializer.Serialize(student);
            var content = new StringContent(studentJson,Encoding.UTF8, "application/json");
            var response = await apiClient.PostAsync($"{urlApi}/Student",content);
        }
    }
}
