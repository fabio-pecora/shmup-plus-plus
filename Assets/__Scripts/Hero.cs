using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UIElements;

public class Hero : MonoBehaviour
{
    public static Hero S;

    [Header("Set in Inspector")]

    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public GameObject homingProjectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;
    public static ObjectPool<GameObject> _pool;
    [SerializeField] private bool _usePool;
    Transform transfor;
    protected BoundsCheck bndCheck;

    [Header("Set Dynamically")]
    [SerializeField]
    public static float _shieldLevel = 1;

    //This variable holds a reference to the last triggering GameObject
    private GameObject lastTriggerGo = null;

    //Declare a new delegate type WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    //Create a WeaponFireDelegate field names fireDelegate
    public WeaponFireDelegate fireDelegate;

    void Start()
    {
        _pool = new ObjectPool<GameObject>(() =>
        {
            return Instantiate(projectilePrefab);
        }, projectile =>
        {
            projectile.gameObject.SetActive(true);
        }, projectile =>
        {
            projectile.gameObject.SetActive(false);
        }, projectile =>
        {
            Destroy(projectile.gameObject);
        }, false, 10, 20);

        if (S == null)
        {
            S = this;
        } else
        {
            Debug.Log("Hero.Awake() - attempted to assign second Hero.S!");
        }
        //fireDelegate += TempFire;
        //Reset the weapons to start _Hero with 1 blaster
        ClearAllWeapons();
        weapons[0].SetType(WeaponType.blaster);
    }

    // Update is called once per frame
    void Update()
    {
        //Pull in information from the Input class
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        //Change transform.position based on the axes
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;
        

        //Rotate the ship to make it feel more dynamic
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        //Allow the ship to Fire
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            TempFire();
        }*/
        //Use the fireDelegate to fire Weapons
        //First, make sure the button is pressed: Axis("Jump")
        //Then ensure that fireDelegate isn't null to avoid an error
        
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {
            fireDelegate();
        }
    }

    void TempFire()
    {
        GameObject projectGo = _usePool ? _pool.Get() : Instantiate(projectilePrefab);
        projectGo.transform.position = transform.position;
        Rigidbody rb = projectGo.GetComponent<Rigidbody>();
        //rb.velocity = Vector3.up * projectileSpeed;
        Projectile proj = projectGo.GetComponent<Projectile>();
        proj.type = WeaponType.blaster;
        float tSpeed = Main.GetWeaponDefinition(proj.type).velocity*-1;
        rb.velocity = Vector3.up * tSpeed;
    }

    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        //print("Triggered: " + go.name);

        //Make sure it's not the same triggering go as last time
        if (go == lastTriggerGo)
        {
            return;
        }
        lastTriggerGo = go;
        if (go.tag == "Enemy")
        {
            FindObjectOfType<AudioManager>().Play("LowerShield");
            shieldLevel--;
            Destroy(go);
            if (shieldLevel <= 0) shieldLevel = 0;
        }

        else if (go.tag == "PowerUp")
        {
            //If the shield was triggered by a PowerUp
            AbsorbPowerUp(go);
        }
        else
        {
            print("Triggered by non-enemy: " + go.name);
        }
    }
    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGo = coll.gameObject;
        Hero._pool.Release(otherGo);
        shieldLevel--;
        if (shieldLevel <= 0) shieldLevel = 0;
    }

        public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield:
                shieldLevel++;
                break;
            default:
                if (pu.type == weapons[0].type) //if it is the same type
                {
                    Weapon w = GetEmptyWeaponSlot();
                    if (w != null)
                    {
                        //Set it to pu.type
                        w.SetType(pu.type);
                    }
                }
                else //If this is a different weapon type
                {
                    ClearAllWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        
        }
        pu.AbsorbedBy(this.gameObject);
    }

    public float shieldLevel
    {
        get {

            return _shieldLevel;
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            //If the shield is going to be set to less than zero
            if (value < 0)
            {
                Destroy(this.gameObject);
                //Tell Main.S to restart the game after a delay
                Main.S.DelayedRestart(gameRestartDelay);
                FindObjectOfType<AudioManager>().Play("HeroDeath");
            }
        }
    }

    Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type == WeaponType.none)
            {
                return weapons[i];
            }
        }
        return null;
    }

    void ClearAllWeapons()
    {
        foreach(Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }

}
 