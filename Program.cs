using System;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using System.Text.Json;
using System.IO;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;

namespace CSharpTelegramBot
{
    static class Program
    {
        public static TelegramBotClient Worker = new TelegramBotClient("1743603163:AAF6GACSicfQPets7eYxjhiV-YTy3N8AL48");
        static void Main(string[] args)
        {
            var me = Worker.GetMeAsync().Result;
            Worker.OnMessage += Worker_OnMessage;
            Worker.StartReceiving();
            Console.ReadLine();
            Worker.StopReceiving();

        }

        public async static void Worker_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;
            if (message == null)
                return;
            if (message.Type != MessageType.Text)
                return;
            else
            {
                switch (message.Text)
                {
                    case "/start":
                        await Worker.SendTextMessageAsync(message.Chat.Id, "Чтобы поиграть крестики нолики набери /xoro", replyMarkup: GetButtons());
                        break;
                    case "/xoro":
                        XORO x1 = new XORO();
                        await x1.PlayGame(message);
                        break;

                    default:
                        break;
                }
            }
            
        }

        public static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "/xoro" } }
                }
            };
        }
    }
}
