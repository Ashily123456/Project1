using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventTrigger : MonoBehaviour
{
    public void FlagGameBegin()
    {
        // hide cutscene
        LevelManager.instance.introCanvas.SetActive(false);
        
        // flag game begin
        LevelManager.instance.gameStarted = true;
    }
}
