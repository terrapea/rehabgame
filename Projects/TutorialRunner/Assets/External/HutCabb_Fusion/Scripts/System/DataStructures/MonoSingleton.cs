using UnityEngine;
public abstract class IxoMonoSingleton<T> : MonoBehaviour where T : IxoMonoSingleton<T>
{
    private static T _instance = null;
    public static T Instance
    {
        get
        {
            // Instance requiered for the first time, we look for it
            if( _instance == null )
            {
                _instance = GameObject.FindObjectOfType(typeof(T)) as T;
 
                // Object not found, we create a temporary one
                if( _instance == null )
                {
                    Debug.Log("No instance of " + typeof(T).ToString() + ", a new one is created.");
                    _instance = new GameObject("Temp Instance of " + typeof(T).ToString(), typeof(T)).GetComponent<T>();
 
                    // Problem during the creation, this should not happen
                    if( _instance == null )
                    {
                        Debug.LogError("Problem during the creation of " + typeof(T).ToString());
                    }
                }
            }
            return _instance;
        }
    }
    // If no other monobehaviour request the instance in an awake function
    // executing before this one, no need to search the object.
    virtual protected void Awake()
    {
        if( _instance == null )
            _instance = this as T;
    }
 
    // Make sure the instance isn't referenced anymore when the user quit, just in case.
    protected virtual void OnDestroy()
    {
        _instance = null;
		Destroy( gameObject );
    }
}