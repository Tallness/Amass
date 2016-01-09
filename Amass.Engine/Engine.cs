using Amass.Engine.Actions;
using Amass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amass.Engine
{
    public static class Engine
    {
        private static ICollection<PricePoint> _priceList = new List<PricePoint>{
                new PricePoint{Company="Tower",MinChainLength=2,MaxChainLength=2,StockPrice=200,FirstBonus=2000,SecondBonus=1000},
                new PricePoint{Company="Tower",MinChainLength=3,MaxChainLength=3,StockPrice=300,FirstBonus=3000,SecondBonus=1500},
                new PricePoint{Company="Tower",MinChainLength=4,MaxChainLength=4,StockPrice=400,FirstBonus=4000,SecondBonus=2000},
                new PricePoint{Company="Tower",MinChainLength=5,MaxChainLength=5,StockPrice=500,FirstBonus=5000,SecondBonus=2500},
                new PricePoint{Company="Tower",MinChainLength=6,MaxChainLength=10,StockPrice=600,FirstBonus=6000,SecondBonus=3000},
                new PricePoint{Company="Tower",MinChainLength=11,MaxChainLength=20,StockPrice=700,FirstBonus=7000,SecondBonus=3500},
                new PricePoint{Company="Tower",MinChainLength=21,MaxChainLength=30,StockPrice=800,FirstBonus=8000,SecondBonus=4000},
                new PricePoint{Company="Tower",MinChainLength=31,MaxChainLength=40,StockPrice=900,FirstBonus=9000,SecondBonus=4500},
                new PricePoint{Company="Tower",MinChainLength=41,MaxChainLength=1000,StockPrice=1000,FirstBonus=10000,SecondBonus=5000},
                new PricePoint{Company="Luxor",MinChainLength=2,MaxChainLength=2,StockPrice=200,FirstBonus=2000,SecondBonus=1000},
                new PricePoint{Company="Luxor",MinChainLength=3,MaxChainLength=3,StockPrice=300,FirstBonus=3000,SecondBonus=1500},
                new PricePoint{Company="Luxor",MinChainLength=4,MaxChainLength=4,StockPrice=400,FirstBonus=4000,SecondBonus=2000},
                new PricePoint{Company="Luxor",MinChainLength=5,MaxChainLength=5,StockPrice=500,FirstBonus=5000,SecondBonus=2500},
                new PricePoint{Company="Luxor",MinChainLength=6,MaxChainLength=10,StockPrice=600,FirstBonus=6000,SecondBonus=3000},
                new PricePoint{Company="Luxor",MinChainLength=11,MaxChainLength=20,StockPrice=700,FirstBonus=7000,SecondBonus=3500},
                new PricePoint{Company="Luxor",MinChainLength=21,MaxChainLength=30,StockPrice=800,FirstBonus=8000,SecondBonus=4000},
                new PricePoint{Company="Luxor",MinChainLength=31,MaxChainLength=40,StockPrice=900,FirstBonus=9000,SecondBonus=4500},
                new PricePoint{Company="Luxor",MinChainLength=41,MaxChainLength=1000,StockPrice=1000,FirstBonus=10000,SecondBonus=5000},
                new PricePoint{Company="American",MinChainLength=2,MaxChainLength=2,StockPrice=300,FirstBonus=3000,SecondBonus=1500},
                new PricePoint{Company="American",MinChainLength=3,MaxChainLength=3,StockPrice=400,FirstBonus=4000,SecondBonus=2000},
                new PricePoint{Company="American",MinChainLength=4,MaxChainLength=4,StockPrice=500,FirstBonus=5000,SecondBonus=2500},
                new PricePoint{Company="American",MinChainLength=5,MaxChainLength=5,StockPrice=600,FirstBonus=6000,SecondBonus=3000},
                new PricePoint{Company="American",MinChainLength=6,MaxChainLength=10,StockPrice=700,FirstBonus=7000,SecondBonus=3500},
                new PricePoint{Company="American",MinChainLength=11,MaxChainLength=20,StockPrice=800,FirstBonus=8000,SecondBonus=4000},
                new PricePoint{Company="American",MinChainLength=21,MaxChainLength=30,StockPrice=900,FirstBonus=9000,SecondBonus=4500},
                new PricePoint{Company="American",MinChainLength=31,MaxChainLength=40,StockPrice=1000,FirstBonus=10000,SecondBonus=5000},
                new PricePoint{Company="American",MinChainLength=41,MaxChainLength=1000,StockPrice=1100,FirstBonus=11000,SecondBonus=5500},
                new PricePoint{Company="Worldwide",MinChainLength=2,MaxChainLength=2,StockPrice=300,FirstBonus=3000,SecondBonus=1500},
                new PricePoint{Company="Worldwide",MinChainLength=3,MaxChainLength=3,StockPrice=400,FirstBonus=4000,SecondBonus=2000},
                new PricePoint{Company="Worldwide",MinChainLength=4,MaxChainLength=4,StockPrice=500,FirstBonus=5000,SecondBonus=2500},
                new PricePoint{Company="Worldwide",MinChainLength=5,MaxChainLength=5,StockPrice=600,FirstBonus=6000,SecondBonus=3000},
                new PricePoint{Company="Worldwide",MinChainLength=6,MaxChainLength=10,StockPrice=700,FirstBonus=7000,SecondBonus=3500},
                new PricePoint{Company="Worldwide",MinChainLength=11,MaxChainLength=20,StockPrice=800,FirstBonus=8000,SecondBonus=4000},
                new PricePoint{Company="Worldwide",MinChainLength=21,MaxChainLength=30,StockPrice=900,FirstBonus=9000,SecondBonus=4500},
                new PricePoint{Company="Worldwide",MinChainLength=31,MaxChainLength=40,StockPrice=1000,FirstBonus=10000,SecondBonus=5000},
                new PricePoint{Company="Worldwide",MinChainLength=41,MaxChainLength=1000,StockPrice=1100,FirstBonus=11000,SecondBonus=5500},
                new PricePoint{Company="Festival",MinChainLength=2,MaxChainLength=2,StockPrice=300,FirstBonus=3000,SecondBonus=1500},
                new PricePoint{Company="Festival",MinChainLength=3,MaxChainLength=3,StockPrice=400,FirstBonus=4000,SecondBonus=2000},
                new PricePoint{Company="Festival",MinChainLength=4,MaxChainLength=4,StockPrice=500,FirstBonus=5000,SecondBonus=2500},
                new PricePoint{Company="Festival",MinChainLength=5,MaxChainLength=5,StockPrice=600,FirstBonus=6000,SecondBonus=3000},
                new PricePoint{Company="Festival",MinChainLength=6,MaxChainLength=10,StockPrice=700,FirstBonus=7000,SecondBonus=3500},
                new PricePoint{Company="Festival",MinChainLength=11,MaxChainLength=20,StockPrice=800,FirstBonus=8000,SecondBonus=4000},
                new PricePoint{Company="Festival",MinChainLength=21,MaxChainLength=30,StockPrice=900,FirstBonus=9000,SecondBonus=4500},
                new PricePoint{Company="Festival",MinChainLength=31,MaxChainLength=40,StockPrice=1000,FirstBonus=10000,SecondBonus=5000},
                new PricePoint{Company="Festival",MinChainLength=41,MaxChainLength=1000,StockPrice=1100,FirstBonus=11000,SecondBonus=5500},
                new PricePoint{Company="Imperial",MinChainLength=2,MaxChainLength=2,StockPrice=400,FirstBonus=4000,SecondBonus=2000},
                new PricePoint{Company="Imperial",MinChainLength=3,MaxChainLength=3,StockPrice=500,FirstBonus=5000,SecondBonus=2500},
                new PricePoint{Company="Imperial",MinChainLength=4,MaxChainLength=4,StockPrice=600,FirstBonus=6000,SecondBonus=3000},
                new PricePoint{Company="Imperial",MinChainLength=5,MaxChainLength=5,StockPrice=700,FirstBonus=7000,SecondBonus=3500},
                new PricePoint{Company="Imperial",MinChainLength=6,MaxChainLength=10,StockPrice=800,FirstBonus=8000,SecondBonus=4000},
                new PricePoint{Company="Imperial",MinChainLength=11,MaxChainLength=20,StockPrice=900,FirstBonus=9000,SecondBonus=4500},
                new PricePoint{Company="Imperial",MinChainLength=21,MaxChainLength=30,StockPrice=1000,FirstBonus=10000,SecondBonus=5000},
                new PricePoint{Company="Imperial",MinChainLength=31,MaxChainLength=40,StockPrice=1100,FirstBonus=11000,SecondBonus=5500},
                new PricePoint{Company="Imperial",MinChainLength=41,MaxChainLength=1000,StockPrice=1200,FirstBonus=12000,SecondBonus=6000},
                new PricePoint{Company="Continental",MinChainLength=2,MaxChainLength=2,StockPrice=400,FirstBonus=4000,SecondBonus=2000},
                new PricePoint{Company="Continental",MinChainLength=3,MaxChainLength=3,StockPrice=500,FirstBonus=5000,SecondBonus=2500},
                new PricePoint{Company="Continental",MinChainLength=4,MaxChainLength=4,StockPrice=600,FirstBonus=6000,SecondBonus=3000},
                new PricePoint{Company="Continental",MinChainLength=5,MaxChainLength=5,StockPrice=700,FirstBonus=7000,SecondBonus=3500},
                new PricePoint{Company="Continental",MinChainLength=6,MaxChainLength=10,StockPrice=800,FirstBonus=8000,SecondBonus=4000},
                new PricePoint{Company="Continental",MinChainLength=11,MaxChainLength=20,StockPrice=900,FirstBonus=9000,SecondBonus=4500},
                new PricePoint{Company="Continental",MinChainLength=21,MaxChainLength=30,StockPrice=1000,FirstBonus=10000,SecondBonus=5000},
                new PricePoint{Company="Continental",MinChainLength=31,MaxChainLength=40,StockPrice=1100,FirstBonus=11000,SecondBonus=5500},
                new PricePoint{Company="Continental",MinChainLength=41,MaxChainLength=1000,StockPrice=1200,FirstBonus=12000,SecondBonus=6000}
        };

        public static void DrawForFirstMove(Match match)
        {
            int bestTile = match.Board.Spaces + 1;
            int playerIndex = 0;

            for (int i = 0; i < match.Players.Count; i++)
            {
                Tile t = match.AvailableTiles.Dequeue();
                Console.WriteLine("{0} draws {1}", match.Players[i].Member.Name, t.Description);
                match.PlayedTiles.Add(t);

                if (t.Sequence < bestTile)
                {
                    bestTile = t.Sequence;
                    playerIndex = i;
                }
            }
            Console.WriteLine("Player {0} wins.", match.Players[playerIndex].Member.Name);
            match.CurrentPlayerIndex = playerIndex;
        }

        public static void DrawStartingTiles(Match match)
        {
            foreach (var player in match.Players)
            {
                Console.Write("{0} drew: ", player.Member.Name);
                for (int i = 0; i < 6; i++)
                {
                    player.Tiles.Add(match.AvailableTiles.Dequeue());
                    Console.Write(player.Tiles[i].Description);
                    if (i < 5) Console.Write(" | ");
                }
                Console.WriteLine();
            }
            match.CurrentPhase = MatchPhase.WaitingForMove;
        }

        public static void ExecuteAction(Match match,BaseGameAction action)
        {
            action.Apply(match);
        }

        public static void PlayTile(Match match, int playerIndex, int tileId)
        {
            //var playerTiles = match.Players[playerIndex].Tiles;
            //Tile playedTile = playerTiles.Find(t => t.Sequence == tileId);

            //playerTiles.Remove(playedTile);
            //match.PlayedTiles.Add(playedTile);

            ////Find list of spaces adjacent to played tile.
            //List<Tile> adjacentTiles = match.GetAdjacentTiles(tileId);

            ////Check for adjacent chains.
            //List<Chain> adjacentChains = new List<Chain>();
            //Console.Write("Adjacent Tiles: ");
            //foreach (var t in adjacentTiles)
            //{
            //    Chain c = match.Chains.FirstOrDefault(ch => ch.Tiles.Contains(t));
            //    if (c != null && !adjacentChains.Contains(c))
            //    {
            //        adjacentChains.Add(c);
            //    }

            //    Console.Write(t.Description);
            //    if (adjacentTiles.IndexOf(t) < 3) Console.Write(" | ");
            //}
            //Console.WriteLine();

            //if (adjacentChains.Count == 0)  //No existing chains, check for new one.
            //{
            //    if (adjacentTiles.Count > 0)     //New tile creates a new chain.
            //    {
            //        Chain c = new Chain();
            //        c.Tiles.Add(playedTile);
            //        c.Tiles.AddRange(adjacentTiles);
            //        //TODO: Handle pseudo-chains from draw for first move.
            //        match.Chains.Add(c);

            //        match.PendingDecisions.Enqueue(new Decision
            //        {
            //            PlayerIndex = match.CurrentPlayerIndex,
            //            Type = DecisionType.NewChain,
            //            Data = match.AvailableStock.Where(s => !match.Chains.Select(ch => ch.Company).Contains(s.Key))
            //        });
            //        //match.Players[match.CurrentPlayerIndex].AddStock(match.AvailableStock)
            //    }
            //}
            //else if (adjacentChains.Count == 1)    //Tile extends existing chain.
            //{
            //    Chain c = adjacentChains.Single();
            //    c.Tiles.Add(playedTile);
            //    c.Tiles.AddRange(adjacentTiles.Where(t => !c.Tiles.Contains(t)));
            //}
            //else //Tile creates a merger.
            //{
            //    adjacentChains.Sort(delegate(Chain a, Chain b)
            //    {
            //        return a.Tiles.Count.CompareTo(b.Tiles.Count);
            //    });
            //}
        }

        public static void FinishTurn(Match match)
        {
            var playerTiles = match.Players[match.CurrentPlayerIndex].Tiles;

            //Draw new tile.
            playerTiles.Add(match.AvailableTiles.Dequeue());

            //Advance player index.
            match.CurrentPlayerIndex++;
            if (match.CurrentPlayerIndex >= match.Players.Count) match.CurrentPlayerIndex = 0;
        }

        public static int GetStockPrice(string company, Match match)
        {
            var chainLength = match.Chains.Single(c => c.Company.Equals(company)).Tiles.Count;

            return _priceList.Single(p => p.Company.Equals(company)
                && chainLength >= p.MinChainLength
                && chainLength <= p.MaxChainLength).StockPrice;
        }

        public static decimal GetFirstBonus(string defunctCompany, Match match)
        {
            var chainLength = match.Chains.Single(c => c.Company.Equals(defunctCompany)).Tiles.Count;

            return _priceList.Single(p => p.Company.Equals(defunctCompany)
                && chainLength >= p.MinChainLength
                && chainLength <= p.MaxChainLength).FirstBonus;
        }

        public static decimal GetSecondBonus(string defunctCompany, Match match)
        {
            var chainLength = match.Chains.Single(c => c.Company.Equals(defunctCompany)).Tiles.Count;

            return _priceList.Single(p => p.Company.Equals(defunctCompany)
                && chainLength >= p.MinChainLength
                && chainLength <= p.MaxChainLength).SecondBonus;
        }

        private class PricePoint
        {
            public string Company { get; set; }
            public int MinChainLength { get; set; }
            public int MaxChainLength { get; set; }
            public int StockPrice { get; set; }
            public decimal FirstBonus { get; set; }
            public decimal SecondBonus { get; set; }
        }
    }

    public enum StockDisposalType
    {
        Hold,
        Sell,
        Trade
    }
}
