using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_5 : Enemy
{
    public GameObject projectilePrefab;
    public Weapon weapon;
    public float lastShotTime; 
    //Declare a new delegate type WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    //Create a WeaponFireDelegate field names fireDelegate
    public WeaponFireDelegate fireDelegate;
    public static float secondsLeft;
    float x, y;

    // Start is called before the first frame update
    public override void Start()
    {
        speed = 5f;
        weapon.SetType(WeaponType.laser);
        x = pos.x;
        y = pos.y;
        secondsLeft = 2;
    }

    // Update is called once per frame
    public override void Update()
    {
        fireDelegate();
        base.Update();
        secondsLeft -= Time.deltaTime;
        if (secondsLeft <= 0)
        {
            x = Random.Range(-60f, 60f);
            y = Random.Range(0f, 50f);
            this.transform.position = new Vector3(x, y, 0);
            secondsLeft = 2;
        }
        Move();
    }

    public override void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }
}

