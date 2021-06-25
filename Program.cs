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
        public static long[] chats = new long[10];
        public static XORO[] games = new XORO[10];
        static int gameId = 0;
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
                        if (chats.Contains(message.Chat.Id)) 
                        {
                            await Worker.SendTextMessageAsync(message.Chat.Id, "Игра существует, присоединение");
                            int findId = Array.IndexOf(chats, message.Chat.Id); //finding game that already started
                            await games[findId].ContinueGame(); //continue game
                        }
                        else
                        {
                            await Worker.SendTextMessageAsync(message.Chat.Id, "Игра создана");
                            for (int i = 0; i < chats.Length; i++) //finding free space in array games
                            {
                                if (games[i] == null)
                                {
                                    games[i] = new XORO(message, gameId); //creating new game
                                    chats[i] = message.Chat.Id; //creating space for new chat
                                    await games[i].StartGame();
                                    break;
                                }
                            }

                        }
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
