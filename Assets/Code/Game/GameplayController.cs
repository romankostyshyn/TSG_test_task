﻿using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Tools;
using TSG.Model;
using TSG.Popups;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TSG.Game
{
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] private EnemyConfig enemyConfig;
        [SerializeField] private PlayerConfig playerConfig;
        [SerializeField] private SpawnerConfig spawnerConfig;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private Transform enemySpawnPoint;
        [SerializeField] private GameObject enemySpawner;

        private Player player;
        private Enemy enemy;
        private float lastTimeSpawnedEnemy;
        private bool state;

        private void Start()
        {
            SpawnPlayer();
            
            var topBar = Game.Get<PopupManager>().Get<TopBar>();
            topBar.Setup(player.Model);
            Game.Get<PopupManager>().Open<TopBar>().Forget();
        }

        private void Update()
        {
            if (player.Model.IsDead())
            {
                if (state)
                {
                    foreach (Transform child in enemySpawner.transform)
                    {
                        EnemyVisible(child);
                        //enemy.Model.OffRange();
                        //Debug.Log(child);
                    }

                    state = false;
                }
                
                //Pool.Destroy(enemy);
                //state = false;
                //EnemyVisible(state);
                return;
            }

            if (lastTimeSpawnedEnemy + spawnerConfig.SpawnDelay <= Time.timeSinceLevelLoad)
            {
                lastTimeSpawnedEnemy = Time.timeSinceLevelLoad;
                SpawnEnemy();
            }
        }

        private void EnemyVisible(Transform enemy)
        {
            Pool.Destroy(enemy);
            this.enemy.Model.OffRange();
            this.enemy.onImpact -= HandleEnemyImpact;
            this.enemy.onDie -= HandleEnemyDeath;
        }

        private void SpawnPlayer()
        {
            var playerGo = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity, transform);
            player = playerGo.GetComponent<Player>();
            player.Setup(new PlayerModel(playerConfig), bulletPrefab);

            player.onDie += HandlePlayerDeath;
        }
        
        private void SpawnEnemy()
        {
            // if (state == false)
            // {
            //     state = true;
            //     EnemyVisible(state);
            // }
            var g = Pool.Instantiate(spawnerConfig.Prefab, enemySpawnPoint.transform.position,
                Quaternion.identity, enemySpawner.transform);
            g.transform.position = enemySpawnPoint.transform.position +
                                   new Vector3(
                                       Random.Range(spawnerConfig.SpawnPosition.x, spawnerConfig.SpawnPosition.y),
                                       0, 0);
            enemy = g.GetComponent<Enemy>();
            enemy.Setup(new EnemyModel(enemyConfig));
            enemy.onImpact -= HandleEnemyImpact;
            enemy.onImpact += HandleEnemyImpact;
            enemy.onDie += HandleEnemyDeath;
        }

        private void HandleEnemyDeath(Enemy obj)
        {
            Debug.Log("dead");
            Pool.Destroy(obj);
            obj.onImpact -= HandleEnemyImpact;
            obj.onDie -= HandleEnemyDeath;
        }

        private void HandleEnemyImpact(Enemy enemy, GameObject other)
        {
            Debug.Log("spawn");
            if (other.gameObject.TryGetComponent<Player>(out var playerComponent))
            {
                Debug.Log("connect");
                playerComponent.TakeDamage(enemy.Model.Damage);
                enemy.Model.OffRange();
                enemy.onImpact -= HandleEnemyImpact;
                enemy.onDie -= HandleEnemyDeath;
            }
            else if (other.gameObject.TryGetComponent<Bullet>(out var bullet))
            {
                bullet.GiveDamage(enemy);

                if (enemy.Model.IsDead())
                {
                    Debug.Log("why?");
                    player.Model.KillEnemy();
                    enemy.onImpact -= HandleEnemyImpact;
                    enemy.onDie -= HandleEnemyDeath;
                }
            }
                
            Debug.Log(enemy.Model.IsDead());
        }

        private void HandlePlayerDeath(Player playerModel)
        {
            state = true;
            var endPopup = Game.Get<PopupManager>().Get<EndPopup>();
            endPopup.Setup(player.Model);
            Game.Get<PopupManager>().Open<EndPopup>().Forget();
        }
    }
}
