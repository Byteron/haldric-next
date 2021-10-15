public class Scenario
{
    public int PlayerCount;
    public int CurrentPlayer;

    public void EndTurn()
    {
        CurrentPlayer = (CurrentPlayer + 1) % PlayerCount;
    }
}