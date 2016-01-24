using Amass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amass.Engine.Actions
{
    public class DrawTile : BaseGameAction
    {
        public DrawTile(int playerIndex)
            : base(playerIndex)
        {

        }

        public override void Apply(Match match)
        {
            if (!this.IsValid(match))
            {
                return;
            }


            var playerTiles = match.Players[_playerIndex].Tiles;

            //Draw new tile.
            Tile newTile = match.AvailableTiles.Dequeue();
            playerTiles.Add(newTile);
            Console.WriteLine("  Drawing tile for [{0}] - {1}", match.Players[_playerIndex].Member.Name, newTile.Description);

            //Advance player index.
            match.AdvancePlayer();

            match.CurrentPhase = MatchPhase.WaitingForMove;
        }

        public override bool IsValid(Match match)
        {
            return match.CurrentPlayerIndex == this._playerIndex
                && match.CurrentPhase == MatchPhase.DrawingTile;
        }
    }
}
