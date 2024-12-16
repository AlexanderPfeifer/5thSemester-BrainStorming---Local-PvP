using UnityEngine;

public class ResurrectZombieHorde : MonoBehaviour
{
    private ZombiePlayerHordeRegistry zombiePlayerHordeRegistry;

    private void Start()
    {
        GetComponent<ZombiePlayerHordeRegistry>();
    }

    public void ResurrectNumberOfZombies(int numberOfZombies, GameObject zombie)
    {
        for (int i = 0; i < numberOfZombies; i++)
        {
            Instantiate(zombie);

            //zombie.GetComponent<CachedZombieData>().ZombieMovement.TargetGroup
        }
    }
}
