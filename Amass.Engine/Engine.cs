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
                if(adjacentTiles.Count > 0)     //New tile creates a new chain.
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
            else if(adjacentChains.Count == 1)    //Tile extends existing chain.
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

        public void FinishTurn(Match match)
        {
            var playerTiles = match.Players[match.CurrentPlayerIndex].Tiles;

            //Draw new tile.
            playerTiles.Add(match.AvailableTiles.Dequeue());

            //Advance player index.
            match.CurrentPlayerIndex++;
            if (match.CurrentPlayerIndex >= match.Players.Count) match.CurrentPlayerIndex = 0;
        }


    }
}
