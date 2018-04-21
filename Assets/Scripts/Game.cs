using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum GameState
{
    PREPARATION_PHASE,
    DAMAGE_PHASE,
    KO_PHASE
}

public class Game : MonoBehaviour {

    public EventSystem eventSystem;

    public Transform handReference;

    private GameState currentState;

    public void InitGame()
    {
        eventSystem.enabled = true;
        // generate random deck for players (same deck but duplicated) (different random)

        // prepare player and enemy (life, cards, etc)

        // change game state to preparation
    }

    
}
