using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnderworldEditor
{
    class PathFinding
    {
        public static void pathfind()
        {
            List<string> map = new List<string>
            {
"****************************************************************",
"****************************************************************",
"****************************************************************",
"****************************************************************",
"****************************************************************",
"****************************************************************",
"****************************************************************",
"****************************************************************",
"*******************************###******************************",
"********************#####*#***#   #*****************************",
"*******************#     #        #*****************************",
"*******************#     #        #*****************************",
"********************####    ### #####***************************",
"********************# # # # #    #   #####*#********************",
"*******************#        #  #            ###*****************",
"****************####         # #     ###    ##******************",
"***************#   #             #  ####    #*******************",
"**************#      #       #         #    #*******************",
"*************#  #### #               # #####********************",
"************#  #     #   #    ## # ##    #**********************",
"************  ##   #            #*###    ####*******************",
"************ #*#   # ##      #   ##  ##*#    #*#****************",
"**********##  #       # #                    ## #***************",
"*********#              #       # ##  #####      #####**********",
"********#  ####       # ####          *    ##         **********",
"*******#    #  #                 #    #    ##      ## #*********",
"*******#        #                ## ### ####        # #*********",
"******** *#   #    #### ## ###   #       #             *********",
"******** *        ##     # #        # ##   ##* ###*#   *********",
"******** #     #   #     ##          #   # #       #B# *********",
"******** ### ##           #  ##   #  ###           # # *********",
"*******# #     # #  #     #            # #    ##       *********",
"*******# # # # #           #              ##           #********",
"******#  # # #                         ###*        ## #*********",
"*****#   #     #       #          ##  # #** #   # #   #*********",
"******##  #####    ########  #          ###     *    #**********",
"*******##      #   #  #               #   #     #  ##***********",
"******#            #       # #     ####    #    ##*****#********",
"******#        #   #       ##*##   #   #    ## #****************",
"*****#  # ######   #         ###   #   #    #  #****************",
"******#        ##  #  #  #     #       #    # ###***************",
"******#            #  #  #     #   #          #  #**************",
"*******#           #     #   ###   ####          #**************",
"********# ######      #  # #####   #   ###   ##  #**************",
"********#  #   #   #  #   #             #     ###***************",
"*********#          ##    #    #   ##   # #   ##****************",
"**********#    #     # ##      # ##*#  #        #***************",
"***********#     ##       #    #  ##   # #  ####****************",
"************####          #  # #    #       #*******************",
"**************#   #*# ## ### # ###         #********************",
"***************###    #         #  #  ##  #*********************",
"*****************#   ##         #    ##*##**********************",
"******************#      #      #      #************************",
"*******************##    #             #************************",
"********************#   #*#  #  #  #  #*************************",
"*********************##**#             #************************",
"**************************#*************************************",
"****************************************************************",
"****#***********************************************************",
"***#A#**********************************************************",
"****#***********************************************************",
"****************************************************************",
"****************************************************************",
"****************************************************************"
            };

            var start = new PathTile();
            start.Y = map.FindIndex(x => x.Contains("A"));
            start.X = map[start.Y].IndexOf("A");


            var finish = new PathTile();
            finish.Y = map.FindIndex(x => x.Contains("B"));
            finish.X = map[finish.Y].IndexOf("B");

            start.SetDistance(finish.X, finish.Y);

            var activeTiles = new List<PathTile>();
            activeTiles.Add(start);
            var visitedTiles = new List<PathTile>();

            while (activeTiles.Any())
            {
                var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();

                if (checkTile.X == finish.X && checkTile.Y == finish.Y)
                {
                    //We found the destination and we can be sure (Because the the OrderBy above)
                    //That it's the most low cost option. 
                    var tile = checkTile;
                    Console.WriteLine("Retracing steps backwards...");
                    int NoOfSteps = 0;
                    while (true)
                    {
                        Console.WriteLine($"{tile.X} : {tile.Y}");
                        if (map[tile.Y][tile.X] == ' ')
                        {
                            var newMapRow = map[tile.Y].ToCharArray();
                            newMapRow[tile.X] = '*';
                            map[tile.Y] = new string(newMapRow);
                        }
                        NoOfSteps++;
                        tile = tile.Parent;
                        if (tile == null)
                        {
                            Console.WriteLine("Map looks like with "+NoOfSteps +" steps :");
                            map.ForEach(x => Console.WriteLine(x));
                            Console.WriteLine("Done!");
                            return;
                        }
                    }
                }

                visitedTiles.Add(checkTile);
                activeTiles.Remove(checkTile);

                var walkableTiles = GetWalkableTiles(map, checkTile, finish);

                foreach (var walkableTile in walkableTiles)
                {
                    //We have already visited this tile so we don't need to do so again!
                    if (visitedTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                        continue;

                    //It's already in the active list, but that's OK, maybe this new tile has a better value (e.g. We might zigzag earlier but this is now straighter). 
                    if (activeTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
                    {
                        var existingTile = activeTiles.First(x => x.X == walkableTile.X && x.Y == walkableTile.Y);
                        if (existingTile.CostDistance > checkTile.CostDistance)
                        {
                            activeTiles.Remove(existingTile);
                            activeTiles.Add(walkableTile);
                        }
                    }
                    else
                    {
                        //We've never seen this tile before so add it to the list. 
                        activeTiles.Add(walkableTile);
                    }
                }
            }

            Console.WriteLine("No Path Found!");
        }

        private static List<PathTile> GetWalkableTiles(List<string> map, PathTile currentTile, PathTile targetTile)
        {
            var possibleTiles = new List<PathTile>()
            {
                new PathTile { X = currentTile.X, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new PathTile { X = currentTile.X, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1},
                new PathTile { X = currentTile.X - 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
                new PathTile { X = currentTile.X + 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
            };

            possibleTiles.ForEach(tile => tile.SetDistance(targetTile.X, targetTile.Y));

            var maxX = map.First().Length - 1;
            var maxY = map.Count - 1;

            return possibleTiles
                    .Where(tile => tile.X >= 0 && tile.X <= maxX)
                    .Where(tile => tile.Y >= 0 && tile.Y <= maxY)
                    .Where(tile => map[tile.Y][tile.X] == ' ' || map[tile.Y][tile.X] == 'B')
                    .ToList();
        }
    }

    class PathTile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Cost { get; set; }
        public int Distance { get; set; }
        public int CostDistance => Cost + Distance;
        public PathTile Parent { get; set; }

        //The distance is essentially the estimated distance, ignoring walls to our target. 
        //So how many tiles left and right, up and down, ignoring walls, to get there. 
        public void SetDistance(int targetX, int targetY)
        {
            this.Distance = Math.Abs(targetX - X) + Math.Abs(targetY - Y);
        }
    }
}
