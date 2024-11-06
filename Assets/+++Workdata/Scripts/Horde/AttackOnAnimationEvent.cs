using UnityEngine;

public class AttackOnAnimationEvent : MonoBehaviour
{
    [SerializeField] private ZombieAutoAttack zombieAutoAttack;

    public void AttackEnemy()
    {
        zombieAutoAttack.AttackEnemyAnimationEvent();
    }
}
