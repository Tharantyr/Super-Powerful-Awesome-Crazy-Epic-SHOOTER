using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerEnemy : Enemy
{
    Coroutine c;

    protected override void Awake()
    {
        base.Awake();
        explosion = "ChargerEnemyExplosion";
    }

    // Start is called before the first frame update
    protected override void OnEnable()
    {
        c = null;
        base.OnEnable();
        hp = 50;
    }

    protected override void Spawn()
    {
        base.Spawn();

        Vector3 direction = GameSystem.instance.player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // Face player when spawning

        transform.position = ChooseLocation();
        float distanceToPlayer = Vector2.Distance(transform.position, GameSystem.instance.player.transform.position);

        while (distanceToPlayer < 7) // Make sure enemy always spawns at least 7 units away from player
        {
            transform.position = ChooseLocation();
            distanceToPlayer = Vector2.Distance(transform.position, GameSystem.instance.player.transform.position);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (CrossedBound(Bounds.LeftBound)) // If crossing left boundary, stop and push back
        {
            body.velocity = new Vector2(0, body.velocity.y);
            body.AddForce(new Vector2(5, 0));
        }

        if (CrossedBound(Bounds.TopBound))
        {
            body.velocity = new Vector2(body.velocity.x, 0);
            body.AddForce(new Vector2(0, -5));
        }

        if (CrossedBound(Bounds.BottomBound))
        {
            body.velocity = new Vector2(body.velocity.x, 0);
            body.AddForce(new Vector2(0, 5));
        }

        if (CrossedBound(Bounds.RightBound))
        {
            body.velocity = new Vector2(0, body.velocity.y);
            body.AddForce(new Vector2(-5, 0));
        }
    }

    protected override void AI() // Program charger enemy AI
    {
        if (GameSystem.instance.player != null) // Make sure there's still something left to kill
        {
            // Orientate towards player
            Vector3 direction = GameSystem.instance.player.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle - 90)), 5 * Time.deltaTime);

            if (c == null)
                c = StartCoroutine(Charge());
        }
    }

    private IEnumerator Charge()
    {
        yield return StartCoroutine(FadeToColor(Color.red, 1f));
        render.material.color = Color.white;
        Vector3 direction = GameSystem.instance.player.transform.position - transform.position;
        body.AddForce(direction.normalized * 1000);
        yield return new WaitForSeconds(2);
        c = null;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Player player = col.gameObject.GetComponent<Player>();
            player.body.AddForce((col.collider.ClosestPoint(transform.position) - (Vector2)transform.position).normalized * 300); // Use amazing linear algebra to push object away from asteroid
            GameSystem.instance.soundManager.PlayOneShot(GameSystem.instance.PlayerSounds[8]); // Play sound effect
            player.DecreaseHealthShield(40);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        GameSystem.instance.soundManager.PlayOneShot(GameSystem.instance.EnemySounds[Random.Range(1, 3)]); // Play sound effect

        GameObject obj = ObjectPool.instance.GetPooledObject(explosion); // Spawn explosion
        obj.transform.localScale = new Vector3(0.05f, 0.05f, 1);
        obj.transform.GetChild(0).localScale = new Vector3(0.05f, 0.05f, 1);
        obj.transform.GetChild(1).localScale = new Vector3(0.05f, 0.05f, 1);
        obj.transform.GetChild(2).localScale = new Vector3(0.05f, 0.05f, 1);
        obj.transform.position = GetComponent<Collider2D>().ClosestPoint(col.transform.position);
        obj.transform.rotation = transform.rotation;

        if (gameObject.activeInHierarchy)
            StartCoroutine(Flash(Color.red, 0.075f)); // Flash red

        lastHit = col.gameObject.tag; // Store who gave the last hit so we can update kill counter

        // If hit by bullet
        if (col.tag == "Bullet")
        {
            col.gameObject.SetActive(false); // Destroy bullet
            hp -= 10;
            GameSystem.instance.Score += 5; // Award points for decent aim
        }

        if (col.tag == "Rocket") // If rocket collision, detonate rocket
        {
            GameObject rocket = ObjectPool.instance.GetPooledObject("RocketExplosion"); // Create rocket explosion
            rocket.transform.position = GetComponent<Collider2D>().ClosestPoint(col.transform.position);
            rocket.transform.rotation = transform.rotation;
            rocket.GetComponent<Collider2D>().enabled = true;
            tempRocket = col.gameObject;
            tempRocket.SetActive(false);
        }

        if (col.tag == "RocketExplosion") // If hit by rocket explosion
        {
            GameSystem.instance.soundManager.PlayOneShot(GameSystem.instance.PlayerSounds[7]); // Play sound effect
            hp -= 30;
            GameSystem.instance.Score += 15;
        }
    }

    protected override void Death()
    {
        GameSystem.instance.explosionManager.PlayOneShot(GameSystem.instance.EnemySounds[0]);

        GameSystem.instance.enemyList.Remove(gameObject);

        GameObject obj = ObjectPool.instance.GetPooledObject("Orb"); // Spawn orb
        obj.transform.position = transform.position - new Vector3(0.25f, 0);
        obj.transform.rotation = transform.rotation;

        obj = ObjectPool.instance.GetPooledObject("Orb"); // Spawn orb
        obj.transform.position = transform.position + new Vector3(0.25f, 0);
        obj.transform.rotation = transform.rotation;

        obj = ObjectPool.instance.GetPooledObject(explosion); // Spawn explosion
        obj.transform.localScale = explosionSizeVec;
        obj.transform.GetChild(0).localScale = explosionSizeVec;
        obj.transform.GetChild(1).localScale = explosionSizeVec;
        obj.transform.GetChild(2).localScale = explosionSizeVec;
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
        gameObject.SetActive(false);

        if (lastHit == "Bullet" || lastHit == "RocketExplosion")
        {
            GameSystem.instance.Kills++;
            GameSystem.instance.Score += 120;
        }
    }

    public override void WaveDeath()
    {
        GameSystem.instance.explosionManager.PlayOneShot(GameSystem.instance.EnemySounds[0]);
        base.WaveDeath();
    }
}
