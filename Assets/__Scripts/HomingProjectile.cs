using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingProjectile : MonoBehaviour
{
    public float speed;
    public float rotateSpeed = 200f;
    public Transform target;
    public Rigidbody2D rb;
    private WeaponType _type;
    Enemy enemy;
    private Renderer rend;
    private BoundsCheck bndCheck;

    //This public property masks the field _type and takes action when it is set
    public WeaponType type
    {
        get
        {
            return _type;
        }
        set
        {
            SetType(value);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {

        speed = 30;
    }

    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        //print("Triggered: " + go.name);

        Destroy(go);
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        enemy = FindClosestEnemy();
        // I want the projectile to follow the first child of the enemy because usually the first child is the most inner 
        // part of the enemy (the part in the middlte) and it looks better rather than just make it follow the whole enemy
        if (enemy != null) target = enemy.transform.GetChild(0).gameObject.transform;

        Vector2 direction = (Vector2)target.position - rb.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.velocity = transform.up * speed;
        
        /*if (enemy != null)
        {
            target = enemy.transform;
            rigid.velocity = transform.forward * speed;
            var rocketTargetRotation = Quaternion.LookRotation(target.position - transform.position);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rocketTargetRotation, rotateSpeed));
        }*/
        
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

    public void SetType(WeaponType eType)
    {
        //Set the _type
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
    }

}
