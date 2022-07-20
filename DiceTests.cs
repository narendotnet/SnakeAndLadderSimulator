using System.Reflection;
using Xunit.Sdk;

namespace SnakeAndLadder.Tests
{
    public class DiceTests
    {
        [Theory]
        [InlineData(1, 6)]
        public void ShouldDiceValuesBetween1And6(int minDiceValue, int maxDiceValue)
        {
            //Arrange
            var die = new Dice();

            //Act
            var actualResult = die.RollDie();

            //Assert
            Assert.True(minDiceValue <= actualResult);
            Assert.True(maxDiceValue >= actualResult);
        }
    }
}