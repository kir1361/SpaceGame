using UnityEngine;

public class TransitionLoader : MonoBehaviour
{
    void Awake()
    {
        if (Transition.Instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("TransitionCanvas");
            if (prefab != null)
            {
                Instantiate(prefab);
            }
            else
            {
                Debug.LogError("TransitionCanvas prefab not found in Resources!");
            }
        }
        Destroy(gameObject);
    }
}
