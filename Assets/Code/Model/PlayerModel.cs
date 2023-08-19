using System;
using TSG.Utils;
using UnityEngine;

namespace TSG.Model
{
	public class PlayerModel : IModel
	{
		public event Action<PlayerModel> die = delegate { };
		public event Action<PlayerModel> killedEnemy = delegate { };
		public event Action<PlayerModel> revive = delegate { };
		public event Action<PlayerModel, float> damageTaken = delegate { };
		
		public float Speed { get; }
		public float BulletDamage { get; }
		public float BulletSpeed { get;}
		public float BulletCooldown { get; }
		public int Score { get; private set; }
		public int HitPoints { get; private set; }

		private readonly int hitPoints;
		
		public PlayerModel(PlayerConfig config)
		{
			Speed = config.Speed;
			HitPoints = config.Hitpoints;
			BulletDamage = config.BulletDamage;
			BulletCooldown = config.BulletCooldown;
			BulletSpeed = config.BulletSpeed;

			hitPoints = config.Hitpoints;
		}
		
		public bool IsDead()
		{
			return HitPoints == 0;
		}

		public void Revive()
		{
			HitPoints = hitPoints;
			revive(this);
		}

		public void TakeDamage(float damage)
		{
			Debug.Log("Player takes damage");
			if (IsDead())
			{
				return;
			}
			
			HitPoints = (int) Mathf.Max(0, HitPoints - damage);
			damageTaken(this, damage);
			
			if (!IsDead())
			{
				return;
			}
            
			die(this);
		}

		public void KillEnemy()
		{
			Score += 1;
			killedEnemy(this);
		}
	}
}