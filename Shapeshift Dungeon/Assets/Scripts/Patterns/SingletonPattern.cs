using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  HOW TO USE THIS SCRIPT TO MAKE SINGLETONS:
    1. Set your Singleton class to derive from SingletonPattern<ScriptName> instead of from Monobehaviour
    2. From any other class, you can now use ScriptName.Instance.FunctionName(); to call public functions from your Singletons
*/

public abstract class SingletonPattern<T> : MonoBehaviour where T : SingletonPattern<T>
{
    public static T Instance;

    //To create Awake in derived classes, use an override Awake and call base.Awake()
    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this as T;
        //DontDestroyOnLoad(this);
    }
}
