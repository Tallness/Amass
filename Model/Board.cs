using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amass.Model
{
    public class Board
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Board(int width, int height)
        {
            Height = height;
            Width = width;
        }

        public int Spaces
        {
            get
            {
                return Width * Height;
            }
        }

        public List<int> GetAdjacentSpaces(int spaceId)
        {
            var outSpaces = new List<int>();
            int col = spaceId / this.Height;
            int row = (spaceId % this.Height);

            if (row > 0)
            {
                outSpaces.Add(spaceId-1);
            }

            if (row < this.Height - 1)
            {
                outSpaces.Add(spaceId + 1);
            }

            if (col > 0)
            {
                outSpaces.Add(spaceId - this.Height);
            }

            if (col < this.Width - 1)
            {
                outSpaces.Add(spaceId + this.Height);
            }

            return outSpaces;
        }
    }

    public class Tile
    {
        public int Sequence { get; set; }
        public string Description { get; set; }
        public bool IsPlayable { get; set; }

        public Tile(int sequence, int row, int col)
        {
            this.Sequence = sequence;
            Description = (col+1).ToString() + "-" + (char)(row+1+64);
        }
    }

    public class Chain
    {
        public Chain()
        {
            this.Tiles = new List<Tile>();
        }
        public List<Tile> Tiles { get; set; }
        public string Company { get; set; }
    }

    public class Stock
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int StockClass { get; set; }
    }
}
