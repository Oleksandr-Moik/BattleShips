using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BattleShips
{
    public class Player
    {
        public Player(DataGridView playerBoard, string name, List<Ship> ships)
        {
            this.PlayerBoard = playerBoard;
            this.Name = name;
            this.Ships = ships;
        }
        
        public string Name { get; }

        public DataGridView PlayerBoard { get; }
        public List<Ship> Ships { get; }

        public bool IsFollowsRules()
        {
            return Ships.FindAll(ship => ship.GetType() == typeof(AircraftCarrier)).Count == 1
                   && Ships.FindAll(ship => ship.GetType() == typeof(Battlecruiser)).Count == 2
                   && Ships.FindAll(ship => ship.GetType() == typeof(Frigate)).Count == 3
                   && Ships.FindAll(ship => ship.GetType() == typeof(Corvette)).Count == 4;
        }

        public bool ReactOnHit(Coordinates coordinates)
        {
            if (!String.IsNullOrEmpty((string)PlayerBoard.Rows[coordinates.x].Cells[coordinates.y].Value)) return true;
            
            var hitShip = Ships.Find(ship => ship.isHit(coordinates));
            bool hasHit = hitShip != null;
            if (hasHit)
            {
                HighlightHit(coordinates);
                if (!hitShip.isAlive())
                {
                    HighlightCellsAroundShip(hitShip);
                }
            }
            else
            {
                HighlightMiss(coordinates);
            }
            
            return hasHit;
        }

        private void HighlightCellsAroundShip(Ship brokenShip)
        {
            foreach (var shipCoordinate in brokenShip.coordinates)
            {
                if (shipCoordinate.y - 1 >= 0)
                {
                    HighlightMiss(new Coordinates(shipCoordinate.x, shipCoordinate.y - 1));
                }
                if (shipCoordinate.y + 1 < PlayerBoard.RowCount)
                {
                    HighlightMiss(new Coordinates(shipCoordinate.x, shipCoordinate.y + 1));
                }
                if (shipCoordinate.x - 1 >= 0)
                {
                    HighlightMiss(new Coordinates(shipCoordinate.x - 1, shipCoordinate.y));
                    if (shipCoordinate.y - 1 >= 0)
                    {
                        HighlightMiss(new Coordinates(shipCoordinate.x - 1, shipCoordinate.y - 1));
                    }
                    if (shipCoordinate.y + 1 < PlayerBoard.RowCount)
                    {
                        HighlightMiss(new Coordinates(shipCoordinate.x - 1, shipCoordinate.y + 1));
                    }
                }

                if (shipCoordinate.x + 1 < PlayerBoard.ColumnCount)
                {
                    HighlightMiss(new Coordinates(shipCoordinate.x + 1, shipCoordinate.y));
                    if (shipCoordinate.y - 1 >= 0)
                    {
                        HighlightMiss(new Coordinates(shipCoordinate.x + 1, shipCoordinate.y - 1));
                    }
                    if (shipCoordinate.y + 1 < PlayerBoard.RowCount)
                    {
                        HighlightMiss(new Coordinates(shipCoordinate.x + 1, shipCoordinate.y + 1));
                    }
                }
            }
            
        }

        private void HighlightMiss(Coordinates coordinates)
        {
            var cell = PlayerBoard.Rows[coordinates.x].Cells[coordinates.y];
            if (!String.IsNullOrEmpty((string)cell.Value)) return;
            cell.Value = "*";
            
        }

        private void HighlightHit(Coordinates coordinates)
        {
            var cell = PlayerBoard.Rows[coordinates.x].Cells[coordinates.y];
            if (!String.IsNullOrEmpty((string)cell.Value)) return;
            cell.Value = "X";
            cell.Style.BackColor = Color.DarkRed;
        }
        
        public bool IsLost()
        {
            return Ships.TrueForAll(s => !s.isAlive());
        }

        public Coordinates randomizeShoot()
        {
            var rnd = new Random();
            return new Coordinates(rnd.Next(PlayerBoard.RowCount), rnd.Next(PlayerBoard.ColumnCount));
        }
    }
}