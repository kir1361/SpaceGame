using UnityEngine;

public class AutoDestroyAnimation : MonoBehaviour
{
    public void DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }
}
