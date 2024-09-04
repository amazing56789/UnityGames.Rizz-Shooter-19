using UnityEngine;

public abstract class Enemy : MonoBehaviour {
    public abstract float Health {get; protected set;}
    
    public bool invincible = false;

    public virtual void Hurt(float dmg) {
        if (!invincible) Health -= dmg;
        if (Health <= 0.0f) Kill();
    }
    public virtual void Kill(PlayerDataSingleton.Weapon weapon = PlayerDataSingleton.Weapon.NONE) {
        Destroy(gameObject);
    }
}