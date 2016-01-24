using Amass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amass.Engine.Actions
{
    public class ChooseNewStock : BaseGameAction
    {
        private string _stock;

        public ChooseNewStock(int playerIndex, string company)
            : base(playerIndex)
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
            Console.WriteLine("   New chain will be: {0}", this._stock);

            if (match.AvailableStock[_stock] > 0)
            {
                match.Players[_playerIndex].AddStock(_stock, 1);
                match.AvailableStock[_stock] -= 1;
            }

            Chain newChain = match.Chains.First(c => String.IsNullOrEmpty(c.Company));
            newChain.Company = _stock;

            // Grant stock bonus for new chain.
            if (match.AvailableStock[_stock] >= 1)
            {
                match.AvailableStock[_stock] -= 1;
                match.Players[_playerIndex].AddStock(_stock, 1);
            }


            if (match.PendingDecisions.Count == 0)
                match.CurrentPhase = MatchPhase.DrawingTile;
        }

        public override bool IsValid(Match match)
        {
            var nextDecision = match.PendingDecisions.First();

            return nextDecision.PlayerIndex == this._playerIndex
                && nextDecision.Type == DecisionType.ChooseNewStock
                && !match.Chains.Any(c => c.Company!=null && c.Company.Equals(_stock));
        }
    }
}
