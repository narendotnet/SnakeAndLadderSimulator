using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeAndLadder
{
    public class Game : IGame
    {
        public void StartSimulation()
        {
            var config = new ConfigurationBuilder().AddJsonFile($"appSettings.json", true, true).Build();
            int no_of_simulation = int.Parse(config.GetSection("no_of_simulation").Value);
            int game_board_size = int.Parse(config.GetSection("game_board_size").Value);
            var players_list = config.GetSection("players_list").GetChildren().Select(x => x.Value).ToArray();
            var position_of_snakes = config.GetSection("position_of_snakes").GetChildren().Select(x => x.Value).ToArray();
            var position_of_ladders = config.GetSection("position_of_ladders").GetChildren().Select(x => x.Value).ToArray();
            int i = 1;

            while (i <= no_of_simulation)
            {
                Dictionary<string, Dictionary<string, int>> statsRolls = new Dictionary<string, Dictionary<string, int>>();
                Dictionary<string, List<int>> statsClimbs = new Dictionary<string, List<int>>();
                Dictionary<string, List<int>> statsSlides = new Dictionary<string, List<int>>();

                string currentSimulation = "Simulation " + i;
                Dice dice = new Dice();
                Queue<Player> allPlayers = new Queue<Player>();
                Dictionary<string, int> playersCurrentPosition = new Dictionary<string, int>();

                foreach (var item in players_list)
                {
                    string name = item.Split(",").ToArray()[0];
                    Player player = new Player(name);
                    allPlayers.Enqueue(player);
                    playersCurrentPosition.Add(name, 0);
                }

                List<Position> snakes = new List<Position>();
                List<Position> ladders = new List<Position>();

                foreach (var position in position_of_snakes)
                {
                    int higher_numbered_square = int.Parse(position.Split(",").ToArray()[0]);
                    int lower_numbered_square = int.Parse(position.Split(",").ToArray()[1]);
                    Position snake = new Position(higher_numbered_square, lower_numbered_square);
                    snakes.Add(snake);
                }

                foreach (var position in position_of_ladders)
                {
                    int higher_numbered_square = int.Parse(position.Split(",").ToArray()[0]);
                    int lower_numbered_square = int.Parse(position.Split(",").ToArray()[1]);
                    Position ladder = new Position(higher_numbered_square, lower_numbered_square);
                    ladders.Add(ladder);
                }
                statsRolls.Add(currentSimulation, new Dictionary<string, int>());
                statsClimbs.Add(currentSimulation, new List<int>());
                statsSlides.Add(currentSimulation, new List<int>());
                Console.WriteLine("Simulation No is: " + i);
                Console.WriteLine("*******************");

                // Start the game
                StartGame(dice, allPlayers, snakes, ladders, playersCurrentPosition, game_board_size, statsRolls, statsClimbs, statsSlides, currentSimulation);
                i++;
            }
        }

        #region private methods

        private void StartGame(Dice dice, Queue<Player> allPlayers, List<Position> snakes, List<Position> ladders, Dictionary<string, int> playersCurrentPosition, int boardSize, Dictionary<string, Dictionary<string, int>> statsRolls, Dictionary<string, List<int>> statsClimbs, Dictionary<string, List<int>> statsSlides, string currentSimulation)
        {
            int i = 0;
            int j = 0;
            bool rolledAgain = false;
            Dictionary<int, string> highestStreak = new Dictionary<int, string>();
            Dictionary<int, string> lowestStreak = new Dictionary<int, string>();
            Dictionary<string, int> playersThrowsCount = AddPlayersThrows(allPlayers);
            List<bool> isSingleRollWin = new List<bool>();
            List<bool> isSnakeMiss = new List<bool>();

            while (allPlayers.Count() > 0)
            {
                int diceValue = dice.RollDie();
                if (diceValue == 6)
                {
                    if (rolledAgain == false)
                    {
                        i++;
                        highestStreak.Add(i, diceValue.ToString());
                    }
                    if (rolledAgain == true) highestStreak[highestStreak.Count] += diceValue.ToString();
                }
                else
                {
                    if (rolledAgain == false)
                    {
                        j++;
                        lowestStreak.Add(j, diceValue.ToString());
                    }
                    if (rolledAgain == true) highestStreak[highestStreak.Count] += diceValue.ToString();
                }

                Player player = allPlayers.Dequeue();
                int throws = playersThrowsCount[player.playerName] += 1;
                int currentPosition = playersCurrentPosition.FirstOrDefault(x => x.Key == player.playerName).Value;
                int nextCell = currentPosition + diceValue;

                if (nextCell > boardSize) allPlayers.Enqueue(player);
                else if (nextCell == boardSize)
                {
                    if (diceValue <= 5) isSingleRollWin.Add(true);
                    Console.WriteLine(player.playerName + " won the game\n");
                    UpdateStats(throws, allPlayers, statsRolls, currentSimulation);
                }
                else
                {
                    int[] nextPosition = new int[1];
                    bool[] b = new bool[1];
                    nextPosition[0] = nextCell;
                    snakes.ForEach(v =>
                    {
                        if (v.startPoint == nextCell)
                        {
                            nextPosition[0] = v.endPoint;
                            int distance_slided = v.startPoint - v.endPoint;
                            statsSlides[currentSimulation].Add(distance_slided);
                        }
                        if (nextCell == v.startPoint + 1 || nextCell == v.startPoint + 2)
                        {
                            isSnakeMiss.Add(true);
                        }
                    });
                    if (nextPosition[0] != nextCell) Console.WriteLine(player.playerName + " Bitten by Snake present at: " + nextCell);
                    ladders.ForEach(v =>
                    {
                        if (v.startPoint == nextCell)
                        {
                            nextPosition[0] = v.endPoint;
                            b[0] = true;
                            int distance_climbed = v.endPoint - v.startPoint;
                            statsClimbs[currentSimulation].Add(distance_climbed);
                        }
                    });
                    if (nextPosition[0] != nextCell && b[0]) Console.WriteLine(player.playerName + " Got ladder present at:  " + nextCell);
                    if (nextPosition[0] == boardSize)
                    {
                        Console.WriteLine(player.playerName + " won the game\n");
                        UpdateStats(throws, allPlayers, statsRolls, currentSimulation);
                    }
                    else
                    {
                        playersCurrentPosition[player.playerName] = nextPosition[0];
                        if (diceValue == 6)
                        {
                            rolledAgain = true;
                            Console.WriteLine(player.playerName + " Rolled 6!");
                            allPlayers.Enqueue(player);
                            allPlayers = new Queue<Player>(allPlayers.Reverse());
                        }
                        else
                        {
                            rolledAgain = false;
                            allPlayers.Enqueue(player);
                        }
                        Console.WriteLine(player.playerName + " is at position " + nextPosition[0]);
                    }
                }
            }
            GameStats(statsRolls, statsClimbs, statsSlides, currentSimulation, isSingleRollWin, isSnakeMiss, highestStreak, lowestStreak);
        }
        public void GameStats(Dictionary<string, Dictionary<string, int>> statsRolls, Dictionary<string, List<int>> statsClimbs, Dictionary<string, List<int>> statsSlides, string currentSimulation, List<bool> isSingleRollWin, List<bool> isSnakeMiss, Dictionary<int, string> highestStreak, Dictionary<int, string> lowestStreak)
        {
            int minRolls = (from outer in statsRolls
                            from inner in outer.Value
                            where inner.Key == "Minimum"
                            select inner)
                            .OrderBy(x => x.Value).First().Value;

            var avgRolls = (from outer in statsRolls
                            from inner in outer.Value
                            select inner.Value).Average();

            int maxRolls = (from outer in statsRolls
                            from inner in outer.Value
                            where inner.Key == "Maximum"
                            select inner)
                            .OrderByDescending(x => x.Value).First().Value;

            int minClimbs = (from outer in statsClimbs
                             from inner in outer.Value
                             select inner)
                             .OrderBy(x => x).First();

            var avgClimbs = (from outer in statsClimbs
                             from inner in outer.Value
                             select inner)
                             .Average();

            int maxClimbs = (from outer in statsClimbs
                             from inner in outer.Value
                             select inner)
                             .OrderByDescending(x => x).First();

            int minSlides = (from outer in statsSlides
                             from inner in outer.Value
                             select inner)
                             .OrderBy(x => x).First();

            var avgSlides = (from outer in statsSlides
                             from inner in outer.Value
                             select inner)
                             .Average();

            int maxSlides = (from outer in statsSlides
                             from inner in outer.Value
                             select inner)
                             .OrderByDescending(x => x).First();

            string longestTurn = highestStreak.Count > 0 ? highestStreak.OrderByDescending(x => x.Value).First().Value : lowestStreak.OrderByDescending(x => x.Value).First().Value;

            var unluckyRolls = (from outer in statsSlides
                                from inner in outer.Value
                                select inner);

            int playerLandsLadder = (from outer in statsClimbs
                                     from inner in outer.Value
                                     select inner).Count();

            int singleRollWin = isSingleRollWin.Count();

            int missesASnake = isSnakeMiss.Count();

            int minLuckyRolls = 1;
            int avgLuckyRolls = (playerLandsLadder + singleRollWin + missesASnake) / 2;
            int maxLuckyRolls = playerLandsLadder + singleRollWin + missesASnake;

            Console.WriteLine("===========================================");
            Console.WriteLine("Stats Captured from " + currentSimulation);
            Console.WriteLine("===========================================");
            Console.WriteLine("Minimum number of rolls needed to win: {0}", minRolls);
            Console.WriteLine("Average number of rolls needed to win: {0}", (int)avgRolls);
            Console.WriteLine("Maximum number of rolls needed to win: {0}", maxRolls);

            Console.WriteLine("Minimum amount of climbs during the game: {0}", minClimbs);
            Console.WriteLine("Average amount of climbs during the game: {0}", (int)avgClimbs);
            Console.WriteLine("Maximum amount of climbs during the game: {0}", maxClimbs);

            Console.WriteLine("Minimum amount of slides during the game: {0}", minSlides);
            Console.WriteLine("Average amount of slides during the game: {0}", (int)avgSlides);
            Console.WriteLine("Maximum amount of slides during the game: {0}", maxSlides);

            Console.WriteLine("The biggest climb in a single turn: {0}", maxClimbs);
            Console.WriteLine("The biggest slide in a single turn: {0}", maxSlides);

            Console.WriteLine("Longest turn: {0}", longestTurn);

            Console.WriteLine("Minimum number of unlucky rolls during the game: {0}", unluckyRolls.Any() ? 1 : 0);
            Console.WriteLine("Average number of unlucky rolls during the game: {0}", unluckyRolls.Count() / 2);
            Console.WriteLine("Maximum number of unlucky rolls during the game: {0}", unluckyRolls.Count());

            Console.WriteLine("Minimum number of lucky rolls during the game: {0}", minLuckyRolls);
            Console.WriteLine("Average number of lucky rolls during the game: {0}", avgLuckyRolls);
            Console.WriteLine("Maximum number of lucky rolls during the game: {0}\n", maxLuckyRolls);
        }
        private void UpdateStats(int throws, Queue<Player> allPlayers, Dictionary<string, Dictionary<string, int>> statsRolls, string currentSimulation)
        {
            if (allPlayers.Count() == 0)
            {
                statsRolls[currentSimulation].Add("Maximum", throws);
            }
            else { statsRolls[currentSimulation].Add("Minimum", throws); }

        }
        public Dictionary<string, int> AddPlayersThrows(Queue<Player> allPlayers)
        {
            var result = new Dictionary<string, int>();
            foreach (var item in allPlayers)
            {
                result.Add(item.playerName, 0);
            }
            return result;
        }

        #endregion
    }
}
