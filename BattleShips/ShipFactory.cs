using System.Collections.Generic;

namespace BattleShips
{
    public class ShipFactory
    {
        public Ship CreateShip(List<Coordinates> coordinates)
        {
            if (coordinates.Count > 4)
            {
                return null;
                // Not use exceptions
                // throw new ArgumentException()
            }

            switch (coordinates.Count)
            {
                case 4: return new AircraftCarrier(coordinates);
                case 3: return new Battlecruiser(coordinates);
                case 2: return new Frigate(coordinates);
                case 1: return new Corvette(coordinates);
                default: return null;
            }
        }
    }
}