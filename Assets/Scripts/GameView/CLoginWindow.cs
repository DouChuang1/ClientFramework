using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CLoginWindow : CBaseWindow
{

    private InputField userName;
    private InputField password;
    public Button login;

    public CLoginWindow()
    {
        mResName = GameConstDefine.LoginPath;
        mScenesType = EScenesType.EST_Login;
        mResident = false;

    }
    ////////////////////////////继承接口/////////////////////////
    //类对象初始化
    public override void Init()
    {
        Event.AddListener(EGameEvent.eGameEvent_LoginEnter, Show);
        Event.AddListener(EGameEvent.eGameEvent_LoginExit, Hide);
    }


    //类对象释放
    public override void Realse()
    {
        Event.RemoveListener(EGameEvent.eGameEvent_LoginEnter, Show);
        Event.RemoveListener(EGameEvent.eGameEvent_LoginExit, Hide);
    }

    //窗口控件初始化
    protected override void InitWidget()
    {
        //userName = GameObject.Find("InputField_username").GetComponent<InputField>();
        //password = GameObject.Find("InputField_password").GetComponent<InputField>();
        login = GameObject.Find("Button_login").GetComponent<Button>();

        //login.onClick.AddListener()
    }



    protected override void RealseWidget()
    {

    }


    public override void OnEnable()
    {

    }
    //隐藏
    public override void OnDisable()
    {

    }

    //游戏事件注册
    protected override void OnAddListener()
    {

    }

    //游戏事件注消
    protected override void OnRemoveListener()
    {

    }
   
    
}
