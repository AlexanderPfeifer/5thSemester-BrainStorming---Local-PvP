using UnityEngine;

public class AttackOnAnimationEvent : MonoBehaviour
{
   [SerializeField] private AutoAttack autoAttack;

    public void AttackEnemy()
    {
        autoAttack.AttackEnemyAnimationEvent();
    }
}
