using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Schema;

namespace BattleShips
{
    public partial class Form1 : Form
    {
        private int maxX = 10;
        private int maxY = 10;
        
        private Player player;
        private Player ai;

        private List<List<Coordinates>> selectedCells = new List<List<Coordinates>>();

        public Form1()
        {
            InitializeComponent();
            initTable(userBoard);
            userBoard.DefaultCellStyle.BackColor = Color.White;
            userBoard.DefaultCellStyle.SelectionBackColor = Color.Chartreuse;
            initTable(shootingBoard);
            Debug.WriteLine("load");
        }

        private void userBoard_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var coordinates = new Coordinates(e.RowIndex, e.ColumnIndex);
            var add = true;
            if (userBoard.CurrentCell.Tag == null)
            {
                if (hasConflicts(coordinates, selectedCells))
                {
                    userBoard.ClearSelection();
                    return;
                }

                List<Coordinates> chain = getChain(coordinates);
                if (chain.Count == 1)
                {
                    selectedCells.Add(chain);
                }

                userBoard.CurrentCell.Style.BackColor = Color.Chartreuse;
                userBoard.CurrentCell.Tag = true;
            }
            else
            {
                userBoard.CurrentCell.Style.BackColor = Color.White;
                foreach (var potentialShip in selectedCells)
                {
                    potentialShip.Remove(coordinates);
                }
                selectedCells.RemoveAll(list => list.Count == 0);
                userBoard.CurrentCell.Tag = null;
            }
            userBoard.ClearSelection();
        }

        private bool hasConflicts(Coordinates coordinates, List<List<Coordinates>> selectedCells)
        {
            List<Coordinates> potentialConflicts = new List<Coordinates>();
            potentialConflicts.Add(new Coordinates(coordinates.x - 1, coordinates.y - 1));
            potentialConflicts.Add(new Coordinates(coordinates.x - 1, coordinates.y + 1));
            potentialConflicts.Add(new Coordinates(coordinates.x + 1, coordinates.y - 1));
            potentialConflicts.Add(new Coordinates(coordinates.x + 1, coordinates.y + 1));

            foreach (var potentialConflict in potentialConflicts)
            {
                if (
                    potentialConflict.x >= 0
                    && potentialConflict.x < maxX
                    && potentialConflict.y >= 0
                    && potentialConflict.y < maxY
                    && selectedCells.Any(chain => chain.Contains(potentialConflict)))
                {
                    return true;
                }
            }

            return false;
        }

        private List<Coordinates> getChain(Coordinates coordinates)
        {
            List<Coordinates> chain = new List<Coordinates>();
            foreach (var selectedCellChain in selectedCells)
            {
                if (selectedCellChain.Contains(new Coordinates(coordinates.x - 1, coordinates.y))
                    || selectedCellChain.Contains(new Coordinates(coordinates.x + 1, coordinates.y))
                    || selectedCellChain.Contains(new Coordinates(coordinates.x, coordinates.y - 1))
                    || selectedCellChain.Contains(new Coordinates(coordinates.x, coordinates.y + 1))
                )
                {
                    chain = selectedCellChain;
                    break;
                }
            }
            
            chain.Add(coordinates);
            return chain;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<Ship> ships = InitUserBoard();
            String name = textBox1.Text;
            player = new Player(userBoard, name, ships);
            if (!player.IsFollowsRules()&&false)
            {
                MessageBox.Show("Please reorganize ships");
            }
            else
            {
                userBoard.Enabled = true;
                shootingBoard.Enabled = true;
                button1.Enabled = false;
                List<Ship> aiShips = PlaceShipsRandomly();
                ai = new Player(shootingBoard,"AI", aiShips);
                MessageBox.Show("Let's start the BattleShip. Your turn first");
            }
        }

        private List<Ship> PlaceShipsRandomly()
        {
            List<Ship> ships = new List<Ship>();
            List<List<Coordinates>> generatedCoordinates = new List<List<Coordinates>>();
            ships.Add(PlaceShip(4, generatedCoordinates));
            ships.Add(PlaceShip(3, generatedCoordinates));
            ships.Add(PlaceShip(3, generatedCoordinates));
            ships.Add(PlaceShip(2, generatedCoordinates));
            ships.Add(PlaceShip(2, generatedCoordinates));
            ships.Add(PlaceShip(2, generatedCoordinates));
            ships.Add(PlaceShip(1, generatedCoordinates));
            ships.Add(PlaceShip(1, generatedCoordinates));
            ships.Add(PlaceShip(1, generatedCoordinates));
            ships.Add(PlaceShip(1, generatedCoordinates));
            return ships;
        }

