using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace CSharpTelegramBot
{
    class SUEFA : Game
    {
        int player = 0;
        int numOfTurns = 2;
        int numOfPlayers = 1;
        int gameId;
        Telegram.Bot.Types.Message msg;
        Telegram.Bot.Types.Message[] playersId = new Telegram.Bot.Types.Message[2];
        public SUEFA (Telegram.Bot.Types.Message message, int id)
        {
            msg = message;
            gameId = id;
            playersId[0] = msg;
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
                playersId[numOfPlayers] = message;
                numOfPlayers++;
                await Program.Worker.SendTextMessageAsync(message.Chat.Id, $"Игрок принят");
                return;
            }
            else if (numOfPlayers == 2 && message.Text != "/vote")
            {
                if (message.From.Username != playersId[player].From.Username)
                    return;
                if (message.Text == "/stop")
                {
                    await Program.Worker.SendTextMessageAsync(message.Chat.Id, "Прекращаю..", replyMarkup: Program.GetButtons());
                    EndGame();
                    return;
                }
                else
                {
                    await Program.Worker.SendTextMessageAsync(message.Chat.Id, $"Игрок {playersId[player].From.Username} походил");
                    // Game starts here

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
    }
}
