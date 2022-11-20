using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class ProjectileScript : MonoBehaviour
{

    [SerializeField]
    private Vector3 direction;
    private float damage;
    private float lifetime = 5f;
    private float curTime;

    public void SetDirectionAndDamage(Vector3 incDir, float incDam)
    {
        direction = incDir;
        damage = incDam;
        curTime = 0;
    }

    void FixedUpdate()
    {
        if (Time.deltaTime + curTime > lifetime)
        {
            Destroy(this.gameObject);
        } else {
            curTime += Time.deltaTime;
        }
        transform.Translate(direction);
    }

    private void OnTriggerEnter(Collider c)
    {
        //Debug.Log($"Bullet hit {c.gameObject.tag} for {damage} damage!");
        if (c.gameObject.tag == "PlayerBody" || c.gameObject.tag == "Player")
        {
            if (c.gameObject.TryGetComponent<FirstPersonController>(out FirstPersonController controller))
            {
                controller.ApplyDamageToPlayer(damage); // Damages the player on contact
            }
            Destroy(this.gameObject);
        }

        if (c.gameObject.tag == "Enemy")
        {
            if (c.gameObject.TryGetComponent<EnemyScript>(out EnemyScript eS))
            {
                eS.DamageEnemy((int)damage, false);
            }
            Destroy(this.gameObject);
        }

        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision c)
    {
       //Debug.Log($"Bullet hit {c.gameObject.tag} for {damage} damage!");
        if (c.gameObject.tag == "PlayerBody" || c.gameObject.tag == "Player")
        {
            if (c.gameObject.TryGetComponent<FirstPersonController>(out FirstPersonController controller))
            {
                controller.ApplyDamageToPlayer(damage); // Damages the player on contact
            }
            Destroy(this.gameObject);
        }

        if (c.gameObject.tag == "Enemy")
        {
            if (c.gameObject.TryGetComponent<EnemyScript>(out EnemyScript eS))
            {
                eS.DamageEnemy((int)damage, false);
            }
            Destroy(this.gameObject);
        }

        Destroy(this.gameObject);
    }

}
