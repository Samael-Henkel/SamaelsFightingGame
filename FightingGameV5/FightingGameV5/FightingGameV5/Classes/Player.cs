namespace PlayerCsharp
{
    class Player
    {
        public int hp = 50; // Hit Points. You lose when this reaches 0
        public int maxhp = 50; // Maximum HP
        public int atk = 20; // Attack. Used to calculate Damage dealt
        public int def = 20; // Defence. TBH basically useless exept for gaining TP due to the nonexistence of invincibility frames
        public string Name = "CHARA"; //Failsafe player name
        public bool isDef = false; // Is Player Defending and blocking Attacks?
        public int BlockableAtks = 0; //How many Attacks can be blocked.
        public int TP = 0; // Tension Points, for certain acts.
    }
}