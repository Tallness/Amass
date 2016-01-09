using Amass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amass.Engine.Actions
{
    public class PlaceTile : BaseGameAction
    {
        private int _tileId;

        public PlaceTile(int playerIndex, int tileId)
            : base(playerIndex)
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
                Console.WriteLine("Does not extend any existing chains.");
                if (adjacentTiles.Count > 0)     //New tile creates a new chain.
                {
                    Console.WriteLine("New chain created.");
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

            if (match.PendingDecisions.Count > 0)
            {
                match.CurrentPhase = MatchPhase.HandlingDecisions;
            }
            else
            {
                match.CurrentPlayerIndex = match.CurrentPlayerIndex == match.Players.Count ? 0 : match.CurrentPlayerIndex + 1;
            }
        }

        public override bool IsValid(Match match)
        {
            var playerTiles = match.Players[this._playerIndex].Tiles;

            return match.CurrentPhase == MatchPhase.WaitingForMove
                && match.CurrentPlayerIndex == this._playerIndex
                && playerTiles.Any(t => t.Sequence == _tileId);
        }
    }
}
