using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour {
	[SerializeField][ReadOnlyAttribute] Vector3 velocity;
	[SerializeField][ReadOnlyAttribute] float damage;
	[SerializeField][ReadOnlyAttribute] Vector3 position;
	[SerializeField][ReadOnlyAttribute] Enermy parent;
    [SerializeField] [ReadOnlyAttribute] Transform target;
    [SerializeField] [ReadOnlyAttribute] float speed;
    bool isHit = false;
    public void Init(Transform t, Vector3 vel, float dmg, Vector3 pos, Enermy par)
    {
        target = t;

        velocity = vel;
        position = pos;
        parent = par;
        damage = dmg;
        speed = vel.magnitude;
    }


    public void Init( float explosionDuration , float dmg , Vector3 pos , Enermy par )
    {
        velocity = Vector3.zero;
        position = pos;
        parent = par;
        damage = dmg;

        var col = GetComponent<CircleCollider2D>();

        if ( col != null)
        {
            float radius = col.radius;
            col.radius = radius * 0.5f ;
            DOTween.To(() => col.radius, (x) => col.radius = x, radius, explosionDuration).OnComplete(delegate () {
                col.enabled = false;
                Destroy(gameObject, 10f); });
        }

        transform.position = position;
    }
	public void Init( Vector3 vel , float dmg , Vector3 pos , Enermy par)
	{
		velocity = vel;
		damage = dmg;
		position = pos;
		parent = par;

		transform.position = position;
	}

	public void Update()
	{
        if (!isHit)
        {
            if (target != null)
            {
                
                Vector3 toTarget = target.position - position;
                velocity += speed * 5f * Time.deltaTime * toTarget.normalized;
                velocity = Vector3.ClampMagnitude(velocity, speed);
                // velocity = Vector3.Lerp(velocity, toTarget * velocity.magnitude , 2f * Time.deltaTime ).normalized * velocity.magnitude;
                //velocity = (target.position - position).normalized * velocity.magnitude;
            }

            position += velocity * Time.deltaTime;
            transform.position = position;

            if (Global.CheckInFrame(position, ref velocity))
            {
                transform.DOScale(0, 2f).OnComplete(delegate
                {
                    Destroy(gameObject);
                });
            }
        }
	}

	public void OnTriggerEnter2D( Collider2D col )
	{
		if (col.tag == "Player" && parent != null )
        {
            MPlayer.Instance.Attack(parent, damage);

            SelfDestory();

        } else if ( col.tag == "Enermy" && parent == null )
        {
            var ene = col.GetComponent<Enermy>();
            if (ene != null) {
                ene.Attack(damage);
                SelfDestory();
             }
        }
	}

    public void SelfDestory()
    {
        isHit = true;

        var ps = GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            var e = ps.emission;
            e.enabled = false;
        }

        var sprite = GetComponentInChildren<SpriteRenderer>();
        if ( sprite != null )
        {
            sprite.DOFade(0, 1f).SetEase(Ease.InOutCubic);
        }

        Destroy(gameObject, 3f);
    }
}
