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
        public static long[] XOROchats = new long[10];
        public static XORO[] XOROgames = new XORO[10];
        public static long[] SUEFAchats = new long[10];
        public static SUEFA[] SUEFAgames = new SUEFA[10];
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
                        if (XOROchats.Contains(message.Chat.Id)) 
                        {
                            await Worker.SendTextMessageAsync(message.Chat.Id, "Игра существует, присоединение");
                            int findId = Array.IndexOf(XOROchats, message.Chat.Id); //finding game that already started
                            await XOROgames[findId].ContinueGame(); //continue game
                        }
                        else
                        {
                            for (int i = 0; i < XOROchats.Length; i++) //finding free space in array games
                            {
                                if (XOROgames[i] == null)
                                {
                                    await Worker.SendTextMessageAsync(message.Chat.Id, $"Игра создана, {i}");
                                    XOROgames[i] = new XORO(message, i); //creating new game
                                    XOROchats[i] = message.Chat.Id; //creating space for new chat
                                    await XOROgames[i].StartGame();
                                    break;
                                }
                            }

                        }
                        break;
                    case "/suefa":
                        if (!SUEFAchats.Contains(message.Chat.Id))
                        {
                            await Worker.SendTextMessageAsync(message.Chat.Id, "Игра существует, присоединение");
                            int findId = Array.IndexOf(SUEFAchats, message.Chat.Id); //finding game that already started
                            /*await SUEFAgames[findId].ContinueGame();*/ //continue game
                        }
                        else
                        {
                            for (int i = 0; i < SUEFAchats.Length; i++) //finding free space in array games
                            {
                                if (SUEFAgames[i] == null)
                                {
                                    await Worker.SendTextMessageAsync(message.Chat.Id, $"Игра создана, {i}");
                                    SUEFAgames[i] = new SUEFA(message, i); //creating new game
                                    SUEFAchats[i] = message.Chat.Id; //creating space for new chat
                                    SUEFAgames[i].StartGame();
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
