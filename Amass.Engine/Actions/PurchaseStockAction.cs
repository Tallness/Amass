using Amass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amass.Engine.Actions
{
    public class PurchaseStock : BaseGameAction
    {
        private Dictionary<string, int> _stocks;

        public PurchaseStock(int playerIndex, Dictionary<string, int> stocks)
            : base(playerIndex)
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
                var price = Engine.GetStockPrice(stock.Key, match);
                match.Players[_playerIndex].Money -= stock.Value * price;
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
}
