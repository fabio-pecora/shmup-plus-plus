using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy_1 : Enemy
{
    [Header("Set in Inspector: Enemy 1")]
    //# seconds for a full sine wave
    public float waveFrequency = 2;
    //sine wave width in meters
    public float waveWidth = 4;
    public float waveRotY = 45;
    public GameObject projectilePrefab;
    public Weapon weapon;
    public float lastShotTime; //time last shot was fired
    //public static ObjectPool<GameObject> _pool;
    //[SerializeField] private bool _usePool;

    //Declare a new delegate type WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    //Create a WeaponFireDelegate field names fireDelegate
    public WeaponFireDelegate fireDelegate;


    private float x0; //the initial x vlaue of pos
    private float birthTime;

    // Start works well because it's not used by the Enemy superclass
    public override void Start()
    {
        /*_pool = new ObjectPool<GameObject>(() =>
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
        }, false, 10, 20);*/
        //Set x0 is the initial x position of Enemy_1
        weapon.SetType(WeaponType.blaster);

        x0 = pos.x;

        birthTime = Time.time;
    }

    //Override the Move function on Enemy
    public override void Move()
    {
        //Because pos is a property, you can't directly set pos.x
        // so get the pos as an editable Vector3
        Vector3 tempPos = pos;
        //theta adjusts based on time
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        //rotate a bit about y
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        //base.Move() still handles the movement down in y
        base.Move();

        //print(bndCheck.isOnScreen);
    }

    

    // Update is called once per frame
    public override void Update()
    {
        
        fireDelegate();
      
        base.Update();
        /*if (bndCheck != null && bndCheck.offDown)
        {
            //We're off the bottom, so destroy this GameObject
            Destroy(gameObject);
        }*/
    }

}
