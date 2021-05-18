using System.Collections.Generic;

namespace BattleShips
{
    public class Battlecruiser : Ship
    {
        public Battlecruiser(List<Coordinates> coordinates) : base(3, coordinates)
        {
        }
    }
}