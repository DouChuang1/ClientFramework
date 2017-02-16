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

    public void Login(string userName,string pwd)
    {
        if (userName == "小黑子" && pwd == "1992")
        {
            Event.Broadcast(EGameEvent.eGameEvent_LoginSccess);
        }
        else
        {
            Event.Broadcast(EGameEvent.eGameEvent_LoginFail);
        }
    }

}
