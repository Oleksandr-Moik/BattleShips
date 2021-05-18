using System.Collections.Generic;

namespace BattleShips
{
    public class Frigate : Ship
    {
        public Frigate(List<Coordinates> coordinates) : base(2, coordinates)
        {
        }
    }
}