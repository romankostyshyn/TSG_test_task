using System;
using Tools;
using UnityEngine;

namespace TSG.Game
{
	public class Bullet : MonoBehaviour
	{
		private float damage;
		private Rigidbody cachedRigidbody;
		public float endPoint;

		private void Awake()
		{
			cachedRigidbody = GetComponent<Rigidbody>();
		}

		public void Setup(float speed, float damage)
		{
			cachedRigidbody.velocity = Vector3.forward * speed;
			this.damage = damage;
		}

		private void Update()
		{
			if (transform.position.z > endPoint)
			{
				gameObject.DestroyWithPool(this);
			}
		}

		public void GiveDamage(Enemy enemy)
		{
			enemy.TakeDamage(damage);
			Pool.Destroy(this);
		}
	}
}