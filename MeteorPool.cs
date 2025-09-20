using System.Collections.Generic;
using UnityEngine;

public class MeteorPool : MonoBehaviour
{
    public GameObject meteorPrefab;  
    public int poolSize = 10;

    private List<GameObject> meteorPool;

    void Start()
    {
        meteorPool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
          GameObject  meteor = Instantiate(meteorPrefab);

            meteor.SetActive(false);
            meteorPool.Add(meteor);
        }
    }

    public GameObject GetMeteor()
    {
        foreach (GameObject meteor in meteorPool)
        {
            if (!meteor.activeInHierarchy) 
            {
                meteor.SetActive(true);
                return meteor;
            }
        }


        GameObject newMeteor = Instantiate(meteorPrefab);
        meteorPool.Add(newMeteor);
        return newMeteor;
    }
}
