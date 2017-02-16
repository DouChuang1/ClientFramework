using UnityEngine;
using System.Collections;

public class LoginState : IGameState {

    GameStateType stateTo;

	public LoginState()
    {

    }

    public GameStateType GetStateType()
    {
        return GameStateType.GS_Login;
    }

    public void SetStateTo(GameStateType gs)
    {
        stateTo = gs;
    }

    public void Enter()
    {
        SetStateTo(GameStateType.GS_Continue);

        LoginCtrl.Instance.Enter();

        //添加时间监听 
        //Event.AddListener<CEvent>(EGameEvent.eGameEvent_InputUserData, OnEvent);
        Event.AddListener<CEvent>(EGameEvent.eGameEvent_IntoLobby, OnEvent);
    }

    public void Exit()
    {
        LoginCtrl.Instance.Exit();
        Debug.LogError("LoginState Exit");

        //移除事件监听
        //Event.RemoveListener<CEvent>(EGameEvent.eGameEvent_InputUserData, OnEvent);
        Event.RemoveListener<CEvent>(EGameEvent.eGameEvent_IntoLobby, OnEvent);
    }

    public void OnEvent(CEvent evt)
    {
        switch (evt.GetEventId())
        {
            case EGameEvent.eGameEvent_InputUserData:
                SetStateTo(GameStateType.GS_User);
                break;
            case EGameEvent.eGameEvent_IntoLobby:
                GameStateManager.Instance.ChangeGameStateTo(GameStateType.GS_Lobby);
                break;
        }
    }

    public void FixedUpdate(float fixedDeltaTime)
    {

    }

    public GameStateType Update(float fDeltaTime)
    {
        return stateTo;
    }
}
