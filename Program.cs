using System;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using System.Text.Json;
using System.IO;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Linq;

namespace CSharpTelegramBot
{
    static class Program
    {
        public static TelegramBotClient Worker = new TelegramBotClient("1743603163:AAF6GACSicfQPets7eYxjhiV-YTy3N8AL48");

        public static Game[] games = new Game[10];
        public static long[] chats = new long[10];
        public enum GameType
        {
            XORO,
            SUEFA
        }

        static void Main(string[] args)
        {
            var me = Worker.GetMeAsync().Result;
            Worker.OnMessage += Worker_OnMessage;
            Worker.StartReceiving();
            Console.ReadLine();
            Worker.StopReceiving();

        }

        async static void Worker_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
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
                        await Worker.SendTextMessageAsync(message.Chat.Id, "Выбери игру", replyMarkup: GetButtons());
                        break;
                    case "/xoro":
                        CreateMatchmaking(message, XORO.generalGameId);
                        break;
                    case "/suefa":
                        CreateMatchmaking(message, SUEFA.generalGameId);
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
                    new List<KeyboardButton> { new KeyboardButton { Text = "/xoro" }, new KeyboardButton { Text = "/suefa" } }
                }
            };
        }
        async static void CreateMatchmaking(Telegram.Bot.Types.Message message, int generalGameId)
        {
            if (!chats.Contains(message.Chat.Id))
                for (int i = 0; i < chats.Length; i++) //finding free space in array of games
                {
                    if (games[i] == null)
                    {
                        await Worker.SendTextMessageAsync(message.Chat.Id, $"Игра создана, {i}");
                        switch(generalGameId)
                        {
                            case XORO.generalGameId:
                                games[i] = new XORO(message, i);
                                break;
                            case SUEFA.generalGameId:
                                games[i] = new SUEFA(message, i);
                                break;
                            default:
                                break;
                        }
                        chats[i] = message.Chat.Id; //creating space for new chat
                        games[i].StartGame();
                        break;
                    }
                }
        }

    }
}
