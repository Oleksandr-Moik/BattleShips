using System.Collections.Generic;

namespace BattleShips
{
    public abstract class Ship
    {

        public Ship(int palubaCount, List<Coordinates> coordinates)
        {
            this.palubaCount = palubaCount;
            this.coordinates = coordinates;
        }

        public int palubaCount { get; }

        private int hitsCount;
        public List<Coordinates> coordinates { get; }

        public bool isHit(Coordinates hitCoordinate)
        {
            if (this.coordinates.Contains(hitCoordinate))
            {
                hitsCount++;
                return true;
            }

            return false;
        }

        public bool isAlive()
        {
            return this.palubaCount > this.hitsCount;
        }
    }
}