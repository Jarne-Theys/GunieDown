using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    // This server as a singleton to run MonoBehaviour methods (such as WaitForSeconds) in non MonoBehaviour classes.
    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("No CoroutineRunner instance exists. Creating a new one");
                var obj = new GameObject("CoroutineRunner");
                GameObject.DontDestroyOnLoad(obj);
                _instance = obj.AddComponent<CoroutineRunner>();
            }
            return _instance;
        }
    }
}
