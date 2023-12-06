namespace Runtime.Services.Pause
{
    public class PauseSystem : IPauseHandler
    {
        public bool IsPaused { get; private set; }

        public void SetPaused(bool isPaused)
        {
            IsPaused = isPaused;
        }
    }
}