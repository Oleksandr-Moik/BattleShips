using System;

namespace BattleShips
{
    public class Coordinates : IEquatable<Coordinates>
    {

        public Coordinates(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int x { get; }

        public int y { get; }

        public bool Equals(Coordinates other)
        {
            return x == other.x && y == other.y;
        }
    }
}