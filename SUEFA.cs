using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Args;

namespace CSharpTelegramBot
{
    class SUEFA : Game
    {
        int gameId;
        public SUEFA (Telegram.Bot.Types.Message msg = null, int id = 0)
        {
            gameId = id;
        }
        public void StartGame()
        {
            Program.Worker.OnMessage += SUEFA_OnMessage;
        }
        public void ContinueGame()
        {

            Program.Worker.OnMessage += SUEFA_OnMessage;
        }

        private void SUEFA_OnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            Program.Worker.SendTextMessageAsync(message.Chat.Id, "SUEFA game");
        }
    }
}
