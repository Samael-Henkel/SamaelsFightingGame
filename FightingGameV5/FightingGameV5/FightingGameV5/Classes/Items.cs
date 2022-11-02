using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FightingGameV5.Items
{ 
    public class Item
    {
        static string[] PossibleNames =
        {
            "Cheese", "Salmon Sandwich", "Bottle of Rivella", "Spicy Salmon Sandwich", "Toblerone", "Filet-O-Fish" // All the Names an Item can have
        };
        static Random random = new();
        public static string staticName; // static name for static uses
        public static int Heals; // how much an item heals
        public void generateNewItem() // generates item name
        {
            staticName = PossibleNames[random.Next(0, PossibleNames.Length)];
        }
        public string getName() // gets name for use in the grid
        {
            return staticName;
        }
        public int getHeal(string ItemName) // gets how much an item heals
        {
            switch (ItemName)
            {
                case "Cheese":
                    Heals = 25;
                    break;
                case "Salmon Sandwich":
                    Heals = 20;
                    break;
                case "Bottle of Rivella":
                    Heals = 15;
                    break;
                case "Spicy Salmon Sandwich":
                    Heals = 30;
                    break;
                case "Toblerone":
                    Heals = 35;
                    break;
                case "Filet-O-Fish":
                    Heals = 10;
                    break;
                default: //failsafe and TP heal
                    Heals = random.Next(5, 10);
                    break;
            }
            return Heals;
        }
    }
}