using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameStateType
{
    GS_Continue,
    GS_Login,
    GS_Loading,
    GS_Lobby,
    GS_User,
}

public class GameStateManager : Singleton<GameStateManager>
{

    Dictionary<GameStateType, IGameState> gameStates;
    IGameState currentState;

    public GameStateManager()
    {
        gameStates = new Dictionary<GameStateType, IGameState>();

        IGameState gameState;

        gameState = new LoginState();
        gameStates.Add(gameState.GetStateType(), gameState);

        gameState = new LobbyState();
        gameStates.Add(gameState.GetStateType(), gameState);


    }

    private IGameState GetCurState()
    {
        return currentState;
    }

    public void ChangeGameStateTo(GameStateType stateType)
    {
        if (currentState != null && currentState.GetStateType() != GameStateType.GS_Loading && currentState.GetStateType() == stateType)
            return;

        if (gameStates.ContainsKey(stateType))
        {
            if (currentState != null)
            {
                currentState.Exit();
            }

            currentState = gameStates[stateType];
            currentState.Enter();
        }
    }

    public void EnterDefaultState()
    {
        ChangeGameStateTo(GameStateType.GS_Login);
    }

    public void FixedUpdate(float fixedDeltaTime)
    {
        if (currentState != null)
        {
            currentState.FixedUpdate(fixedDeltaTime);
        }
    }

    public void Update(float fDeltaTime)
    {
        GameStateType nextStateType = GameStateType.GS_Continue;
        if (currentState != null)
        {
            nextStateType = currentState.Update(fDeltaTime);
        }

        if (nextStateType > GameStateType.GS_Continue)
        {
            ChangeGameStateTo(nextStateType);
        }
    }

    private IGameState getState(GameStateType type)
    {
        if (!gameStates.ContainsKey(type))
        {
            return null;
        }
        return gameStates[type];
    }
}
