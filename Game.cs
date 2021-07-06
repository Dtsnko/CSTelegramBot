using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpTelegramBot
{
    public abstract class Game
    {
        public virtual int gameId { get; set; }
        public virtual void StartGame() { }
        public virtual void EndGame() { }


    }
}
