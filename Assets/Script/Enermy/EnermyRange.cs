using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnermyRange : Enermy {

	[SerializeField] GameObject bullet;
	[SerializeField] float bulletSpeed = 40f;

	public enum BulletType
	{
		Normal,

	}
	[SerializeField] BulletType bulletType;

	public void EnterAvoid()
	{
		m_velocity = m_velocity.normalized * vel;
		m_rigidbody.isKinematic = false;
	}

	public void UpdateAvoid()
	{

		transform.up = m_velocity;

		m_velocity += - acc * toPlayer.normalized * Time.deltaTime;
		//		m_velocity = toPlayer.normalized * vel;

		m_velocity = Vector3.ClampMagnitude (m_velocity, vel);

		m_rigidbody.velocity = m_velocity;

		Global.CheckInFrame (transform.position, ref m_velocity);
	}

	public void FireBullet()
	{
		if (bulletType == BulletType.Normal) {
			FireNormalBullet ();
		}
	}
	public void UpdateStand()
	{
		transform.up = toPlayer.normalized;
	}

	public void FireNormalBullet()
	{
		m_source.clip = attackSFX;
		m_source.Play ();

		CreateBullet (toPlayer.normalized);
	}

	public void CreateBullet( Vector3 dir )
	{
		var bul = Instantiate (bullet) as GameObject;
		bul.GetComponent<Bullet> ().Init (dir * bulletSpeed, dmg, transform.position, this);
	}

}
