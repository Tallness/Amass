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
            Console.WriteLine("   {0} adjacent tiles on board", adjacentTiles.Count);

            if (adjacentTiles.Count > 0)
            {

                Console.WriteLine("   {0} adjacent tiles", adjacentTiles.Count);

                //Check for adjacent chains.
                List<Chain> adjacentChains = new List<Chain>();
                Console.Write("Adjacent Tiles: ");
                foreach (var t in adjacentTiles)
                {
                    Console.Write("   {0}", t.Description);
                    Chain c = match.Chains.FirstOrDefault(ch => ch.Tiles.Contains(t));
                    if (c != null && !adjacentChains.Contains(c))
                    {
                        adjacentChains.Add(c);
                    }

                    if (adjacentTiles.IndexOf(t) < adjacentTiles.Count) Console.Write(" | ");
                }
                Console.WriteLine();


                if (adjacentChains.Count == 0)  //No existing chains, new one created.
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
                match.AdvancePlayer();
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
