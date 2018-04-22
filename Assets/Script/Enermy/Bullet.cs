using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour {
	[SerializeField][ReadOnlyAttribute] Vector3 velocity;
	[SerializeField][ReadOnlyAttribute] float damage;
	[SerializeField][ReadOnlyAttribute] Vector3 position;
	[SerializeField][ReadOnlyAttribute] Enermy parent;

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
		position += velocity * Time.deltaTime;
		transform.position = position;

		if (Global.CheckInFrame (position, ref velocity )) {
			transform.DOScale (0, 2f).OnComplete (delegate {
				Destroy (gameObject);
			});
		}
	}

	public void OnTriggerEnter2D( Collider2D col )
	{
		if (col.tag == "Player") {
			MPlayer.Instance.Attack (parent, damage);
		}
	}
}
