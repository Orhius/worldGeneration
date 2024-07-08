using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    GameObject objectPrefab = null;

    public void SpawnObject()
    {
        Instantiate(objectPrefab);
    }
}
