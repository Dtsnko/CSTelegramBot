using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace CSharpTelegramBot
{
    class SUEFA : Game
    {
        public const int generalGameId = (int)Program.GameType.SUEFA;
        int numOfTurns = 0;
        int numOfPlayers = 0;
        Telegram.Bot.Types.Message msg;
        long[] playersId = new long[2];
        Telegram.Bot.Types.Message[] playersMsg = new Telegram.Bot.Types.Message[2];
        enum objects
        {
            Ножницы,
            Камень,
            Бумага
        }
        objects[] answers = new objects[2];

        public SUEFA (Telegram.Bot.Types.Message message, int gameId)
        {
            msg = message;
            this.gameId = gameId;
            playersId[0] = msg.From.Id;
            numOfPlayers++;
        }
        public override async void StartGame()
        {
           Program.Worker.OnMessage += SUEFA_OnMessage;
           await Program.Worker.SendTextMessageAsync(msg.Chat.Id, "/vote чтобы стать вторым игроком", replyMarkup: GetInvited()) ;
        }

        private async void SUEFA_OnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            if (gameId != Array.IndexOf(Program.chats, message.Chat.Id))
                return;
            if (numOfPlayers < 2 && message.Text == "/vote")
            {
                playersId[numOfPlayers] = message.From.Id;
                numOfPlayers++;
                await Program.Worker.SendTextMessageAsync(message.Chat.Id, $"Игрок принят");
                CountDown();
                return;
            }
            else if (numOfPlayers == 2)
            {
                if (!playersId.Contains(message.From.Id))
                    return;
                await Program.Worker.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                if (numOfTurns < 2)
                    switch (message.Text)
                        {
                            case "/Ножницы":
                                answers[numOfTurns] = objects.Ножницы;
                                playersMsg[numOfTurns] = message;
                                numOfTurns++;
                                break;
                            case "/Бумага":
                                answers[numOfTurns] = objects.Бумага;
                                playersMsg[numOfTurns] = message;
                                numOfTurns++;
                                break;
                            case "/Камень":
                                answers[numOfTurns] = objects.Камень;
                                playersMsg[numOfTurns] = message;
                                numOfTurns++;
                                break;
                    }
            }
            else
                return;

        }
        private async void CountDown() 
        {
            try
            {
                await Program.Worker.SendTextMessageAsync(msg.Chat.Id, "Су", replyMarkup: GetButtons());
                await Task.Delay(2000);
                await Program.Worker.SendTextMessageAsync(msg.Chat.Id, "Е");
                await Task.Delay(2000);
                await Program.Worker.SendTextMessageAsync(msg.Chat.Id, "Фа", replyMarkup: Program.GetButtons());
                CheckForWin();
            }
            finally
            {
                EndGame();
            }

        }
        public override void EndGame()
        {
            Program.Worker.OnMessage -= SUEFA_OnMessage;
            Program.chats[gameId] = 0;
            Program.games[gameId] = null;
        }
        private IReplyMarkup GetInvited()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "/vote" } },

                }
            };
        }
        private IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "/Камень" }, new KeyboardButton { Text = "/Ножницы" }, new KeyboardButton { Text = "/Бумага" } },

                }
            };
        }
        private void CheckForWin()
        {
            try
            {
                int enumerator = 0;
                foreach (int a in answers)
                    enumerator += a;
                if (enumerator == 1)
                    Program.Worker.SendTextMessageAsync(msg.Chat.Id, $"Камень бьет ножницы, {playersMsg[Array.IndexOf(answers, objects.Камень)].From.FirstName} выиграл", replyMarkup: Program.GetButtons());
                else if (enumerator == 2)
                    Program.Worker.SendTextMessageAsync(msg.Chat.Id, $"Ножницы режут бумагу, {playersMsg[Array.IndexOf(answers, objects.Ножницы)].From.FirstName} выиграл", replyMarkup: Program.GetButtons());
                else if (enumerator == 3)
                    Program.Worker.SendTextMessageAsync(msg.Chat.Id, $"Бумага кроет камень, {playersMsg[Array.IndexOf(answers, objects.Бумага)].From.FirstName} выиграл", replyMarkup: Program.GetButtons());
                else
                    Program.Worker.SendTextMessageAsync(msg.Chat.Id, "Ничья", replyMarkup: Program.GetButtons());
                numOfTurns = 0;
                return;
            }
            catch
            {
                EndGame();
            }

        }
    }
}
