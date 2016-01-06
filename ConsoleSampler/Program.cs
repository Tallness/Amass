using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amass.Engine;
using Amass.Model;


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
                Player p = newMatch.Players[newMatch.CurrentPlayerIndex];
                Tile t = p.Tiles.First();
                Console.WriteLine("{0} plays tile: {1}", p.Member.Name, t.Description);
                Engine.PlayTile(newMatch, newMatch.CurrentPlayerIndex, t.Sequence);
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
