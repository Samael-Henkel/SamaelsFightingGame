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

namespace FightingGameV5.ButtonFunctionality
{
    // Made on 04.10.2022
    public class ButtonFunctionalityClass
    {
        Random random = new();
        public int Fight(int PlayerAtk, int EnemyDef, int EnemyHP)
        {
            int dmg = random.Next(1, PlayerAtk + 5 - EnemyDef); // generates randomized amounts of Damage
            EnemyHP -= dmg;                                     // deals Damage
            return EnemyHP;                                     // returns HP after Damage was dealt
        }
        public int defendTP(int TP, int def)
        {
            TP += def; //Gain TP by amount of def you have...
            if(TP > 100)
            {
                TP = 100; // ...but it can never be more than 100 TP.
            }
            return TP;
        }
        public int specatk(int TP, int PlayerAtk, int EnemyDef, int EnemyHP)
        {
            TP -= 50;
            int dmg = random.Next(10, PlayerAtk + 50 - EnemyDef);
            EnemyHP -= dmg;
            return EnemyHP; // Same as Fight, just higher chance of making a lot of damage
        }
    }
}