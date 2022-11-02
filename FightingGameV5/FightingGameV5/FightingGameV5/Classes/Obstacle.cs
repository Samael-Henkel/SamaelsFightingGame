using System;

namespace ObstacleCsharp
{
    class Obstacle
    {
        static Random random = new();
        public int height = random.Next(50, 110); // Height of it, randomized
        public int width = 10; // Width of it, permanent
        static string[] PossiblePositions = new string[]
        {
                "Top", "Bottom",
        };
        public string position = PossiblePositions[random.Next(0, PossiblePositions.Length)]; //should be self explanatory
        public bool right = random.Next(0, 100) >= 50; //direction on movement
        public double speed; //speed of the obstacle
    }
}
