using System;

namespace SnakeAndLadder
{
    public  class Dice
    {
        public int RollDie()
        {
            Random rnd = new Random();
            return rnd.Next(6) + 1;
        }
    }
}