        private Ship PlaceShip(int paluba, List<List<Coordinates>> occupiedCoordinates)
        {
            Random rnd = new Random();
            Ship ship = null;
            do
            {
                var randomX = rnd.Next(maxX);
                var randomY = rnd.Next(maxY);
                bool vertical = rnd.Next(100) % 2 == 0;
                if (vertical)
                {
                    var occupiedY = occupiedCoordinates
                        .SelectMany(list => list)
                        .ToList()
                        .FindAll(coordinates => coordinates.x == randomX)
                        .Select(coordinates => coordinates.y)
                        .ToList();
                    var possibleY = new List<int>(Enumerable.Range(0, maxY - paluba));
                    possibleY.RemoveAll(y =>
                    {
                        var possibleLocations = new List<int>(Enumerable.Range(y - 1, paluba + 2));
                        return possibleLocations.Any(pl => occupiedY.Contains(pl) || hasConflicts(new Coordinates(randomX, pl), occupiedCoordinates));
                    });
                    if (possibleY.Count > 0)
                    {
                        var startPosition = possibleY[rnd.Next(possibleY.Count)];
                        var coordinates = new List<Coordinates>();
                        for (int i = startPosition; i < startPosition + paluba; i++)
                        {
                            coordinates.Add(new Coordinates(randomX, i));
                        }

                        occupiedCoordinates.Add(coordinates);
                        ship = new ShipFactory().CreateShip(coordinates);
                    }
                    
                }
                else
                {
                    var occupiedX = occupiedCoordinates
                        .SelectMany(list => list)
                        .ToList()
                        .FindAll(coordinates => coordinates.y == randomY)
                        .Select(coordinates => coordinates.x)
                        .ToList();
                    var possibleX = new List<int>(Enumerable.Range(0, maxX - paluba));
                    possibleX.RemoveAll(x =>
                    {
                        var possibleLocations = new List<int>(Enumerable.Range(x - 1, paluba + 2));
                        return possibleLocations.Any(pl => occupiedX.Contains(pl) || hasConflicts(new Coordinates(pl, randomY), occupiedCoordinates));
                    });
                    if (possibleX.Count > 0)
                    {
                        var startPosition = possibleX[rnd.Next(possibleX.Count)];
                        var coordinates = new List<Coordinates>();
                        for (int i = startPosition; i < startPosition + paluba; i++)
                        {
                            coordinates.Add(new Coordinates(i, randomY));
                        }
                        occupiedCoordinates.Add(coordinates);
                        ship = new ShipFactory().CreateShip(coordinates);
                    }
                }
            } while (ship == null);
            return ship;
        }

        private List<Ship> InitUserBoard()
        {
            var ships = new List<Ship>();
            var shipFactory = new ShipFactory();
            foreach (var selectedChain in selectedCells)
            {
              ships.Add(shipFactory.CreateShip(selectedChain));  
            }

            return ships;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            selectedCells.Clear();
            player = null;
            ai = null;
            button1.Enabled = true;
            userBoard.Enabled = true;
            shootingBoard.Enabled = false;
            ClearTable(userBoard);
            ClearTable(shootingBoard);
        }

        private void ClearTable(DataGridView table)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    var dataGridViewCell = table.Rows[i].Cells[j];
                    dataGridViewCell.Value = "";
                    dataGridViewCell.Tag = null;
                    dataGridViewCell.Style.BackColor = Color.White;
                }
            }
        }

        private void shootingBoard_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var hit = ai.ReactOnHit(new Coordinates(e.RowIndex, e.ColumnIndex));
            if (hit)
            {
                if (ai.IsLost())
                {
                    MessageBox.Show(player.Name + " has won! Congrats!");
                    ai.PlayerBoard.Enabled = false;
                    button1.Enabled = false;
                }
            }
            else
            {
                var aiHit = false;
                do
                {
                    var coordinates = ai.randomizeShoot();
                    aiHit = player.ReactOnHit(coordinates);
                    if (aiHit)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }

                    if (player.IsLost())
                    {
                        MessageBox.Show("AI has won! You can do better!");
                        ai.PlayerBoard.Enabled = false;
                        button1.Enabled = false;
                    }
                } while (aiHit);
            }
        }

        private void userBoard_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}