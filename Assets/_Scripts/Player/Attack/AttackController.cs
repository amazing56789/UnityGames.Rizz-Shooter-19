using UnityEngine;

public abstract class AttackController : MonoBehaviour {
    protected readonly int attackAnimationParameter = Animator.StringToHash("Attack");
    protected readonly int reloadAnimationParameter = Animator.StringToHash("Reload");
    protected readonly int enemyLayerMask = 1 << 6; // Enemy layer == 6
    protected bool inAnimation = false;
    protected TMPro.TMP_Text ammoText;
    protected Camera mainCamera;
    protected int ammo;

    // 
    public bool freeze = false;

    protected abstract int MaxAmmo {get;}

    protected virtual void Start() {
        ammoText = GameObject.FindWithTag("Ammo text").GetComponent<TMPro.TMP_Text>();
        mainCamera = Camera.main;
        
        ammo = MaxAmmo;
        ammoText.SetText(ammo.ToString());
    }
    protected virtual void Update() {
        if (inAnimation || freeze) return;
        if (Input.GetMouseButton(0)) {
            if (ammo-- > 0) BeginAttack();
            else Reload();
        }
        if (Input.GetKey(KeyCode.R)) Reload();
    }
    protected virtual void Reload() {
        inAnimation = true;
        PlayerDataSingleton.Instance.WeaponAnimator.SetTrigger(reloadAnimationParameter);
    }
    protected virtual void BeginAttack() {
        inAnimation = true;
        PlayerDataSingleton.Instance.WeaponAnimator.SetTrigger(attackAnimationParameter);
    }

    // All of these to be called in animation events
    protected RaycastHit hitEnemy;
    protected Enemy enemy;
    /// <summary>
    /// Called when damage should actually be dealt
    /// </summary>
    protected abstract void Attack();
    
    public virtual void EndAttack() {
        ammoText.SetText(ammo.ToString());
        inAnimation = false;
    }
    public virtual void EndReload() {
        ammo = MaxAmmo;
        ammoText.SetText(ammo.ToString());
        inAnimation = false;
    }

}