using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    public List<GameObject> Zombies { get; private set; } = new List<GameObject>();

    public void RegisterZombie(GameObject zombie)
    {
        if(!Zombies.Contains(zombie))
        {
            Zombies.Add(zombie);
        }
    }

    public void UnregisterZombie(GameObject zombie)
    {
        if (Zombies.Contains(zombie))
        {
            Zombies.Remove(zombie);
        }
    }
}
