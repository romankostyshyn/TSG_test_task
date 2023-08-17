using UnityEngine;

namespace TSG.Game
{
	public class Bullet : MonoBehaviour
	{
		private float damage;
		private Rigidbody cachedRigidbody;

		private void Awake()
		{
			cachedRigidbody = GetComponent<Rigidbody>();
		}

		public void Setup(float speed, float damage)
		{
			cachedRigidbody.velocity = Vector3.forward * speed;
			this.damage = damage;
		}

		public void GiveDamage(Enemy enemy)
		{
			enemy.TakeDamage(damage);
			Destroy(gameObject);
		}
	}
}