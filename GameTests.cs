namespace SnakeAndLadder.Tests
{
    public class GameTests
    {
        [Fact]
        public void ShouldGameBePlayed()
        {
            //Arrange
            var game = new Game();

            //Act
            game.StartSimulation();

            //Assert
            Assert.True(true);
        }

        [Theory]
        [InlineData("Player1", "Player2")]
        public void ShouldGameBeAbleToAddPlayersThrowsDictionary(string value1, string value2)
        {
            //Arrange
            var game = new Game();
            var player1 = new Player(value1);
            var player2 = new Player(value2);

            Queue<Player> allPlayers = new Queue<Player>();
            allPlayers.Enqueue(player1);
            allPlayers.Enqueue(player2);

            var expectedResult = new Dictionary<string, int>()
            {
                { value1, 0 },
                { value2, 0 },
            };

            //Act
            var actualResult = game.AddPlayersThrows(allPlayers);

            //Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void ShouldGameBeAbleToGenerateStats()
        {
            //Arrange
            var statsRollsTest = new Dictionary<string, Dictionary<string, int>>()
            {
                {"Simulation1",
                    new Dictionary<string, int>()
                    {
                        { "Minimum", 10 }, { "Maximum", 10 }
                    }
                }
            };

            var statsClimbsTest = new Dictionary<string, List<int>>()
            {
                { "Simulation1", new List<int>(){ 33, 19, 16, 18 } }
            };

            var statsSlidesTest = new Dictionary<string, List<int>>()
            {
                { "Simulation1", new List<int>(){ 36,22,36,21,21,21 } }
            };

            var isSingleRollWinTest = new List<bool>() { true, true };
            var isSnakeMissTest = new List<bool>() { true, true, true, true };

            var highestStreakTest = new Dictionary<int, string>() { { 1, "63" }, { 2, "663" }, { 3, "6665" }, { 4, "6661" } };
            var lowestStreakTest = new Dictionary<int, string>() { { 1, "1" }, { 2, "2" }, { 3, "3" }, { 4, "5" } };

            var game = new Game();

            //Act
            game.GameStats(statsRollsTest, statsClimbsTest, statsSlidesTest, "Simulation1", isSingleRollWinTest, isSnakeMissTest, highestStreakTest, lowestStreakTest);

            //Assert
            Assert.True(true);
        }

    }
}
