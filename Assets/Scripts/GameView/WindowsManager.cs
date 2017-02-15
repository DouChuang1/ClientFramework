using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum EScenesType
{
    EST_None,
    EST_Login,
    EST_Play,
}

public enum EWindowType
{
    EWT_LoginWindow,
}

public class WindowsManager : Singleton<WindowsManager> {

    private Dictionary<EWindowType, CBaseWindow> m_WindowDic;
	public WindowsManager()
    {
        m_WindowDic = new Dictionary<EWindowType, CBaseWindow>();

        m_WindowDic[EWindowType.EWT_LoginWindow] = new CLoginWindow();
    }

    public CBaseWindow GetWindow(EWindowType type)
    {
        if (m_WindowDic.ContainsKey(type))
            return m_WindowDic[type];

        return null;
    }

    public void Update(float deltaTime)
    {
        foreach (CBaseWindow pWindow in m_WindowDic.Values)
        {
            if (pWindow.IsVisible())
            {
                pWindow.Update(deltaTime);
            }
        }
    }

    public void ChangeScenseToPlay(EScenesType front)
    {
        foreach (CBaseWindow pWindow in m_WindowDic.Values)
        {
            if (pWindow.GetScenseType() == EScenesType.EST_Play)
            {
                pWindow.Init();

                if (pWindow.IsResident())
                {
                    pWindow.PreLoad();
                }
            }
            else if ((pWindow.GetScenseType() == EScenesType.EST_Login) && (front == EScenesType.EST_Login))
            {
                pWindow.Hide();
                pWindow.Realse();

                if (pWindow.IsResident())
                {
                    pWindow.DelayDestory();
                }
            }
        }
    }

    public void ChangeScenseToLogin(EScenesType front)
    {
        foreach (CBaseWindow pWindow in m_WindowDic.Values)
        {
            if (front == EScenesType.EST_None && pWindow.GetScenseType() == EScenesType.EST_None)
            {
                pWindow.Init();

                if (pWindow.IsResident())
                {
                    pWindow.PreLoad();
                }
            }

            if (pWindow.GetScenseType() == EScenesType.EST_Login)
            {
                pWindow.Init();

                if (pWindow.IsResident())
                {
                    pWindow.PreLoad();
                }
            }
            else if ((pWindow.GetScenseType() == EScenesType.EST_Play) && (front == EScenesType.EST_Play))
            {
                pWindow.Hide();
                pWindow.Realse();

                if (pWindow.IsResident())
                {
                    pWindow.DelayDestory();
                }
            }
        }
    }

    /// <summary>
    /// 隐藏该类型的所有Window
    /// </summary>
    /// <param name="front"></param>
    public void HideAllWindow(EScenesType front)
    {
        foreach (var item in m_WindowDic)
        {
            if (front == item.Value.GetScenseType())
            {
                Debug.Log(item.Key);
                item.Value.Hide();
                //item.Value.Realse();
            }
        }
    }

    public void ShowWindowOfType(EWindowType type)
    {
        CBaseWindow window;
        if (!m_WindowDic.TryGetValue(type, out window))
        {
            return;
        }
        window.Show();
    }

}
