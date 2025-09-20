using DG.Tweening;
using TMPro;
using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    public MeteorPool meteorPool;
    public float spawnRate = 1f;
    public int spawnLimit = 10;
    public int SpawnCount = 0;
    public Transform MeteorMessagePos;

    public void RepeatMeteor()
    {
        InvokeRepeating(nameof(SpawnMeteor), 1f, spawnRate);
    }

    public void SpawnMeteor()
    {
        if (SpawnCount >= spawnLimit)
        {
            CancelInvoke(nameof(SpawnMeteor));
            return;
        }
        GameObject meteor = meteorPool.GetMeteor();

        meteor.transform.position = new Vector2(Random.Range(-4.5f, 4.5f), 11f);

        SpawnCount++;

    }
}
