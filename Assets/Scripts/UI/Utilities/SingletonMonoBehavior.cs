using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMonoBehavior<T> : MonoBehaviour where T : SingletonMonoBehavior<T>
{
    protected bool BeingDestroyed;
    private static T instance;
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<T>();
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if(Instance.Equals(this))
        {
            if(instance.transform.parent != null)
            {
                instance.transform.parent = null;
            }
            DontDestroyOnLoad(instance.gameObject);
        }
        else
        {
            Destroy(gameObject);
            BeingDestroyed = true;
        }
    }
}
