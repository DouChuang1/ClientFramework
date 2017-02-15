using UnityEngine;
using System.Collections;

public class TestEvent : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Event.AddListener(EGameEvent.eGameEvent_TestNoParam, TestEventMethodNoParam);
        Event.AddListener<string>(EGameEvent.eGameEvent_TestSingleParam, TestEventMethodSingleParam);
        WindowsManager.Instance.ChangeScenseToLogin(EScenesType.EST_None);
        LoginCtrl.Instance.Enter();
	}
	
	// Update is called once per frame
    void TestEventMethodNoParam()
    {
        Debug.LogError("Test Event");
	}

    void TestEventMethodSingleParam(string name)
    {
        Debug.LogError("This GameObject Name is " + name);
    }


    void OnDestroy()
    {
        Event.RemoveListener(EGameEvent.eGameEvent_TestNoParam, TestEventMethodNoParam);
        Event.RemoveListener<string>(EGameEvent.eGameEvent_TestSingleParam, TestEventMethodSingleParam);
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            Event.Broadcast(EGameEvent.eGameEvent_TestNoParam);
            Event.Broadcast<string>(EGameEvent.eGameEvent_TestSingleParam,"MainCamera");
        }
    }
}
