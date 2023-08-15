using UnityEngine;

public class AdManager : MonoBehaviour
{
    private void Start()
    {
        IronSource.Agent.init("1b3d5184d");
        IronSource.Agent.validateIntegration();
    }

    private void OnEnable() 
    {
        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
        
    }

    void OnApplicationPause(bool isPaused) 
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    private void SdkInitializationCompletedEvent()
    {
        
    }
}
