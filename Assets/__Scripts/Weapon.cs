using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;


/// <summary>
/// This is an enum of the various possible wepaon types.
/// It also includes a "shield" type to allow a shiewld power up.
/// Items marked [NI] below are not implemented in the IGDPD book.
/// </summary>
public enum WeaponType
{
    none,       //The default / no weapon
    blaster,    //a simple blaster
    spread,     //Three shots simultaneously
    phaser,     //[NI] Shots that move in waves
    missile,    //[NI] Homing missiles
    laser,      //Damage over time
    shield      //Raise shieldLevel
}

/// <summary>
/// The WeaponDefinition class allows you to set the properties
///    of a specific weapon in the Inspector.  The Main class has
///    an array of WeaponDefintions that makes this possible.
/// </summary>
[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter;                       //letter to show on the power up
    public Color color = Color.white;           //Color of Collar & power-up
    public GameObject projectilePrefab;         //Prefab for projectiles
    public GameObject homingProjectilePrefab;
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;               //amount of damage caused
    public float continuousDamage = 0;          //damage per second (Laser)
    public float delayBetweenShots = 0;
    public float velocity = 20;                 //Speed of projectiles
}

public class Weapon : MonoBehaviour
{
    public Weapon w;
    static public Transform PROJECTILE_ANCHOR;

    [Header("Set Dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime; //time last shot was fired
    private Renderer collarRend;
    //public Transform enemyTransform;
    Enemy enemy;
    public GameObject homingProjectilePref;
    
   


    GameObject closestEnemy;
    //List<Enemy> result;
    //public GameObject projectilePrefab;        

    private void Start()
    {
        //result = new List<Enemy>();
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        //Call SetType() for the default _type of WeaponType.none
        SetType(_type);

        //Dynamically create an anchor for all Projectiles
        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        //Find the fireDelegate of the root GameObject
        GameObject rootGO = transform.root.gameObject;
        if (rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
        if (rootGO.GetComponent<Enemy_1>() != null)
        {
            rootGO.GetComponent<Enemy_1>().fireDelegate += Fire;
        }
        if (rootGO.GetComponent<Enemy_2>() != null)
        {
            rootGO.GetComponent<Enemy_2>().fireDelegate += Fire;
        }
        if (rootGO.GetComponent<Enemy_5>() != null)
        {
            rootGO.GetComponent<Enemy_5>().fireDelegate += Fire;
        }
    }

    public WeaponType type
    {
        get { return _type; }
        set { SetType(value); }
    }

    public void SetType(WeaponType wt)
    {
        _type = wt;
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type);
        collarRend.material.color = def.color;
        lastShotTime = 0; //You can fire immediately after _type is set
        
    }

   

    public Enemy FindClosestEnemy()
    {
        Enemy[] en;
        en = FindObjectsOfType<Enemy>();
        Enemy closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (Enemy e in en)
        {
            Vector3 diff = e.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = e;
                distance = curDistance;
            }
        }
        
        return closest;
        
    }

    public void Fire()
    {
        // Playing a different audio for the laser 
        if(type != WeaponType.laser) FindObjectOfType<AudioManager>().Play("Shooting");
        else FindObjectOfType<AudioManager>().Play("Laser");
        //If this.gameObject is inactive, return
        if (!gameObject.activeInHierarchy) return;
        //If it hasn't been enough time between shots, return
        if (Time.time - lastShotTime < def.delayBetweenShots)
        {
            return;
        }
        HomingProjectile hp;
        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;
        if (transform.up.y < 0)
        {
            vel.y = -vel.y;
        }
        switch (type)
        {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                def.damageOnHit = 3;
                break;

            case WeaponType.spread:
                p = MakeProjectile();  //Make middle Projectile
                p.rigid.velocity = vel;
                p = MakeProjectile(); //Make right Projectile
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile(); //Make left Projectile
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                def.damageOnHit = 2;
                break;
            case WeaponType.laser:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;
            case WeaponType.missile:
                def.homingProjectilePrefab = homingProjectilePref;
                hp = MakeHomingProjectile();
                
                
                
                
                break;
        }
    }

    /* Transform GetClosestEnemy(Transform[] enemies)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Transform t in enemies)
        {
            float dist = Vector3.Distance(t.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    } */

    public  Projectile MakeProjectile()
    {    
        GameObject go = Hero._pool.Get();
        if (transform.parent.gameObject.tag == "Hero")
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShotTime = Time.time;
        return p;
        
    }

    public HomingProjectile MakeHomingProjectile()
    {
        
        GameObject homing = Instantiate<GameObject>(homingProjectilePref);
        if (transform.parent.gameObject.tag == "Hero")
        {
            homing.tag = "HomingProjectileHero";
            homing.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            homing.tag = "ProjectileEnemy";
            homing.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        homing.transform.position = collar.transform.position;
        homing.transform.SetParent(PROJECTILE_ANCHOR, true);
        HomingProjectile hp = homing.GetComponent<HomingProjectile>();
        hp.type = type;
        lastShotTime = Time.time;
        return hp;

    }
}