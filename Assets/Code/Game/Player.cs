using System;
using Tools;
using TSG.Model;
using TSG.Popups;
using UnityEngine;

namespace TSG.Game
{
	public class Player : MonoBehaviour
	{
		public event Action<Player> onDie = delegate { };

		private PlayerModel model;
		private Bullet bulletPrefab;
		private float lastTimeShot;

		public PlayerModel Model => model;

		public void Setup(PlayerModel model, Bullet bulletPrefab)
		{
			this.model = model;
			this.bulletPrefab = bulletPrefab;
			model.die += OnModelDie;
			model.revive += Revive;
		}

		private void OnModelDie(PlayerModel obj)
		{
			var dataManager = Game.Get<DataManager>();
			dataManager.SaveHighScore(model.Score);
			onDie(this);
		}

		public void TakeDamage(float damage)
		{
			model.TakeDamage(damage);
		}

		private void Revive(PlayerModel obj)
		{
			var topBar = Game.Get<PopupManager>().Get<TopBar>();
			topBar.Setup(Model);
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

		public void MoveLeft()
		{
			var pos = transform.position;
			pos.x -= model.Speed * Time.deltaTime;
			transform.position = pos;
		}

		public void MoveRight()
		{
			var pos = transform.position;
			pos.x += model.Speed * Time.deltaTime;
			transform.position = pos;
		}

		public void Shoot()
		{
			if (lastTimeShot + model.BulletCooldown <= Time.timeSinceLevelLoad)
			{
				var bulletGo = Pool.Instantiate(bulletPrefab, transform.position, Quaternion.identity, null);
				var bullet = bulletGo.GetComponent<Bullet>();
				bullet.Setup(model.BulletSpeed, model.BulletDamage);
				lastTimeShot = Time.timeSinceLevelLoad;
			}
		}
	}
}