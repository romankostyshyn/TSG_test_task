using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Tools;
using TSG.Model;
using TSG.Popups;
using UnityEngine;
using UnityEngine.UI;
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

            var overlay = Game.Get<PopupManager>().Get<Overlay>();
            overlay.Setup(player.Model);
            overlay.GetPlayer(player);
            Game.Get<PopupManager>().Open<Overlay>().Forget();
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
                    }

                    state = false;
                }

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
            var g = Pool.Instantiate(spawnerConfig.Prefab, enemySpawnPoint.transform.position,
                Quaternion.identity, enemySpawner.transform);
            g.transform.position = enemySpawnPoint.transform.position +
                                   new Vector3(
                                       Random.Range(spawnerConfig.SpawnPosition.x, spawnerConfig.SpawnPosition.y),
                                       0, 0);
            enemy = g.GetComponent<Enemy>();
            enemy.Setup(new EnemyModel(enemyConfig));
            enemy.onImpact -= HandleEnemyImpact;
            enemy.onDie -= HandleEnemyDeath;
            enemy.onImpact += HandleEnemyImpact;
            enemy.onDie += HandleEnemyDeath;
        }

        private void HandleEnemyDeath(Enemy obj)
        {
            Pool.Destroy(obj);
        }

        private void HandleEnemyImpact(Enemy enemy, GameObject other)
        {
            if (other.gameObject.TryGetComponent<Player>(out var playerComponent))
            {
                playerComponent.TakeDamage(enemy.Model.Damage);
                enemy.Model.OffRange();
            }
            else if (other.gameObject.TryGetComponent<Bullet>(out var bullet))
            {
                bullet.GiveDamage(enemy);

                if (enemy.Model.IsDead())
                {
                    player.Model.KillEnemy();
                }
            }
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