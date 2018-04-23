using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour {

    [SerializeField] float explosionRange;
    [SerializeField] float explosionDuration;
    [SerializeField] GameObject bullet;
    [SerializeField] float bulletSpeed = 20f;
    [SerializeField] int bulletNumber;
    [SerializeField] BulletType bulletType;
    [SerializeField] float dmg = 100f;
    [SerializeField] MinMax shootInterval = new MinMax(0, 0.2f);

    public enum BulletType
    {
        Random,

        Direct,

        Explode,

        Chase,
    }

    public void Start()
    {
        if (bulletType == BulletType.Random)
            StartCoroutine(ShootRandom());
        else if (bulletType == BulletType.Direct)
            StartCoroutine(ShpotDir());
        else if (bulletType == BulletType.Explode)
            StartCoroutine(ShpotExplode());
        else if (bulletType == BulletType.Chase)
            StartCoroutine(ShotChase());
    }



    IEnumerator ShotChase()
    {
        yield return new WaitForSeconds(shootInterval.Rand);

        for (int i = 0; i < bulletNumber; ++i)
        {
            Vector3 dir = Random.insideUnitCircle.normalized;
            var target = GameController.Instance.GetRandomEnermy();
            if (target != null)
            {
                ShootChaseBullet(dir, target.transform);
            }else
            {
                ShootBullet(dir);
            }

            
            yield return new WaitForSeconds(shootInterval.Rand);
        }
    }
    IEnumerator ShpotExplode()
    {

        for (int i = 0; i < bulletNumber; ++i)
        {
            ExplandBullet(explosionDuration);
            yield return new WaitForSeconds(shootInterval.Rand);
        }
    }

    IEnumerator ShpotDir()
    {
        var playerDir = MPlayer.Instance.Velocity.normalized;

        if (playerDir.magnitude < 0.1f)
            playerDir = Vector3.up;

        for (int i = 0; i < bulletNumber; ++i)
        {
            var dir = playerDir.normalized + (Vector3)Random.insideUnitCircle.normalized * 0.3f;

            ShootBullet(dir);
            yield return new WaitForSeconds(shootInterval.Rand);
        }
    }

    IEnumerator ShootRandom()
    {
        for ( int i = 0; i < bulletNumber; ++ i )
        {
            ShootBullet(Random.insideUnitCircle.normalized );
            yield return new WaitForSeconds(shootInterval.Rand);
        }
    }

    public void ShootChaseBullet(Vector3 dir , Transform target)
    {
        var b = Instantiate(bullet) as GameObject;
        var com = b.GetComponent<Bullet>();
        com.Init( target , dir.normalized * bulletSpeed, dmg, transform.position, null);

    }

    public void ShootBullet( Vector3 dir )
    {
        var b = Instantiate(bullet) as GameObject;
        var com = b.GetComponent<Bullet>();
        com.Init(dir.normalized * bulletSpeed, dmg, transform.position, null);

    }

    public void ExplandBullet(float duration)
    {

        var b = Instantiate(bullet) as GameObject;
        var com = b.GetComponent<Bullet>();
        com.Init(explosionDuration, dmg, transform.position + (Vector3)Random.insideUnitCircle * explosionRange, null);

    }


}
