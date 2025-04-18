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
        static int Id;
        private static Dictionary<long, StudentState> usersState = new Dictionary<long, StudentState>();
        private static Dictionary<long, StudentRequest> studentState = new Dictionary<long, StudentRequest>();

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
                    if (usersState.TryGetValue(message.Chat.Id, out var state))
                    {
                        switch (state)
                        {
                            case StudentState.AwaitingPostFio:
                                studentState[message.Chat.Id] = new StudentRequest { fio = message.Text };
                                usersState[message.Chat.Id] = StudentState.AwaitingPostIdSpec;
                                await client.SendMessage(message.Chat.Id, "А теперь введи айди специальности");
                                break;
                            case StudentState.AwaitingPostIdSpec:
                                if (studentState.TryGetValue(message.Chat.Id, out var student))
                                {
                                    student.idSpec = int.Parse(message.Text);
                                    usersState[message.Chat.Id] = StudentState.AwaitingPostBirhtday;
                                    await client.SendMessage(message.Chat.Id, "А теперь введи Дату Рождения студента в числовом формате");

                                }
                                break;
                            case StudentState.AwaitingPostBirhtday:
                                if (studentState.TryGetValue(message.Chat.Id, out student))
                                {
                                    student.birthday = int.Parse(message.Text);
                                    usersState[message.Chat.Id] = StudentState.AwaitingPostUchebnoeZav;
                                    await client.SendMessage(message.Chat.Id, "А теперь введи прошлое учебное заведение студента");

                                }
                                break;
                            case StudentState.AwaitingPostUchebnoeZav:
                                if (studentState.TryGetValue(message.Chat.Id, out student))
                                {
                                    student.uchebnoeZav = message.Text;
                                    usersState[message.Chat.Id] = StudentState.AwaitingPostPhoneNumber;
                                    await client.SendMessage(message.Chat.Id, "А теперь введи номер телефона студента");

                                }
                                break;
                            case StudentState.AwaitingPostPhoneNumber:
                                if (studentState.TryGetValue(message.Chat.Id, out student))
                                {
                                    student.phoneNumber = message.Text;
                                    usersState[message.Chat.Id] = StudentState.AwaitingPostAdress;
                                    await client.SendMessage(message.Chat.Id, "А теперь введи адрес студента");

                                }
                                break;
                            case StudentState.AwaitingPostAdress:
                                if (studentState.TryGetValue(message.Chat.Id, out student))
                                {
                                    student.address = message.Text;
                                    usersState[message.Chat.Id] = StudentState.AwaitingPostAge;
                                    await client.SendMessage(message.Chat.Id, "А теперь введи возраст студента числом");

                                }
                                break;
                            case StudentState.AwaitingPostAge:
                                if (studentState.TryGetValue(message.Chat.Id, out student))
                                {
                                    student.age = int.Parse(message.Text);
                                    usersState[message.Chat.Id] = StudentState.AwaitingFinalPost;

                                    var studentJson = JsonSerializer.Serialize(student);
                                    var content = new StringContent(studentJson, Encoding.UTF8, "application/json");
                                    var response = await apiClient.PostAsync($"{urlApi}/Student", content);

                                    await client.SendMessage(message.Chat.Id, "Студент добавлен");
                                    usersState.Remove(message.Chat.Id);
                                    //await client.SendMessage(message.Chat.Id, "Отправляем студента...");
                                }
                                break;
                            case StudentState.AwaitingFinalPost:
                                if (studentState.TryGetValue(message.Chat.Id, out var model))
                                {
                                    var studentJson = JsonSerializer.Serialize(model);
                                    var content = new StringContent(studentJson, Encoding.UTF8, "application/json");
                                    var response = await apiClient.PostAsync($"{urlApi}/Student", content);
                                    //await post(model);
                                    await client.SendMessage(message.Chat.Id, "Студент добавлен");
                                    usersState.Remove(message.Chat.Id);
                                }
                                break;
                            case StudentState.AwaitingGetId:
                                Id = int.Parse(message.Text);
                                await client.SendMessage(message.Chat.Id, "А теперь введи новое имя");
                                usersState[message.Chat.Id] = StudentState.AwaitingUpdateFio;
                                break;
                            case StudentState.AwaitingUpdateFio:
                                studentState[message.Chat.Id] = new StudentRequest { fio = message.Text };
                                await client.SendMessage(message.Chat.Id, "А теперь обнови айди специальности");
                                usersState[message.Chat.Id] = StudentState.AwaitingUpdateIdSpec;
                                break;
                            case StudentState.AwaitingUpdateIdSpec:
                                if (studentState.TryGetValue(message.Chat.Id, out student))
                                {
                                    student.idSpec = int.Parse(message.Text);
                                    await client.SendMessage(message.Chat.Id, "А теперь обнови дату рождения студента");
                                    usersState[message.Chat.Id] = StudentState.AwaitingUpdateBirhtday;
                                }
                                break;
                            case StudentState.AwaitingUpdateBirhtday:
                                if (studentState.TryGetValue(message.Chat.Id, out student))
                                {
                                    student.birthday = int.Parse(message.Text);
                                    await client.SendMessage(message.Chat.Id, "А теперь обнови учебное заведение");
                                    usersState[message.Chat.Id] = StudentState.AwaitingUpdateUchebnoeZav;
                                }
                                break;
                            case StudentState.AwaitingUpdateUchebnoeZav:
                                if (studentState.TryGetValue(message.Chat.Id, out student))
                                {
                                    student.uchebnoeZav = message.Text;
                                    await client.SendMessage(message.Chat.Id, "А теперь введи номер телефона студента");
                                    usersState[message.Chat.Id] = StudentState.AwaitingUpdatePhoneNumber;

                                }
                                break;
                            case StudentState.AwaitingUpdatePhoneNumber:
                                if (studentState.TryGetValue(message.Chat.Id, out student))
                                {
                                    student.phoneNumber = message.Text;
                                    await client.SendMessage(message.Chat.Id, "А теперь введи адрес студента");
                                    usersState[message.Chat.Id] = StudentState.AwaitingUpdateAdress;
                                }
                                break;
                            case StudentState.AwaitingUpdateAdress:
                                if (studentState.TryGetValue(message.Chat.Id, out student))
                                {
                                    student.address = message.Text;
                                    await client.SendMessage(message.Chat.Id, "А теперь введи возраст студента числом");
                                    usersState[message.Chat.Id] = StudentState.AwaitingUpdateAge;
                                }
                                break;
                            case StudentState.AwaitingUpdateAge:
                                if (studentState.TryGetValue(message.Chat.Id, out student))
                                {
                                    student.age = int.Parse(message.Text);
                                    var studentJson = JsonSerializer.Serialize(student);
                                    var content = new StringContent(studentJson, Encoding.UTF8, "application/json");
                                    var response = await apiClient.PutAsync($"{urlApi}/Student/{Id}", content);
                                    await client.SendMessage(message.Chat.Id, "Студент обновлен");
                                    usersState.Remove(message.Chat.Id);
                                }
                                break;
                            case StudentState.AwaitingDeleteById:
                                Id = int.Parse(message.Text);
                                var response1 = await apiClient.DeleteAsync($"{urlApi}/Student/{Id}");
                                await client.SendMessage(message.Chat.Id, "удален");
                                break;
                        }

                    }
                    if (message.Text != null && message.Type == MessageType.Text)
                        Console.WriteLine($"Пришло сообщение в {DateTime.Now} от @{message.From.Username}: текст сообщения - {message.Text}");
                    if (message.Text == "/start")
                        await client.SendMessage(message.Chat.Id, "привет");
                    switch (message.Text)
                    {
                        case "Ок":
                            var listbutton = new InlineKeyboardMarkup(new[]
                            {
                        new [] {InlineKeyboardButton.WithCallbackData("Получить","give")},
                        new [] {InlineKeyboardButton.WithCallbackData("Отправить","send")},
                        new [] {InlineKeyboardButton.WithCallbackData("Обновить студента","update")},
                        new [] {InlineKeyboardButton.WithCallbackData("Удалить студента","delete")}
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
                            responseMessage = $"Студенты: {string.Join("\n", student.Select(s => s.fio))}";
                            break;
                        case "send":
                            responseMessage = "Введите имя студента";
                            usersState[callback.Message.Chat.Id] = StudentState.AwaitingPostFio;
                            break;
                        case "update":
                            responseMessage = "Введите айди студента";
                            usersState[callback.Message.Chat.Id] = StudentState.AwaitingGetId;
                            break;
                        case "delete":
                            responseMessage = "Введите айди студента";
                            usersState[callback.Message.Chat.Id] = StudentState.AwaitingDeleteById;
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
        public static async Task post(StudentRequest student)
        {
            var studentJson = JsonSerializer.Serialize(student);
            var content = new StringContent(studentJson, Encoding.UTF8, "application/json");
            var response = await apiClient.PostAsync($"{urlApi}/Student", content);
        }
    }
}
