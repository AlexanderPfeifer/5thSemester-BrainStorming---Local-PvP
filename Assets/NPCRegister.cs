using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class NPCRegister : MonoBehaviour
{
    public List<GameObject> Zombies { get; private set; } = new List<GameObject>();

    public void RegisterZombie(GameObject zombie)
    {
        if (!Zombies.Contains(zombie))
        {
            Zombies.Add(zombie);
        }
    }
}
