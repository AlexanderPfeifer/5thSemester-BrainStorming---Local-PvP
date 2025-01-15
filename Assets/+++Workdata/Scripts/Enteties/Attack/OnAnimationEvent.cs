using UnityEngine;
using UnityEngine.VFX;

public class OnAnimationEvent : MonoBehaviour
{
   [SerializeField] private AutoAttack autoAttack;
   [SerializeField] private VisualEffect moveEffect;

    public void AttackEnemy()
    {
        autoAttack.AttackEnemyAnimationEvent();
    }

    public void PlayMoveParticle()
    {
        moveEffect.Play();
    }
}
