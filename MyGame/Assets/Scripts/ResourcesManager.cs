using UnityEngine;
using RESOURCESMANAGER = ResourcesManager;

public class ResourcesManager : MonoBehaviour
{
    private static ResourcesManager _instance = null;

    public static ResourcesManager GetInstance()
    {
        return _instance = _instance == null ? new ResourcesManager() : _instance;
    }

    private void Awake()
    {
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
