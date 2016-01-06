using Amass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amass.Engine.Actions
{
    public class DisposeOfStock : BaseGameAction
    {
        private int[] _disposals;
        private string _company;

        public DisposeOfStock(int[] disposals, string company, int playerIndex)
            : base(playerIndex)
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

            if (_disposals[(int)StockDisposalType.Sell] > 0)
            {
                match.Players[_playerIndex].StockHoldings[_company] -= _disposals[(int)StockDisposalType.Sell];
                match.AvailableStock[_company] += _disposals[(int)StockDisposalType.Sell];

                // TODO: Handle money-side of stock sale.
            }

            if (_disposals[(int)StockDisposalType.Trade] > 0)
            {
                // Move the disposed stock from player holdings to bank.
                match.Players[_playerIndex].StockHoldings[_company] -= _disposals[(int)StockDisposalType.Trade];
                match.AvailableStock[_company] += _disposals[(int)StockDisposalType.Trade];

                // Move the merger winner stock from bank to player holding at 1:2.
                match.Players[_playerIndex].StockHoldings[match.CurrentMergerWinner] += _disposals[(int)StockDisposalType.Trade] / 2;
                match.AvailableStock[match.CurrentMergerWinner] -= _disposals[(int)StockDisposalType.Trade] / 2;
            }
        }

        public override bool IsValid(Match match)
        {
            var nextDecision = match.PendingDecisions.First();
            var activeStocks = match.Chains.Select(c => c.Company);

            return nextDecision.PlayerIndex == this._playerIndex
                && nextDecision.Type == DecisionType.DisposeOfStock
                && _company.Equals(match.CurrentMergerLosers.Peek())
                && match.AvailableStock[match.CurrentMergerWinner] >= _disposals[(int)StockDisposalType.Trade] / 2
                && match.Players[_playerIndex].StockHoldings[_company] >= _disposals.Sum();
        }


    }
}
