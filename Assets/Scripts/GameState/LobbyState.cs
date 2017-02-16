using UnityEngine;
using System.Collections;
using System;



class LobbyState : IGameState
{
    GameStateType stateTo;

    public LobbyState()
    {
    }

    public GameStateType GetStateType()
    {
        return GameStateType.GS_Lobby;
    }

    public void SetStateTo(GameStateType gs)
    {
        stateTo = gs;
    }

    public void Enter()
    {
        SetStateTo(GameStateType.GS_Continue);
        Debug.LogError("Enter Lobby");

    }

  

    public void Exit()
    {
       

    }

    public void FixedUpdate(float fixedDeltaTime)
    {
    }

    public GameStateType Update(float fDeltaTime)
    {
        return stateTo;
    }

    public void OnEvent(CEvent evt)
    {
     

    }

}



