using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;
using DG.Tweening;

public class Enermy : MonoBehaviour {

	[SerializeField] protected float vel = 9.5f;
	[SerializeField] protected float acc = 100f;
	[SerializeField] protected float attVel = 30f;
	[SerializeField] protected float attDuration = 0.5f;
	[SerializeField] protected Rigidbody2D m_rigidbody;
	[SerializeField] protected PlayMakerFSM fsm;
	[SerializeField] protected SpriteRenderer m_sprite;
	[SerializeField] protected float animationDuration = 1f;
	[SerializeField] protected float scale = 1f;
	[SerializeField] protected float dmg = 10f;
	[SerializeField][ReadOnlyAttribute] protected Vector3 m_velocity;
	[SerializeField][ReadOnlyAttribute] protected Vector3 toPlayer;

	[SerializeField][ReadOnlyAttribute] float m_health = 100f;
    [SerializeField] GameObject hitEffect;

	[SerializeField] protected AudioSource m_source;
	[SerializeField]protected AudioClip attackSFX;
	[SerializeField]protected AudioClip hitSFX;

	[SerializeField] bool isSpin = true;

	private float randomSeed = 0;

	public void Awake()
	{
		m_sprite.transform.localScale = Vector3.zero;

		randomSeed = Random.Range (0,1f);
	}

    public void Start()
    {
        GameController.Instance.RegisterEnermy(this);
    }

    public void Update()
	{
		toPlayer = (- transform.position + MPlayer.Instance.Position);
		toPlayer.z = 0;
		float angle = Vector3.Angle (toPlayer, m_velocity);
		fsm.FsmVariables.GetVariable ("PlayerDis").SafeAssign (toPlayer.magnitude);
		fsm.FsmVariables.GetVariable ("Angle").SafeAssign (angle);
	}

    public void EnterShow()
	{
		m_sprite.transform.localScale = Vector3.zero;
		m_sprite.transform.DOScale (scale , animationDuration).SetEase(Ease.InOutCubic);

		GetComponent<BoxCollider2D> ().size = Vector2.one * scale;
		if (isSpin)
			m_sprite.transform.DOLocalRotate (new Vector3 (0, 0, 360f * 5f), animationDuration).SetEase (Ease.OutCubic);

		m_rigidbody.isKinematic = true;
    }

    public void EnterChase()
    {
		m_velocity = m_velocity.normalized * vel;

		m_rigidbody.isKinematic = false;
    }

    public void UpdateChase()
    {
		transform.up = m_velocity;

		m_velocity += acc * toPlayer.normalized * Time.deltaTime;

		m_velocity = Vector3.ClampMagnitude (m_velocity, vel);

		m_rigidbody.velocity = m_velocity;

		Global.CheckInFrame (transform.position, ref m_velocity);
    }

    public void EnterRandomMove()
    {


		m_velocity = m_velocity.normalized * vel * 0.5f ;

		randDir = (Vector3)Random.insideUnitCircle.normalized;
		m_rigidbody.isKinematic = false;
    }


	Vector3 randDir;

    public void UpdateRandomMove()
    {
		
		transform.up = m_velocity;

		Vector3 pos = transform.position;

		if ( Random.Range( 0 , 1f ) < 0.01f )
			randDir = (Vector3)Random.insideUnitCircle.normalized;

		m_velocity += acc * Time.deltaTime * randDir.normalized;

		Global.CheckInFrame (transform.position, ref m_velocity);

		m_velocity = Vector3.ClampMagnitude (m_velocity, vel * 0.5f );

		m_rigidbody.velocity = m_velocity;
    }

    public void EnterAttack()
    {
		m_velocity = toPlayer.normalized * attVel;

		timer = 0;

		m_source.clip = attackSFX;
		m_source.Play ();
    }


	float timer = 0;
    public void UpdateAttack()
    {
		// if (Vector3.Dot (toPlayer, m_velocity) < 0.5f || toPlayer.magnitude > 10f )
//			m_velocity *= Mathf.Max( 0 , 1 - 2f * Time.deltaTime);
		timer += Time.deltaTime;

		if (timer > attDuration )
			fsm.SendEvent ("ToMove");

		m_rigidbody.velocity = m_velocity;
    }

	public void EnterDie()
	{
		m_rigidbody.isKinematic = true;

		m_rigidbody.velocity = Vector3.zero;

		m_sprite.transform.DOScale (0, animationDuration).SetEase (Ease.InOutCubic).OnComplete (delegate {

            GameController.Instance.EnermyDead(this);

			Destroy( gameObject );	
		});
	}



	public void OnTriggerEnter2D( Collider2D col )
	{
		if (col.tag == "Player") {
			MPlayer.Instance.Attack (this, dmg);
		}
	}

	public void Attack( float dmg )
	{
		m_source.clip = hitSFX;
		m_source.Play ();

		m_health -= dmg;

        m_sprite.transform.DOShakePosition(0.5f, 0.5f);

        var hit = Instantiate(hitEffect) as GameObject;
        hit.transform.parent = transform;
        hit.transform.localPosition = Vector3.zero;

		if (m_health <= 0) {
			fsm.SendEvent ("ToDie");
		}
	}

}
