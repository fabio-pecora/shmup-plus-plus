using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    private BoundsCheck bndCheck;
    private Renderer rend;
    Enemy enemy;

    [Header("Set Dynamically")]
    public Rigidbody rigid;
    [SerializeField]
    private WeaponType _type;
    public Transform target;
    private Rigidbody rb;
    new private Transform transform;

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

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
     

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

    // Update is called once per frame
    void Update()
    {
       
        if (bndCheck.offUp)
        {
            Hero._pool.Release(this.gameObject);
        }        
       
    }

    ///<summary>
    ///Sets the _type private field and colors this projectile to match the
    ///   WeaponDefinition.
    ///   </summary>
    ///   <param name = "eType">The WeaponType to use.</param>
    public void SetType(WeaponType eType)
    {
        //Set the _type
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        rend.material.color = def.projectileColor;

    }

}
