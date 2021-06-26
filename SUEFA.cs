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
        int player = 0;
        int numOfTurns = 0;
        int numOfPlayers = 1;
        int gameId;
        Telegram.Bot.Types.Message msg;
        long[] playersId = new long[2];
        enum objects
        {
            Ножницы,
            Камень,
            Бумага
        }
        objects[] answers = new objects[2];
        public SUEFA (Telegram.Bot.Types.Message message, int id)
        {
            msg = message;
            gameId = id;
            playersId[0] = msg.From.Id;
        }
        public async void StartGame()
        {
           Program.Worker.OnMessage += SUEFA_OnMessage;
           await Program.Worker.SendTextMessageAsync(msg.Chat.Id, "/vote чтобы стать вторым игроком", replyMarkup: GetInvited()) ;
        }

        private async void SUEFA_OnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            if (gameId != Array.IndexOf(Program.SUEFAchats, message.Chat.Id))
            {
                return;
            }
            if (numOfPlayers < 2 && message.Text == "/vote")
            {
                playersId[numOfPlayers] = message.From.Id;
                numOfPlayers++;
                await Program.Worker.SendTextMessageAsync(message.Chat.Id, $"Игрок принят");
                CountingDown();
                return;
            }
            else if (numOfPlayers == 2 && message.Text != "/vote")
            {
                if (!playersId.Contains(message.From.Id))
                    return;
                if (message.Text == "/stop")
                {
                    await Program.Worker.SendTextMessageAsync(message.Chat.Id, "Прекращаю..", replyMarkup: Program.GetButtons());
                    EndGame();
                    return;
                }
                else
                {
                    Program.Worker.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                    if (numOfTurns < 2)
                        switch (message.Text)
                        {
                            case "/Ножницы":
                                answers[numOfTurns] = objects.Ножницы;
                                numOfTurns++;
                                break;
                            case "/Бумага":
                                answers[numOfTurns] = objects.Бумага;
                                numOfTurns++;
                                break;
                            case "/Камень":
                                answers[numOfTurns] = objects.Камень;
                                numOfTurns++;
                                break;
                        }
                        


                }
            }
            else if (numOfPlayers == 2 && message.Text == "/vote")
            {
                await Program.Worker.SendTextMessageAsync(message.Chat.Id, "Слишком много игроков");
                return;
            }
            else
                return;

        }
        private async void CountingDown() 
        {
            Program.Worker.SendTextMessageAsync(msg.Chat.Id, "Су", replyMarkup: GetButtons());
            await Task.Delay(2000);
            Program.Worker.SendTextMessageAsync(msg.Chat.Id, "Е");
            await Task.Delay(2000);
            Program.Worker.SendTextMessageAsync(msg.Chat.Id, "Фа", replyMarkup: Program.GetButtons());
            CheckForWin();
            EndGame();

        }
        private void EndGame()
        {
            Program.Worker.OnMessage -= SUEFA_OnMessage;
            Program.SUEFAchats[gameId] = 0;
            Program.SUEFAgames[gameId] = null;
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
            int enumerator = 0;
            foreach (int a in answers)
            {
                enumerator += a;
            }
            if(enumerator == 1)
                Program.Worker.SendTextMessageAsync(msg.Chat.Id, "Камень бьет ножницы", replyMarkup: Program.GetButtons());
            else if (enumerator == 2)
                Program.Worker.SendTextMessageAsync(msg.Chat.Id, "Ножницы режут бумагу", replyMarkup: Program.GetButtons());
            else if (enumerator == 3)
                Program.Worker.SendTextMessageAsync(msg.Chat.Id, "Бумага кроет камень", replyMarkup: Program.GetButtons());
            else
                Program.Worker.SendTextMessageAsync(msg.Chat.Id, "Ничья", replyMarkup: Program.GetButtons());
            return;

        }
    }
}
