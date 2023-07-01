using UnityEngine;

/*
* This can be inherited by any Game Object script,
* will guarantee that there will only be 1 instance allowed in the scene,
* and provide quick access to that instance through the static getter Instance
*
* If you want to use the functionality of Awake(), please use Init() instead
*
*
*/

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            Debug.LogError("No instance of " + typeof(T) + "exists in the scene!");

            return instance;
        }
    }

    protected void Awake()
    {
        if(instance == null)
        {
            instance = this as T;
            Init();
        }
        else
        {
            Debug.LogWarning("An instance of " + typeof(T) + "already exists in the scene. Say goodbye!");
            Destroy(gameObject);
        }
    }

    protected void OnDestroy()
    {
        if(this == instance)
        instance = null;
    }

    protected virtual void Init() { }
}
