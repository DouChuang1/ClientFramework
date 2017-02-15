using UnityEngine;
using System.Collections;

public class LoginCtrl : Singleton<LoginCtrl> {

	public void Enter()
    {
        Event.Broadcast(EGameEvent.eGameEvent_LoginEnter);
    }

    public void Exit()
    {
        Event.Broadcast(EGameEvent.eGameEvent_LoginExit);
    }


}
