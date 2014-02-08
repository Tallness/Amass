using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Model
{
    public class Match
    {
        public int ID { get; set; }
        public Board Board { get; private set; }

        public List<Player> Players { get; private set; }
        public int CurrentPlayerIndex;
        public Queue<Tile> AvailableTiles { get; private set; }
        public IList<Tile> PlayedTiles { get; private set; }
        public List<Chain> Chains { get; set; }
        public Dictionary<string, int> AvailableStock { get; private set; }
        public Queue<Decision> PendingDecisions { get; set; }

        public Match()
        {
            Board = new Board(12, 9);

            PlayedTiles = new List<Tile>();
            AvailableTiles = new Queue<Tile>();
            SetupTiles();
            if (this.AvailableTiles.Count != this.Board.Spaces)
            {
                throw new InvalidOperationException();
            }

            AvailableStock = new Dictionary<string, int>();
            SetupStockMarket();

            Players = new List<Player>();
            Chains = new List<Chain>();
        }

        public Match(List<Member> members)
            : this()
        {
            this.AddPlayers(members);
        }

        private void SetupTiles()
        {
            int i = 0;
            List<Tile> tempTiles = new List<Tile>();
            for (int c = 0; c < Board.Width; c++)
            {
                for (int r = 0; r < this.Board.Height; r++)
                {
                    Tile t = new Tile(i,r,c);
                    tempTiles.Add(t);
                    i++;
                }
            }
            tempTiles.Shuffle();
            this.AvailableTiles = new Queue<Tile>(tempTiles);
        }

        /// <summary>
        /// Initialize the list of available stocks and starting quantities.
        /// </summary>
        private void SetupStockMarket()
        {
            this.AvailableStock.Add("Continental", 25);
            this.AvailableStock.Add("Imperial", 25);
            this.AvailableStock.Add("American", 25);
            this.AvailableStock.Add("Festival", 25);
            this.AvailableStock.Add("Worldwide", 25);
            this.AvailableStock.Add("Luxor", 25);
            this.AvailableStock.Add("Tower", 25);
        }

        public void AddPlayers(IEnumerable<Member> members)
        {
            foreach (var member in members)
            {
                AddPlayer(member);
            }
        }

        public void AddPlayer(Member member)
        {
            Player p = new Player();
            p.Member = member;
            p.Money = 600;

            this.Players.Add(p);
        }

        public List<Tile> GetAdjacentTiles(int tileId)
        {
            List<Tile> temp = new List<Tile>(4);
            int tileCol = tileId / this.Board.Height;
            int tileRow = (tileId % this.Board.Height);

            if (tileRow > 0)
            {
                temp.Add(new Tile(tileId - 1, tileRow - 1, tileCol));
            }

            if (tileRow < this.Board.Height-1)
            {
                temp.Add(new Tile(tileId + 1, tileRow + 1, tileCol));
            }

            if (tileCol > 0)
            {
                temp.Add(new Tile(tileId-this.Board.Height,tileRow,tileCol-1));
            }

            if (tileCol < this.Board.Width-1)
            {
                temp.Add(new Tile(tileId + this.Board.Height, tileRow, tileCol + 1));
            }

            return temp;
        }
    }

    public class Player
    {
        public Member Member { get; set; }
        public int Money { get; set; }
        public Dictionary<Stock,int> StockHoldings { get; set; }
        public List<Tile> Tiles { get; set; }

        public Player()
        {
            Money = 600;
            StockHoldings = new Dictionary<Stock, int>();
            Tiles = new List<Tile>(6);
        }

        public void AddStock(Stock stock, int quantity)
        {
            if (this.StockHoldings.ContainsKey(stock))
            {
                this.StockHoldings[stock] += quantity;
            }
            else
            {
                this.StockHoldings.Add(stock, quantity);
            }
        }
    }

    public class Decision
    {
        public int PlayerIndex { get; set; }
        public DecisionType Type { get; set; }
        public Object Data { get; set; }
    }

    public enum DecisionType
    {
        NewChain,
        MergerOutcome
    }

    public static class ThreadSafeRandom
    {
        [ThreadStatic]
        private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }

    static class MyExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
