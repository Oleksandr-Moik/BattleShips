using System.Collections.Generic;

namespace BattleShips
{
    public class AircraftCarrier : Ship
    {
        public AircraftCarrier(List<Coordinates> coordinates) : base(4, coordinates)
        {
        }
    }
}