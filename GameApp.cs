namespace SnakeAndLadder
{
    public class GameApp
    {
        IGame _game;
        public GameApp(IGame game)
        {
            _game = game;
        }

        public void StartSimulation()
        {
            _game.StartSimulation();
        }

    }
}
