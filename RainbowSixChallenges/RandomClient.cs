using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowSixChallenges
{
    public class RandomClient
    {
        System.IO.StreamReader file = new System.IO.StreamReader("D:/nils/source/MasterYoda/RainbowSixChallenges/Attacker.txt");
        System.IO.StreamReader file1 = new System.IO.StreamReader("D:/nils/source/MasterYoda/RainbowSixChallenges/Defender.txt");

        string[] scopes = new string[] { "iron-sight", "red dot", "holographic", "reflex", "scope 1.5x", "scope 2.0x", "scope 2.5x" };
        List<string> attackerLayouts = new List<string>();
        List<string> defenderLayouts = new List<string>();

        public RandomClient()
        {
            string line;
            while((line = file.ReadLine()) != null)
            {
                attackerLayouts.Add(line);
            }

            while ((line = file1.ReadLine()) != null)
            {
                defenderLayouts.Add(line);
            }
        }

        public string GetRandomAttacker()
        {
            Random rand = new Random();
            return attackerLayouts[rand.Next(0, attackerLayouts.Count - 1)] + " -> " + scopes[rand.Next(0, scopes.Length - 1)]; 
        }
        public string GetRandomDefender()
        {
            Random rand = new Random();
            return defenderLayouts[rand.Next(0, defenderLayouts.Count - 1)] + " -> " + scopes[rand.Next(0, scopes.Length - 1)];
        }

    }
}
