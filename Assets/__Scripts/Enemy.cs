using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Enemy : MonoBehaviour
{
    public Enemy e;
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;
    public float fireRate = .3f;
    public float health = 10;
    public static int score = 0;
    public static int highscore;
    public float showDamageDuration = 0.1f; //# seconds to show damage
    public float powerUpDropChance = 1f;  //Chance to drop a PowerUp
    public float projectileSpeed = 40;

    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials; //All the Materials of this & its children
    public bool showingDamage = false;
    public float damageDoneTime; //Time to stop showing damage
    public bool notifiedOfDestruction = false; //Will be used later
    public static Vector3 enemyPosition;


    protected BoundsCheck bndCheck;

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        //Get materials and colors for this Gameobject and its children
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for(int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    public virtual void Start()
    {
        
    }

    //This is a Property: A method that acts like a field
    public Vector3 pos
    {
        get
        {
            return (this.transform.position);
        }
        set
        {
            this.transform.position = value;
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
       
        Move();

        if (showingDamage && Time.time > damageDoneTime)
        {
            FindObjectOfType<AudioManager>().Play("Damage");
            UnShowDamage();
        }

        if (bndCheck != null && bndCheck.offDown)
        {
            //We're off the bottom, so destroy this GameObject
            Destroy(gameObject);
        }
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGo = coll.gameObject;
        
        switch (otherGo.tag)
        {
            case "ProjectileHero":
                
                Projectile p = otherGo.GetComponent<Projectile>();
                //If this Enemu is off screen, don't damage it
                if (!bndCheck.isOnScreen)
                {
                    Hero._pool.Release(otherGo);
                    break;
                }
                //Hurt this enemy
                ShowDamage();
                //Get the damage amount from the Main WEAP_DICT
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if (health <= 0)
                {
                    //Tell the Main singleton that this ship was destroyed
                    if (!notifiedOfDestruction)
                    {
                        score += 50;
                        Main.S.ShipDestroyed(this);
                    }
                    notifiedOfDestruction = true;
                    //Destroy this Enemy
                    Destroy(this.gameObject);
                }
                Hero._pool.Release(otherGo);
                break;
            case "HomingProjectileHero":
                
                GameObject hp = otherGo;
                HomingProjectile homProj = hp.transform.parent.GetComponent<HomingProjectile>();
                Destroy(hp.transform.parent.gameObject);
                hp = null;
                //Hurt this enemy
                ShowDamage();
                health -= Main.GetWeaponDefinition(homProj.type).damageOnHit;
                if (health <= 0)
                {
                    //Tell the Main singleton that this ship was destroyed
                    if (!notifiedOfDestruction)
                    {
                        score += 50;
                        Main.S.ShipDestroyed(this);
                    }
                    notifiedOfDestruction = true;
                    //Destroy this Enemy
                    Destroy(this.gameObject);
                }
                
                
                Destroy(otherGo);
                break;

            default:
                print("Enemy hit by non-ProjectileHero: " + otherGo.name);
                break;
        }
    }

    

    void ShowDamage()
    {
        foreach(Material m in materials)
        {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    void UnShowDamage()
    {
        for(int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }

}
