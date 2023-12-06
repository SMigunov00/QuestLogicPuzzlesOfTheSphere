namespace Runtime.Services.Pause
{
    public interface IPauseHandler
    {
        public bool IsPaused { get; }
        public void SetPaused(bool isPaused);
    }
}