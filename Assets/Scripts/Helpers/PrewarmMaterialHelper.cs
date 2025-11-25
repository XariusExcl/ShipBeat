// Workaround for now, but sets materials to a cube to prevent lag spike when material is loaded again
// and also instantiates models
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class PrewarmMaterialHelper : MonoBehaviour
{
    [SerializeField] List<Material> materials;
    [SerializeField] List<GameObject> gameObjects;
    [SerializeField] MeshRenderer prewarmCube;
    public static UnityEvent MaterialPrewarmDone = new UnityEvent();
    public static bool IsMaterialPrewarmDone;

    void Start()
    {
        IsMaterialPrewarmDone = false;
        StartCoroutine(PrewarmMaterials());
    }

    public IEnumerator PrewarmMaterials()
    {
        List<GameObject> instantiatedStuff = new List<GameObject>();
        foreach(GameObject toInstantiate in gameObjects)
            instantiatedStuff.Add(Instantiate(toInstantiate, transform.position, Quaternion.identity));

        yield return new WaitForEndOfFrame();
        foreach(GameObject instance in instantiatedStuff)
            Destroy(instance);

        prewarmCube.enabled = true;
        foreach(Material material in materials)
        {
            prewarmCube.material = material;
            yield return new WaitForEndOfFrame();
        }
        prewarmCube.enabled = false;
        Debug.Log("Prewarm Done");
        MaterialPrewarmDone.Invoke();
        IsMaterialPrewarmDone = true;
    }   
}

