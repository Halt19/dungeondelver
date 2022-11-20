using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine;
using StarterAssets;

public class EnemyScript : MonoBehaviour
{
    [SerializeField]
    private int health = 100;

    private NavMeshAgent agent;
    private GameObject playerOBJ;
    public float aggroRange = 50;
    public float damage = 6;
    public float cdFrames = 10;
    private bool cd;
    public Slider healthBar;
    public int points = 2500;
    public bool Grounded;
    public LayerMask GroundLayers;
    private bool wasDamaged;
    public bool isAlerted;
    [SerializeField]
    private bool hasGun;
    [SerializeField]
    private GameObject bulletPrefab;
    private float shotCooldown = 3f;
    private float maxShotCD;
    [SerializeField]
    private Transform bulletSpawnPoint;
    [SerializeField]
    private float gunStoppingDistance = 20f;
    private GameObject playerTargetLocation;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        wasDamaged = false;
        isAlerted = false;
        if (playerOBJ == null)
        {
            playerOBJ = GameObject.FindGameObjectWithTag("Player");
        }
        if (playerTargetLocation == null)
        {
            playerTargetLocation = GameObject.FindGameObjectWithTag("EnemyTargetLocation");
        }
        healthBar.maxValue = health;
        maxShotCD = shotCooldown;
        shotCooldown = 0;
        if (hasGun)
        {
            agent.stoppingDistance = gunStoppingDistance;
        } else
        {
            agent.stoppingDistance = 0f;
        }
    }

    public void HasGun(bool inc)
    {
        hasGun = inc;
    }

    /*
    Checks to see if the enemy is on the ground or not.
    */
	private void GroundedCheck()
	{
		// set sphere position, with offset
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - 0.14f, transform.position.z);
		Grounded = Physics.CheckSphere(spherePosition, 0.5f, GroundLayers, QueryTriggerInteraction.Ignore);
	} // GroundedCheck

    /*
    Sets the internal reference to the player object.
    */
    public void SetPlayer(GameObject inc)
    {
        playerOBJ = inc;
    } // SetPlayer

    // Update is called once per frame
    void Update()
    {
        if (isAlerted || wasDamaged || Vector3.Distance(playerOBJ.transform.position, this.gameObject.transform.position) < aggroRange)
        {
            agent.SetDestination(playerOBJ.transform.position);
            if (playerTargetLocation != null)
            {
                transform.LookAt(playerTargetLocation.transform);
            }
            if (!wasDamaged && agent.remainingDistance > aggroRange)
            {
                agent.SetDestination(this.gameObject.transform.position);
                isAlerted = false;
            } else if (hasGun)
            {
                RaycastHit hit;
                Ray outRay = new Ray(transform.position, transform.forward);
                Debug.DrawRay(transform.position, transform.forward * 50, Color.green, 1, false);
                if (Physics.Raycast(outRay, out hit, 100))
                {
                 //   Debug.Log(hit.collider.tag);
                    if (hit.collider.tag == "PlayerBody" && shotCooldown <= 0)
                    {
                        FireGun();
                        shotCooldown = maxShotCD;
                    } 

                    {
                        shotCooldown -= Time.deltaTime;
                        if (shotCooldown <= 0) shotCooldown = 0;
                    }
                }
            }
        }
        GroundedCheck();
        if (!Grounded)
        {
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - 0.25f, this.transform.localPosition.z);
        } else
        {
        }
    }
    
    public bool DoesHaveGun()
    {
        return hasGun;
    }

    public void FireGun()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.transform);
        bullet.transform.SetParent(null);
        bullet.GetComponent<ProjectileScript>().SetDirectionAndDamage(Vector3.forward, damage*2.4f);
       // Debug.Log($"Spawned bullet: {this.transform.localEulerAngles}");
    }

    /*
    Damages the enemy, updates their health bar, and kills them if need be.
    */
    public void DamageEnemy(int inc)
    {
        wasDamaged = true;
        health -= inc;
        healthBar.value = health;
        if (health <= 0)
        {
            KillEnemy();
        }
    } // DamageEnemy

    public void DamageEnemy(int inc, bool byPlayer) // doesn't aggro enemy if shot by friendly fire
    {
        if (!wasDamaged) wasDamaged = byPlayer;
        health -= inc;
        healthBar.value = health;
        if (health <= 0)
        {
            KillEnemy();
        }
    } // DamageEnemy
    
    /*
    Kills the enemy, adding points to the player.
    */
    public void KillEnemy()
    {
        playerOBJ.GetComponent<FirstPersonController>().AddPointsToPlayer(points);
        Destroy(this.gameObject);
    } // KillEnemy

    private void OnTriggerStay(Collider c)
    {
        if (c.gameObject.tag == "PlayerBody" || c.gameObject.tag == "Player")
        {
            if (!cd && c.gameObject.TryGetComponent<FirstPersonController>(out FirstPersonController controller))
            {
                controller.ApplyDamageToPlayer(damage); // Damages the player on contact
                StartCoroutine(AttackCooldownTiming()); // Sets cooldown for cd to be false again
            }
        }
    }

    /*
    Sets the initial health and damage values for this enemy.
    */
    public void SetHealthAndDamage(int incHealth, int incDamage)
    {
        health = incHealth;
        damage = incDamage;
        healthBar.maxValue = health;
        healthBar.value = health;
    } // SetHealthAndDamage

    /*
    Cooldown time between attacks (so player doesn't lose all health in one second of continuous contact).
    */
    private IEnumerator AttackCooldownTiming()
    {
        cd = true;
        for (int i=0; i<cdFrames; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        cd = false;
    } // AttackCooldownTiming
}
