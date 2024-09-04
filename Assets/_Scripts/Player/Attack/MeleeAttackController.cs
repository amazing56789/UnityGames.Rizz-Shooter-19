using UnityEngine;

public abstract class MeleeAttackController : AttackController {
    protected readonly Vector3 attackViewportRayOrigin = new(0.5f, 0.5f);

    protected override int MaxAmmo => 6;

    protected abstract float MeleeRange {get;}

    protected override void Attack() {
        inAnimation = true;
        if (Physics.Raycast(
                mainCamera.ViewportPointToRay(attackViewportRayOrigin), out hitEnemy, MeleeRange, enemyLayerMask
            )
        ) hitEnemy.collider.GetComponent<Enemy>().Hurt(PlayerDataSingleton.Instance.Damage); 
    }
}