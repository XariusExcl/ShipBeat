using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class GraphicsStateCollectionTracing : MonoBehaviour
{
    public GraphicsStateCollection graphicsStateCollection;

    void Start()
    {
        graphicsStateCollection = new GraphicsStateCollection
        {
            runtimePlatform = RuntimePlatform.WebGLPlayer
        };
        graphicsStateCollection.BeginTrace();
        DontDestroyOnLoad(this);
    }

    void OnDestroy()
    {
        graphicsStateCollection.EndTrace();
        graphicsStateCollection.SaveToFile(Application.dataPath+"/graphicsStateCollection.graphicsstate");
    }
}