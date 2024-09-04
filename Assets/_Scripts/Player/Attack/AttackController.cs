using UnityEngine;

// To implement new weapon:
//   Create (or use existing) weapon script class
//   Add weapon script class to prefab
//   Add prefab to Manager (Monobehaviour).PlayerDataSingleton field 
//   Set dict import in PlayerDataSingleton.cs
//   Add Attack, EndAttack, EndReload events in corresponding animations
//   Set weapon tag
public abstract class AttackController : MonoBehaviour {
    protected readonly int attackAnimationParameter = Animator.StringToHash("Attack");
    protected readonly int reloadAnimationParameter = Animator.StringToHash("Reload");
    protected readonly int enemyLayerMask = 1 << 6; // Enemy layer == 6
    protected bool inAnimation = false;
    protected TMPro.TMP_Text ammoText;
    protected Camera mainCamera;
    protected int ammo;

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

    protected RaycastHit hitEnemy;
    protected Enemy enemy;
    /// <summary>
    /// Called from animation when damage should actually be dealt
    /// </summary>
    protected abstract void Attack();
    
    /// <summary>
    /// Called from animation
    /// </summary>
    public virtual void EndAttack() {
        ammoText.SetText(ammo.ToString());
        inAnimation = false;
    }
    /// <summary>
    /// Called from animation
    /// </summary>
    public virtual void EndReload() {
        ammo = MaxAmmo;
        ammoText.SetText(ammo.ToString());
        inAnimation = false;
    }

}