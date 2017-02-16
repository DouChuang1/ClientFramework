using UnityEngine;
using System.Collections;

interface IGameState
{
    GameStateType GetStateType();
    void SetStateTo(GameStateType gsType);
    void Enter();
    GameStateType Update(float fDeltaTime);
    void FixedUpdate(float fixedDeltaTime);
    void Exit();

}
