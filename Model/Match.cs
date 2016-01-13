using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Amass.Model
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
        public Stack<string> CurrentMergerLosers { get; private set; }
        public string CurrentMergerWinner { get; private set; }

        public Queue<Decision> PendingDecisions { get; set; }
        public MatchPhase CurrentPhase { get; set; }

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
            PendingDecisions = new Queue<Decision>();
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

        public void AdvancePlayer()
        {
            this.CurrentPlayerIndex = this.CurrentPlayerIndex == this.Players.Count-1 ? 0 : this.CurrentPlayerIndex + 1;
        }

        public List<Tile> GetAdjacentTiles(int tileId)
        {
            var spaces = Board.GetAdjacentSpaces(tileId);
            var tiles = new List<Tile>();

            foreach (int space in spaces)
            {
                //Tile t = new Tile(space, space % this.Board.Height, space/this.Board.Height);
                if (PlayedTiles.Any(t => t.Sequence == space))
                {
                    tiles.Add(new Tile(space, space % this.Board.Height, space / this.Board.Height));
                }
            }

            return tiles;
        }

        public void SetMergerWinner(string company)
        {
            this.CurrentMergerWinner = company;
        }

        public void SetMergerLosers(IEnumerable<string> companies)
        {
            this.CurrentMergerLosers.Clear();
            foreach (var company in companies)
            {
                this.CurrentMergerLosers.Push(company);
            }
        }
    }

    public class Player
    {
        public Member Member { get; set; }
        public int Money { get; set; }
        public Dictionary<string,int> StockHoldings { get; set; }
        public List<Tile> Tiles { get; set; }

        public Player()
        {
            Money = 600;
            StockHoldings = new Dictionary<string, int>();
            Tiles = new List<Tile>(6);
        }

        public void AddStock(string stock, int quantity)
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
        //public Object Data { get; set; }
    }

    public enum DecisionType
    {
        ChooseNewStock,
        ChooseMergeOrder,
        DisposeOfStock,
        PurchaseStock
    }

    public enum MatchPhase
    {
        WaitingForMove,
        HandlingDecisions,
        DrawingTile
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
