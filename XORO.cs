using System;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using System.Text.Json;
using System.IO;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpTelegramBot
{
    class XORO : Game
    {
        string[] boxes = { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        int player = 0;
        int numOfTurns = 2;
        int numOfPlayers = 1;
        Telegram.Bot.Types.Message[] playersId = new Telegram.Bot.Types.Message[2];

        //public XORO() : this(null) { }
        //public XORO(Telegram.Bot.Types.Message message)
        //{
        //    msg = message;
        //    playersId[0] = msg;
        //    PlayGame();
        //}
        

        public async Task PlayGame(Telegram.Bot.Types.Message msg)
        {
            
                playersId[0] = msg;
                await Program.Worker.SendTextMessageAsync(msg.Chat.Id, "+--+--+ \n" +
                $"|{boxes[0]}|{boxes[1]}|{boxes[2]}| \n" +
                "+--+--+ \n" +
                $"|{boxes[3]}|{boxes[4]}|{boxes[5]}| \n" +
                "+--+--+ \n" +
                $"|{boxes[6]}|{boxes[7]}|{boxes[8]}| \n" +
                "+--+--+ \n");
                await Program.Worker.SendTextMessageAsync(msg.Chat.Id, $"Напиши /vote, чтобы быть игроком", replyMarkup: GetInvited());
                return;
        }
        public async Task ContinueGame(Telegram.Bot.Types.Message msg)
        {
            //await Task.Run(() => Program.Worker.OnMessage -= Program.Worker_OnMessage);
            //await Task.Run(() => Program.Worker.OnMessage += GameWorker_OnMessage);
            var message = msg;
            if (numOfPlayers < 2 && message.Text == "/vote")
            {
                playersId[numOfPlayers] = message;
                numOfPlayers++;
                await Program.Worker.SendTextMessageAsync(message.Chat.Id, "Игрок принят", replyMarkup: GetInvited());
                if (numOfPlayers == 2)
                    await Program.Worker.SendTextMessageAsync(message.Chat.Id, $"Игроки набраны, игрок {playersId[player].From.Username} ходит", replyMarkup: GetButtons());
                return;
            }
            else if (numOfPlayers == 2 && message.Text != "/vote")
            {
                if (message.From.Username != playersId[player].From.Username)
                    return;
                if (message.Text == "/stop")
                {
                    await Program.Worker.SendTextMessageAsync(message.Chat.Id, "Прекращаю..", replyMarkup: Program.GetButtons());
                    //Program.Worker.OnMessage -= GameWorker_OnMessage;
                    //Program.Worker.OnMessage += Program.Worker_OnMessage;
                    return;
                }
                else
                {
                    await Program.Worker.SendTextMessageAsync(message.Chat.Id, $"Игрок {playersId[player].From.Username} походил", replyMarkup: GetButtons());
                    CheckBoxes(message);
                }
            }
            else if (numOfPlayers == 2 && message.Text == "/vote")
            {
                await Program.Worker.SendTextMessageAsync(message.Chat.Id, "Слишком много игроков", replyMarkup: GetButtons());
                return;
            }
            else
                return;
        }

        private IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "/1" }, new KeyboardButton {Text = "/2" }, new KeyboardButton { Text = "/3" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "/4" }, new KeyboardButton {Text = "/5" }, new KeyboardButton { Text = "/6" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "/7" }, new KeyboardButton {Text = "/8" }, new KeyboardButton { Text = "/9" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "/stop"} }
                }
            };
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

        //async void GameWorker_OnMessage(object sender, MessageEventArgs e)
        //{
        //    var message = e.Message;
        //    if (numOfPlayers < 2 && message.Text == "/vote")
        //    {
        //        playersId[numOfPlayers] = message;
        //        numOfPlayers++;
        //        await Program.Worker.SendTextMessageAsync(message.Chat.Id, "Игрок принят", replyMarkup: GetInvited());
        //        if (numOfPlayers == 2)
        //           await Program.Worker.SendTextMessageAsync(message.Chat.Id, $"Игроки набраны, игрок {playersId[player].From.Username} ходит", replyMarkup: GetButtons());
        //        return;
        //    }
        //    else if (numOfPlayers == 2 && message.Text != "/vote")
        //    {
        //        if (message.From.Username != playersId[player].From.Username)
        //            return;
        //        if (message.Text == "/stop")
        //        {
        //            await Program.Worker.SendTextMessageAsync(message.Chat.Id, "Прекращаю..", replyMarkup: Program.GetButtons());
        //            Program.Worker.OnMessage -= GameWorker_OnMessage;
        //            Program.Worker.OnMessage += Program.Worker_OnMessage;
        //            return;
        //        }
        //        else
        //        {
        //            await Program.Worker.SendTextMessageAsync(message.Chat.Id, $"Игрок {playersId[player].From.Username} походил", replyMarkup: GetButtons());
        //            CheckBoxes(message);
        //        }
        //    }
        //    else if (numOfPlayers == 2 && message.Text == "/vote")
        //    {
        //        await Program.Worker.SendTextMessageAsync(message.Chat.Id, "Слишком много игроков", replyMarkup: GetButtons());
        //        return;
        //    }
        //    else
        //        return;
        //}
        void CheckBoxes(Telegram.Bot.Types.Message message)
        {
            string text = message.Text.Substring(1);
            for (int i = 0; i < boxes.Length; i++)
            {
                if (boxes[i].Equals(text) && player == 0)
                {
                    boxes[i] = "X";
                    break;
                }
                else if (boxes[i].Equals(text) && player == 1) 
                {                    
                    boxes[i] = "O";
                    break;
                }
                else if (i+1 == boxes.Length && boxes[i] != text)
                {
                    Program.Worker.SendTextMessageAsync(message.Chat.Id, $"Wrong, choose correctly", replyMarkup: GetButtons());
                    return;
                }
            }
            CheckForWin(message);
        }
        void CheckForWin(Telegram.Bot.Types.Message message)
        {
            int checker = 0;
            for (int i = 0; i < boxes.Length; i++)
            {
                if(boxes[i] != Convert.ToString(i+1))
                {
                    checker++;
                }
            }
            if (checker == 9)
            {
                Program.Worker.SendTextMessageAsync(message.Chat.Id, $"Ничья", replyMarkup: Program.GetButtons());
                //Program.Worker.OnMessage -= GameWorker_OnMessage;
                //Program.Worker.OnMessage += Program.Worker_OnMessage;
                return;
            }
            if (boxes[0] == boxes[4] && boxes[4] == boxes[8] ||
                boxes[0] == boxes[1] && boxes[1] == boxes[2] ||
                boxes[0] == boxes[3] && boxes[3] == boxes[6] ||
                boxes[1] == boxes[4] && boxes[4] == boxes[7] ||
                boxes[2] == boxes[4] && boxes[4] == boxes[6] ||
                boxes[2] == boxes[5] && boxes[5] == boxes[8] ||
                boxes[3] == boxes[4] && boxes[4] == boxes[5] ||
                boxes[6] == boxes[7] && boxes[7] == boxes[8]

            )
            {
                Program.Worker.SendTextMessageAsync(message.Chat.Id, $"Игрок {playersId[player].From.Username} выиграл", replyMarkup: Program.GetButtons());
                //Program.Worker.OnMessage -= GameWorker_OnMessage;
                //Program.Worker.OnMessage += Program.Worker_OnMessage;
                return;
            }
            else
            {
                ShowBoxes(message);
                ChangePlayer();
                Program.Worker.SendTextMessageAsync(message.Chat.Id, $"Игрок {playersId[player].From.Username} ходит");
            }
        }
        void ChangePlayer()
        {
            numOfTurns++;
            player = numOfTurns % 2;
        }
        void ShowBoxes(Telegram.Bot.Types.Message message)
        {
            Program.Worker.SendTextMessageAsync(message.Chat.Id, "+--+--+ \n" +
            $"|{boxes[0]}|{boxes[1]}|{boxes[2]}| \n" +
            "+--+--+ \n" +
            $"|{boxes[3]}|{boxes[4]}|{boxes[5]}| \n" +
            "+--+--+ \n" +
            $"|{boxes[6]}|{boxes[7]}|{boxes[8]}| \n" +
            "+--+--+ \n", replyMarkup: GetButtons());
        }

    }

}


