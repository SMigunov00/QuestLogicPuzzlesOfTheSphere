using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseSystem :  IPauseHandler
{
    public bool IsPaused { get; private set; }

    public void SetPaused(bool isPaused)
    {
        IsPaused = isPaused;
/*if (isPaused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;*/
    }
}
