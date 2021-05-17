using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowSixChallenges
{
    public class OverwatchRandomizer
    {
        string[] operators = new string[]
        {
            "Ana", "Ashe", "Baptiste", "Bastion", "Brigitte", "D.Va", "Doomfist",
            "Echo", "Genji", "Hanzo", "Junkrat", "Lúcio", "McCree", "Mei",
            "Mercy", "Moira", "Orisa", "Pharah", "Reaper", "Reinhardt", "Roadhog",
            "Sigma", "Soldier: 76", "Sombra", "Symmetra", "Torbjörn", "Tracer",
            "Widowmaker", "Winston", "Wrecking ball", "Zarya", "Zenyatta"
        };

        public string GetRandomOperator()
        {
            Random r = new Random();
            return operators[r.Next(0, operators.Length - 1)];
        }
    }


}
