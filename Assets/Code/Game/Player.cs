using System;
using TSG.Model;
using UnityEngine;

namespace TSG.Game
{
	public class Player : MonoBehaviour
	{
		public event Action<Player> onDie = delegate { };

		private PlayerModel model;
		private GameObject bulletPrefab;
		private float lastTimeShot;

		public PlayerModel Model => model;

		public void Setup(PlayerModel model, GameObject bulletPrefab)
		{
			this.model = model;
			this.bulletPrefab = bulletPrefab;
			model.die += OnModelDie;
		}

		private void OnModelDie(PlayerModel obj)
		{
			onDie(this);
		}

		public void TakeDamage(float damage)
		{
			model.TakeDamage(damage);
		}

		private void Update()
		{
			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			{
				MoveLeft();
			}
			else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			{
				MoveRight();
			}

			if (Input.GetKeyDown(KeyCode.Space))
			{
				Shoot();
			}
		}

		private void MoveLeft()
		{
			var pos = transform.position;
			pos.x -= model.Speed * Time.deltaTime;
			transform.position = pos;
		}

		private void MoveRight()
		{
			var pos = transform.position;
			pos.x += model.Speed * Time.deltaTime;
			transform.position = pos;
		}

		private void Shoot()
		{
			if (lastTimeShot + model.BulletCooldown <= Time.timeSinceLevelLoad)
			{
				var bulletGo = Instantiate(bulletPrefab, transform.position, Quaternion.identity, null);
				var bullet = bulletGo.GetComponent<Bullet>();
				bullet.Setup(model.BulletSpeed, model.BulletDamage);
				lastTimeShot = Time.timeSinceLevelLoad;
			}
		}
	}
}