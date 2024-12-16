using System.Collections.Generic;
using UnityEngine;

public class ZombiePlayerHordeRegistry : MonoBehaviour
{
    public List<GameObject> Zombies { get; private set; } = new List<GameObject>();

    [SerializeField] private int playerIndex;

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

            if(Zombies.Count == 0 )
            {
                FindAnyObjectByType<ResurrectZombieHorde>().ResurrectNumberOfZombies(1, zombie);
            }
        }
    }

    public int GetPlayerIndex()
    { 
        return playerIndex; 
    }
}
