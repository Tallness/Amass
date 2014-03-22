using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Amass.Engine
{
    public static class Engine
    {

        public static void DrawForFirstMove(Match match)
        {
            int bestTile = match.Board.Spaces + 1;
            int playerIndex = 0;

            for (int i = 0; i < match.Players.Count; i++)
            {
                Tile t = match.AvailableTiles.Dequeue();
                Console.WriteLine("Player {0} draws {1}", i, t.Description);
                match.PlayedTiles.Add(t);

                if (t.Sequence < bestTile)
                {
                    bestTile = t.Sequence;
                    playerIndex = i;
                }
            }
            Console.WriteLine("Player {0} wins.", playerIndex);
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

        public static void PlayTile(Match match, int playerIndex, int tileId)
        {
            var playerTiles = match.Players[playerIndex].Tiles;
            Tile playedTile = playerTiles.Find(t => t.Sequence == tileId);

            playerTiles.Remove(playedTile);
            match.PlayedTiles.Add(playedTile);

            //Find list of spaces adjacent to played tile.
            List<Tile> adjacentTiles = match.GetAdjacentTiles(tileId);

            //Check for adjacent chains.
            List<Chain> adjacentChains = new List<Chain>();
            Console.Write("Adjacent Tiles: ");
            foreach (var t in adjacentTiles)
            {
                Chain c = match.Chains.FirstOrDefault(ch => ch.Tiles.Contains(t));
                if (c != null && !adjacentChains.Contains(c))
                {
                    adjacentChains.Add(c);
                }

                Console.Write(t.Description);
                if (adjacentTiles.IndexOf(t) < 3) Console.Write(" | ");
            }
            Console.WriteLine();

            if (adjacentChains.Count == 0)  //No existing chains, check for new one.
            {
                if (adjacentTiles.Count > 0)     //New tile creates a new chain.
                {
                    Chain c = new Chain();
                    c.Tiles.Add(playedTile);
                    c.Tiles.AddRange(adjacentTiles);
                    //TODO: Handle pseudo-chains from draw for first move.
                    match.Chains.Add(c);

                    match.PendingDecisions.Enqueue(new Decision
                    {
                        PlayerIndex = match.CurrentPlayerIndex,
                        Type = DecisionType.NewChain,
                        Data = match.AvailableStock.Where(s => !match.Chains.Select(ch => ch.Company).Contains(s.Key))
                    });
                    //match.Players[match.CurrentPlayerIndex].AddStock(match.AvailableStock)
                }
            }
            else if (adjacentChains.Count == 1)    //Tile extends existing chain.
            {
                Chain c = adjacentChains.Single();
                c.Tiles.Add(playedTile);
                c.Tiles.AddRange(adjacentTiles.Where(t => !c.Tiles.Contains(t)));
            }
            else //Tile creates a merger.
            {
                adjacentChains.Sort(delegate(Chain a, Chain b)
                {
                    return a.Tiles.Count.CompareTo(b.Tiles.Count);
                });
            }
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


    }


    public abstract class GameAction
    {
        protected int _playerIndex;

        public GameAction(int playerId)
        {
            _playerIndex = playerId;
        }

        public abstract void Apply(Match match);
        public abstract bool IsValid(Match match);
    }

    public class PlaceTile : GameAction
    {
        private int _tileId;

        public PlaceTile(int playerIndex, int tileId)
            :base(playerIndex)
        {
            _tileId = tileId;
        }

        public override void Apply(Match match)
        {
            if (!this.IsValid(match))
            {
                return;
            }


            var playerTiles = match.Players[_playerIndex].Tiles;
            Tile playedTile = playerTiles.Find(t => t.Sequence == _tileId);

            playerTiles.Remove(playedTile);
            match.PlayedTiles.Add(playedTile);

            //Find list of spaces adjacent to played tile.
            List<Tile> adjacentTiles = match.GetAdjacentTiles(_tileId);

            //Check for adjacent chains.
            List<Chain> adjacentChains = new List<Chain>();
            Console.Write("Adjacent Tiles: ");
            foreach (var t in adjacentTiles)
            {
                Chain c = match.Chains.FirstOrDefault(ch => ch.Tiles.Contains(t));
                if (c != null && !adjacentChains.Contains(c))
                {
                    adjacentChains.Add(c);
                }

                Console.Write(t.Description);
                if (adjacentTiles.IndexOf(t) < 3) Console.Write(" | ");
            }
            Console.WriteLine();

            if (adjacentChains.Count == 0)  //No existing chains, check for new one.
            {
                if (adjacentTiles.Count > 0)     //New tile creates a new chain.
                {
                    Chain c = new Chain();
                    c.Tiles.Add(playedTile);
                    c.Tiles.AddRange(adjacentTiles);
                    //TODO: Handle pseudo-chains from draw for first move.
                    match.Chains.Add(c);

                    match.PendingDecisions.Enqueue(new Decision
                    {
                        PlayerIndex = match.CurrentPlayerIndex,
                        Type = DecisionType.ChooseNewStock
                        //Data = match.AvailableStock.Where(s => !match.Chains.Select(ch => ch.Company).Contains(s.Key))
                    });
                    //match.Players[match.CurrentPlayerIndex].AddStock(match.AvailableStock)
                }
            }
            else if (adjacentChains.Count == 1)    //Tile extends existing chain.
            {
                Chain c = adjacentChains.Single();
                c.Tiles.Add(playedTile);
                c.Tiles.AddRange(adjacentTiles.Where(t => !c.Tiles.Contains(t)));
            }
            else //Tile creates a merger.
            {
                adjacentChains.Sort(delegate(Chain a, Chain b)
                {
                    return a.Tiles.Count.CompareTo(b.Tiles.Count);
                });

                match.PendingDecisions.Enqueue(new Decision
                {
                    PlayerIndex = match.CurrentPlayerIndex,
                    Type = DecisionType.ChooseMergeOrder
                });
            }


            match.PendingDecisions.Enqueue(new Decision
            {
                PlayerIndex = match.CurrentPlayerIndex,
                Type = DecisionType.PurchaseStock
            });

            match.CurrentPhase = MatchPhase.HandlingDecisions;
        }

        public override bool IsValid(Match match)
        {
            var playerTiles = match.Players[this._playerIndex].Tiles;

            return match.CurrentPhase == MatchPhase.WaitingForMove
                && match.CurrentPlayerIndex == this._playerIndex
                && playerTiles.Any(t => t.Sequence == _tileId);
        }
    }

    public class ChooseNewStock : GameAction
    {
        private string _stock;

        public ChooseNewStock(int playerIndex, string company)
            :base(playerIndex)
        {
            _stock = company;
        }

        public override void Apply(Match match)
        {
            if (!this.IsValid(match))
            {
                return;
            }

            match.PendingDecisions.Dequeue();

            if (match.AvailableStock[_stock] > 0)
            {
                match.Players[_playerIndex].AddStock(_stock, 1);
                match.AvailableStock[_stock] -= 1;
            }

            Chain newChain = match.Chains.First(c => String.IsNullOrEmpty(c.Company));
            newChain.Company = _stock;

            if (match.PendingDecisions.Count == 0)
                match.CurrentPhase = MatchPhase.DrawingTile;
        }

        public override bool IsValid(Match match)
        {
            var nextDecision = match.PendingDecisions.First();

            return nextDecision.PlayerIndex == this._playerIndex
                && nextDecision.Type == DecisionType.ChooseNewStock
                && !match.Chains.Any(c => c.Company.Equals(_stock));
        }
    }

    public class SetMergeOrder : GameAction
    {
        private IList<string> _companies;

        public SetMergeOrder(int playerIndex, string[] companies)
            :base(playerIndex)
        {
            this._companies = companies.ToList();
        }

        public override void Apply(Match match)
        {
            if (!this.IsValid(match))
            {
                return;
            }

            match.PendingDecisions.Dequeue();
            
            match.SetMergerWinner(_companies[0]);
            _companies.RemoveAt(0);
            match.SetMergerLosers(_companies);
        }

        public override bool IsValid(Match match)
        {
            var nextDecision = match.PendingDecisions.First();

            return nextDecision.PlayerIndex == this._playerIndex
                && nextDecision.Type == DecisionType.ChooseMergeOrder;
        }
    }

    public class PurchaseStock : GameAction
    {
        private Dictionary<string,int> _stocks;

        public PurchaseStock(int playerIndex, Dictionary<string, int> stocks)
            :base(playerIndex)
        {
            _stocks = stocks;
        }

        public override void Apply(Match match)
        {
            if (!this.IsValid(match))
            {
                return;
            }

            match.PendingDecisions.Dequeue();

            foreach (var stock in _stocks)
            {
                match.Players[_playerIndex].AddStock(stock.Key, stock.Value);
                //TODO: Handle money aspect of stock purchase.
                match.Players[_playerIndex].Money -= 100;
                match.AvailableStock[stock.Key] -= stock.Value;
            }

            if (match.PendingDecisions.Count == 0)
                match.CurrentPhase = MatchPhase.DrawingTile;
        }

        public override bool IsValid(Match match)
        {
            var nextDecision = match.PendingDecisions.First();
            var activeStocks = match.Chains.Select(c => c.Company);
            // TODO: Check for available funds.

            return nextDecision.PlayerIndex == this._playerIndex
                && nextDecision.Type == DecisionType.PurchaseStock
                && _stocks.Sum(s => s.Value) <= 3
                && _stocks.All(s => activeStocks.Contains(s.Key))
                && _stocks.All(s => match.AvailableStock[s.Key] >= s.Value);
        }
    }

    public class DrawTile : GameAction
    {
        public DrawTile(int playerIndex)
            :base(playerIndex)
        {

        }

        public override void Apply(Match match)
        {
            if (!this.IsValid(match))
            {
                return;
            }

            match.PendingDecisions.Dequeue();

            var playerTiles = match.Players[_playerIndex].Tiles;

            //Draw new tile.
            playerTiles.Add(match.AvailableTiles.Dequeue());

            //Advance player index.
            match.CurrentPlayerIndex++;
            if (match.CurrentPlayerIndex >= match.Players.Count) match.CurrentPlayerIndex = 0;
            
            match.CurrentPhase = MatchPhase.WaitingForMove;
        }

        public override bool IsValid(Match match)
        {
            return match.CurrentPlayerIndex == this._playerIndex
                && match.CurrentPhase == MatchPhase.DrawingTile;
        }
    }

    public class DisposeOfStock : GameAction
    {
        private int[] _disposals;
        private string _company;

        public DisposeOfStock(int[] disposals, string company, int playerIndex)
            :base(playerIndex)
        {
            _company = company;
            _disposals = disposals;
        }
        

        public override void Apply(Match match)
        {
            if (!this.IsValid(match))
            {
                return;
            }

            match.PendingDecisions.Dequeue();

            if(_disposals[(int)DisposalType.Sell] > 0)
            {
                match.Players[_playerIndex].StockHoldings[_company] -= _disposals[(int)DisposalType.Sell];
                match.AvailableStock[_company] += _disposals[(int)DisposalType.Sell];

                // TODO: Handle money-side of stock sale.
            }
            
            if(_disposals[(int)DisposalType.Trade] > 0)
            {
                // Move the disposed stock from player holdings to bank.
                match.Players[_playerIndex].StockHoldings[_company] -= _disposals[(int)DisposalType.Trade];
                match.AvailableStock[_company] += _disposals[(int)DisposalType.Trade];

                // Move the merger winner stock from bank to player holding at 1:2.
                match.Players[_playerIndex].StockHoldings[match.CurrentMergerWinner] += _disposals[(int)DisposalType.Trade]/2;
                match.AvailableStock[match.CurrentMergerWinner] -= _disposals[(int)DisposalType.Trade]/2;
            }
        }

        public override bool IsValid(Match match)
        {
            var nextDecision = match.PendingDecisions.First();
            var activeStocks = match.Chains.Select(c => c.Company);

            return nextDecision.PlayerIndex == this._playerIndex
                && nextDecision.Type == DecisionType.DisposeOfStock
                && _company.Equals(match.CurrentMergerLosers.Peek())
                && match.AvailableStock[match.CurrentMergerWinner] >= _disposals[(int)DisposalType.Trade] / 2
                && match.Players[_playerIndex].StockHoldings[_company] >= _disposals.Sum();
        }


        public enum DisposalType
        {
            Hold,
            Sell,
            Trade
        }
    }
}
