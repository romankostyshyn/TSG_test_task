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

        private Player player;
        private float lastTimeSpawnedEnemy;

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
                return;
            }

            if (lastTimeSpawnedEnemy + spawnerConfig.SpawnDelay <= Time.timeSinceLevelLoad)
            {
                lastTimeSpawnedEnemy = Time.timeSinceLevelLoad;
                SpawnEnemy();
            }
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
                Quaternion.identity);
            g.transform.position = enemySpawnPoint.transform.position +
                                   new Vector3(
                                       Random.Range(spawnerConfig.SpawnPosition.x, spawnerConfig.SpawnPosition.y),
                                       0, 0);
            var enemy = g.GetComponent<Enemy>();
            enemy.Setup(new EnemyModel(enemyConfig));
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
            var endPopup = Game.Get<PopupManager>().Get<EndPopup>();
            endPopup.Setup(player.Model);
            Game.Get<PopupManager>().Open<EndPopup>().Forget();
        }
    }
}
