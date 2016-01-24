using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amass.Engine;
using Amass.Model;
using Amass.Engine.Actions;


namespace ConsoleSampler
{
    class Program
    {
        static List<Member> members = new List<Member>();

        static void Main(string[] args)
        {
            members.Add(new Member
            {
                ID = 1,
                Name = "Stephen"
            });
            members.Add(new Member
            {
                ID = 2,
                Name = "Jeff"
            });
            members.Add(new Member
            {
                ID = 3,
                Name = "Matt"
            });

            Match newMatch = new Match(members);
            Engine.DrawForFirstMove(newMatch);
            Engine.DrawStartingTiles(newMatch);

            Console.WriteLine("Press Any key to show next, <Esc> to exit.");

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("First turn: [{0}]", newMatch.Players[newMatch.CurrentPlayerIndex].Member.Name);
            Console.ResetColor();

            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                BaseGameAction action = null;
                switch (newMatch.CurrentPhase)
                {
                    case MatchPhase.WaitingForMove:
                        Player p = newMatch.Players[newMatch.CurrentPlayerIndex];
                        Tile t = p.Tiles.First();
                        action = new PlaceTile(newMatch.CurrentPlayerIndex, t.Sequence);
                        Console.WriteLine("  Plays tile: {1}", p.Member.Name, t.Description);
                        Engine.ExecuteAction(newMatch, action);
                        //Engine.PlayTile(newMatch, newMatch.CurrentPlayerIndex, t.Sequence); 
                        break;
                    case MatchPhase.HandlingDecisions:
                        Decision d = newMatch.PendingDecisions.Peek();
                        //Console.WriteLine("Decision Pending for [{0}]:", newMatch.Players[d.PlayerIndex].Member.Name);
                        //Console.WriteLine("   {0}", d.Type);
                        switch (d.Type)
                        {
                            case DecisionType.ChooseNewStock:
                                string newCompany = newMatch.AvailableStock
                                    .Select(s => s.Key)
                                    .Except(newMatch.Chains.Select(c => c.Company))
                                    .First();
                                action = new ChooseNewStock(d.PlayerIndex, newCompany);
                                break;
                            case DecisionType.ChooseMergeOrder:
                                break;
                            case DecisionType.DisposeOfStock:
                                break;
                            case DecisionType.PurchaseStock:
                                var stocks = new Dictionary<string, int>();
                                switch (newMatch.Chains.Count)
                                {
                                    case 0:
                                        break;
                                    case 1:
                                        stocks.Add(newMatch.Chains.First().Company, 2);
                                        break;
                                    default:
                                        stocks.Add(newMatch.Chains[0].Company, 1);
                                        stocks.Add(newMatch.Chains[1].Company, 1);
                                        break;
                                }
                                action = new PurchaseStock(d.PlayerIndex, stocks);
                                break;
                            default:
                                break;
                        }
                        Engine.ExecuteAction(newMatch, action);
                        break;
                    case MatchPhase.DrawingTile:
                        action = new DrawTile(newMatch.CurrentPlayerIndex);
                        Engine.ExecuteAction(newMatch, action);
                        break;
                    default:
                        break;
                }

                if (newMatch.CurrentPhase == MatchPhase.WaitingForMove)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("New turn: [{0}]", newMatch.Players[newMatch.CurrentPlayerIndex].Member.Name);
                    Console.ResetColor();
                }

                if (newMatch.PendingDecisions.Count>0)
                {
                    Console.WriteLine("  Decisions pending: {0}", newMatch.PendingDecisions.Count);
                }
                //Console.WriteLine("-");
            }
        }
    }
}
