using System;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TelegramBot
{
    // Прием сообщений и оправка перевода в телеграм-бот
    class TelegramBot
    {
        // Токен телеграм-бота
        private static string token = "5664966752:AAG_KwNizBYgX1OUbvobsELCMGowfjHkpZU";

        // Клиент TelegramBot с использованием токена телеграм-бота
        private static TelegramBotClient telegramBotClient = new TelegramBotClient(token);

        // Прием оригинального сообщения
        public static void StartBot()
        {
            telegramBotClient.StartReceiving();
            telegramBotClient.OnMessage += OnLinkHandler;
            telegramBotClient.OnMessage += OnMessageHandler;
            Console.ReadLine();
            telegramBotClient.StopReceiving();
        }

        // Отправка переведенного сообщения
        private static async void OnMessageHandler(object sender, MessageEventArgs e)
        {
            var message = e.Message;

            if (message.Text != null)
            {
                Console.WriteLine($"Текст сообщения: {message.Text}");

                if (Database.IsCheckTranslation(message.Text))
                {
                    var translation = Database.GetData(message.Text);
                    await telegramBotClient.SendTextMessageAsync(message.Chat.Id, translation, replyToMessageId: message.MessageId);
                }
                else
                {
                    var translation = CloudTranslationAPI.Translation(message.Text);

                    if (message.Text.Length <= 50)
                    {
                        Database.AddData(message.Text, translation);
                    }

                    await telegramBotClient.SendTextMessageAsync(message.Chat.Id, translation, replyToMessageId: message.MessageId);
                }

                Console.WriteLine();
            }
        }

        // Отправка коммерсческой ссылки
        private static async void OnLinkHandler(object sender, MessageEventArgs e)
        {
            var message = e.Message;

            if (message.Text != null)
            {
                var translation = "Учи английский легко\nСкачай сейчас ⬇\nt.ly/pmyS";
                await telegramBotClient.SendTextMessageAsync(message.Chat.Id, translation);
            }
        }
    }
}