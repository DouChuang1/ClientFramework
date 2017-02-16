using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;

public class CPoolPrefabInsBase : MonoBehaviour {

    [HideInInspector]
    public SpawnPool pool = null;

    private Vector3 origianlPos=Vector3.zero;
    private Quaternion origianlRotation = Quaternion.identity;
    private Vector3 originalScale = Vector3.one;

    public virtual void Awake()
    {
        origianlPos = transform.localPosition;
        origianlRotation = transform.rotation;
        originalScale = transform.localScale;
        gameObject.tag = "SpawnPool";

    }

    public virtual void DestroyItem()
    {
        PushToPool();
        ResetDespawnTrans();
    }

    void PushToPool()
    {
        pool.Despawn(transform);
    }

    void ResetDespawnTrans()
    {
        //transform.parent = pool.transform;
        transform.SetParent(pool.transform);
        transform.localPosition = origianlPos;
        transform.rotation = origianlRotation;
        transform.localScale = originalScale;
    }
}
