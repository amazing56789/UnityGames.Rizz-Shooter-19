using UnityEngine;

public sealed class PlayerDataSingleton : MonoBehaviour {
    private static PlayerDataSingleton instance;
    public static PlayerDataSingleton Instance {
        get {
            if (instance != null) return instance;
            instance = GameObject.FindAnyObjectByType<PlayerDataSingleton>();
            return instance;
        }
        private set => instance = value;
    }

    readonly static System.Collections.Generic.Dictionary<Weapon, float> damages = new() {
        [Weapon.NONE] = 0.0f,
        [Weapon.FIST_WEAK] = 1.0f,
        [Weapon.FIST] = 3.0f,
        [Weapon.KNIFE] = 6.0f,
        [Weapon.GG_GLOCK] = 12.0f,
        [Weapon.MMMM_GUN] = 12.0f
    };

    [SerializeField] private bool initiallyInvincible = false;
    [SerializeField] private float initialHealth = 5.0f;
    [SerializeField] private Weapon initialWeapon = Weapon.NONE;
    [SerializeField] private GameObject
        noneWeaponPrefab,
        weakArmPrefab,
        armPrefab,
        knifePrefab,
        glockPrefab,
        gunPrefab;
    private readonly System.Collections.Generic.Dictionary<Weapon, GameObject> WeaponPrefabs = new();

    public bool trackedByEnemies = true;
    public float Health {get; private set;}
    public Weapon HeldWeapon {get; private set;}
    [System.NonSerialized] public bool Invincible;
    private GameObject playerObject;
    public GameObject PlayerObject { // [This and WeaponObject] For outside use (think static methods)
        get {
            if (playerObject != null) return playerObject;
            playerObject = GameObject.FindWithTag("Player");
            return playerObject;
        }
    }
    private GameObject weaponObject;
    public GameObject WeaponObject {
        get {
            if (weaponObject != null) return weaponObject;
            weaponObject = GameObject.FindWithTag("PlayerWeapon");
            return weaponObject;
        }
    }
    private Animator weaponAnimator;
    /// <summary>
    /// Beware of animating when HeldWeapon is Weapon.None
    /// </summary>
    public Animator WeaponAnimator {
        get {
            if (weaponAnimator != null) return weaponAnimator;
            weaponAnimator = WeaponObject.GetComponent<Animator>();
            return weaponAnimator;
        }
        private set => weaponAnimator = value;
    }
    public float Damage => damages[HeldWeapon];

    public enum Weapon {
        NONE,
        FIST_WEAK,
        FIST,
        KNIFE,
        GG_GLOCK,
        MMMM_GUN
    }

    void Awake() {
        if (Instance != this) Destroy(this);

        InitializeWeaponDict();
        SwitchWeapon(initialWeapon);
        selectedWeapon = (int) initialWeapon;
        Invincible = initiallyInvincible;
        Health = initialHealth;
        print("ohio sizz");
    }
    void Kill() {

    }
    void InitializeWeaponDict() {
        WeaponPrefabs[Weapon.NONE] = noneWeaponPrefab;
        WeaponPrefabs[Weapon.FIST_WEAK] = weakArmPrefab;
        WeaponPrefabs[Weapon.FIST] = armPrefab;
        // #TODO: Implement
        // WeaponPrefabs[Weapon.KNIFE] = knifePrefab;
        // WeaponPrefabs[Weapon.GG_GLOCK] = glockPrefab;
        // WeaponPrefabs[Weapon.MMMM_GUN] = gunPrefab;
    }

    public void Hurt(float dmg) {
        if (!Invincible) Health -= dmg;
        if (Health <= 0.0f) Kill();
    }
    public void SwitchWeapon(Weapon newWeapon) {
        if (newWeapon == HeldWeapon) return;

        HeldWeapon = newWeapon;

        GameObject.Destroy(WeaponObject);
        GameObject.Instantiate(WeaponPrefabs[newWeapon], PlayerObject.transform, false);
    }

    // #Debug
    readonly string[] weaponsList = {"None", "Weak Fist", "Fist", "Knife", "Glock", "Gun"};
    int selectedWeapon;
    void OnGUI() {
        selectedWeapon = GUI.Toolbar(new Rect(0, 0, 400, 20), selectedWeapon, weaponsList);
        SwitchWeapon((Weapon) selectedWeapon);
    }
}