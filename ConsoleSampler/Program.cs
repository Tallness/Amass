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
            ConsoleKeyInfo input;

            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                BaseGameAction action = null;
                switch (newMatch.CurrentPhase)
                {
                    case MatchPhase.WaitingForMove:
                        Player p = newMatch.Players[newMatch.CurrentPlayerIndex];
                        Tile t = p.Tiles.First();
                        action = new PlaceTile(newMatch.CurrentPlayerIndex, t.Sequence);
                        Console.WriteLine("{0} plays tile: {1}", p.Member.Name, t.Description);
                        Engine.ExecuteAction(newMatch, action);
                        //Engine.PlayTile(newMatch, newMatch.CurrentPlayerIndex, t.Sequence); 
                        break;
                    case MatchPhase.HandlingDecisions:
                        Decision d = newMatch.PendingDecisions.Peek();
                        Console.WriteLine("Decision Pending for [{0}]:", newMatch.Players[d.PlayerIndex].Member.Name);
                        Console.WriteLine("   {0}", d.Type);
                        switch (d.Type)
                        {
                            case DecisionType.ChooseNewStock:
                                action = new ChooseNewStock(d.PlayerIndex, "Tower");
                                break;
                            case DecisionType.ChooseMergeOrder:
                                break;
                            case DecisionType.DisposeOfStock:
                                break;
                            case DecisionType.PurchaseStock:
                                action = new PurchaseStock(d.PlayerIndex, new Dictionary<string, int>());
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
            }




            //foreach (var tile in newMatch.AvailableTiles)
            //{
            //    input = Console.ReadKey();
            //    if (input.Key == ConsoleKey.Escape) break;

            //    Console.WriteLine("Tile: {0}",tile.Description);
            //}
        }
    }
}
