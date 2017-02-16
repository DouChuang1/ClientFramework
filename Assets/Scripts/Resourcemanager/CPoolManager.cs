using UnityEngine;
using System.Collections;
using PathologicalGames;

public class CPoolManager : MonoBehaviour {

    private static CPoolManager m_instance = null;
    private Transform m_transform;
    private SpawnPool m_spawnPool;

    public static CPoolManager instance
    {
        get { return m_instance; }
    }

    void Awake()
    {
        m_instance = this;
        m_transform = transform;
    }

    /// <summary>
    /// 创建预设体对象池接口
    /// </summary>
    /// <param name="prefabTrans">预设体Transform</param>
    /// <param name="preloadAmount">预先加载对象数量</param>
    /// <returns></returns>
    public PrefabPool CreatePrefabPool(string spawnPoolName, Transform prefabTrans, int preloadAmount, System.Type type = null)
    {
        m_spawnPool=PoolManager.Pools[spawnPoolName];
        PrefabPool prefabPool = new PrefabPool(prefabTrans);
        prefabPool.preloadAmount = preloadAmount;
        prefabPool.preloadTime = false;
        prefabPool.limitInstances = true;
        prefabPool.limitAmount = 200;
        prefabPool.scriptType = type;
        m_spawnPool._perPrefabPoolOptions.Add(prefabPool);
        m_spawnPool.CreatePrefabPool(m_spawnPool._perPrefabPoolOptions[m_spawnPool._perPrefabPoolOptions.Count-1]);
        return prefabPool;
    }

    //从对象池中取出GameObject接口
    public GameObject GetFromPool(Transform prefabTrans,string spawnPoolName)
    {
        m_spawnPool=PoolManager.Pools[spawnPoolName];
        Transform prefabPoolIns = m_spawnPool.Spawn(prefabTrans);
        return prefabPoolIns.gameObject;
    }

    //从对象池中取出GameObject接口
    public GameObject GetFromPool(string spawnName, string spawnPoolName)
    {
        m_spawnPool = PoolManager.Pools[spawnPoolName];
        Transform prefabPoolIns = m_spawnPool.Spawn(spawnName);
        return prefabPoolIns.gameObject;
    }

    public SpawnPool GetPool(string spawnPoolName)
    {
        return PoolManager.Pools[spawnPoolName];
    }

    //放入对象池
    public void PutInPool(Transform prefabTrans,string spawnPoolName)
    { 
        m_spawnPool=PoolManager.Pools[spawnPoolName];
        m_spawnPool.Despawn(prefabTrans);
    }
    
}
