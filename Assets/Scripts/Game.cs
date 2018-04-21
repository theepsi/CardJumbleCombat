using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    PREPARATION_PHASE,
    DAMAGE_PHASE,
    KO_PHASE
}

public class Game : MonoBehaviour {

    private GameState currentState;
	
    public void InitGame()
    {
        // generate random deck for players (same deck but duplicated) (different random)

        // prepare player and enemy (life, cards, etc)

        // change game state to preparation
    }

    
}
