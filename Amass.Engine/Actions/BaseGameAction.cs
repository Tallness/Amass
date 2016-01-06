using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amass.Model;

namespace Amass.Engine.Actions
{
    public abstract class BaseGameAction
    {
        protected int _playerIndex;

        public BaseGameAction(int playerId)
        {
            _playerIndex = playerId;
        }

        public abstract void Apply(Match match);
        public abstract bool IsValid(Match match);
    }
}
