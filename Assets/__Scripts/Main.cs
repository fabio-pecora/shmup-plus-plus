using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{

    static public Main S;
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 1f;
    public float enemyDefaultPadding = 1.5f;
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[] { WeaponType.blaster, WeaponType.blaster, WeaponType.laser,
                                                            WeaponType.spread, WeaponType.shield, WeaponType.missile};
   

    private BoundsCheck bndCheck;

    public void ShipDestroyed(Enemy e)
    {
        //Potentially generate a PowerUp
        if (Random.value <= e.powerUpDropChance)
        {
            //Choose which PowerUp to pick
            //Pick one from the possibilities in powerUpFrequency
            int index = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[index];
            //Spawn a PowerUp
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            //Set it to the proper WeaponType
            pu.SetType(puType);

            //Set it to the position of the destroyed ship
            pu.transform.position = e.transform.position;
        }
    }

    void Awake()
    {
        S = this;                                                                               
        //Set bndCheck to reference the BoundsCheck component on this GameObject
        bndCheck = GetComponent<BoundsCheck>();
        //Invoke SpawnEnemy() once (in 2 seconds, based on default values
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);

        //A generic Dictionary with WeaponType as the key
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
            
        }
    }
                      
    public void SpawnEnemy()
    {
        //Pick a random enemy prefab to instantiate
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        //Position the enemy above the screen with a random x position
        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        //set the initial position for the spawned enemy
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        //Invoke SpawnEnemy() again
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }

    public void DelayedRestart(float delay)
    {
        //Invoke the Restart() in delayed seconds.
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        //Reload _Scene_0 to restart the game
        SceneManager.LoadScene("_Scene_0");
    }

    ///<summary>
    /// Static function that gets a WeaponDefinition from the WEAP_DICT static
    ///     protected field of the Main class
    ///</summary>
    ///<returns> The WeaponDefinition or, if there is no WeaponDefinition with
    /// the WeaponType passed in, returns a new WeaponDefinition with a
    /// WeaponType of none..</returns>
    /// <param name = "wt">The WeaponType of the desired WeaponDefinition</param>
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        //Check to make sure that the key exists in the Dictionary
        //Attempting to retrieve a key that didn't exist would throw an error.
        //so the following if statement is important
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }
        //This returns a new WeaponDefinition with a type of WeaponType.non
        //which means it has failed to find the right WeaponDefinition
        return (new WeaponDefinition());
    }
}
