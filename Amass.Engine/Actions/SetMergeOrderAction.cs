using Amass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amass.Engine.Actions
{
    public class SetMergeOrder : BaseGameAction
    {
        private IList<string> _companies;

        public SetMergeOrder(int playerIndex, string[] companies)
            : base(playerIndex)
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
}
