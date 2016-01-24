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

            if (this._stocks.Count>0)
            {
                Console.WriteLine("   [{0}] Purchasing stock in: ",match.Players[this._playerIndex].Member.Name);

                foreach (var stock in _stocks)
                {
                    match.Players[_playerIndex].AddStock(stock.Key, stock.Value);

                    var price = Engine.GetStockPrice(stock.Key, match);
                    Console.WriteLine("      - {1}x {0} - spending {2:C}", stock.Key, stock.Value, stock.Value * price);
                    match.Players[_playerIndex].Money -= stock.Value * price;
                    match.AvailableStock[stock.Key] -= stock.Value;
                }
            }
            else
            {
                Console.WriteLine("   No active chains to purchase stock in");
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
