using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpTelegramBot
{
    public abstract class Game
    {
        int gameid { get; set; }
        Telegram.Bot.Types.Message msg { get; set; }


    }
}
