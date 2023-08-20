using Cysharp.Threading.Tasks;
using TMPro;
using TSG.Model;
using TSG.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace TSG.Game
{
    public class Overlay : PopupStateModel<Overlay, PlayerModel>
    {
        [SerializeField] private TextMeshProUGUI playerHealth;
        [SerializeField] private TextMeshProUGUI score;

        private Player player;

        public override void Setup(PlayerModel model)
        {
            base.Setup(model);
            model.damageTaken += OnModelTakenDamage;
            model.killedEnemy += OnModelKilledEnemy;
            model.revive += OnModelRevived;

            UpdateUI();
        }

        public void GetPlayer(Player player)
        {
            this.player = player;
        }

        private void OnModelKilledEnemy(PlayerModel obj)
        {
            UpdateUI();
        }

        private void OnModelTakenDamage(PlayerModel arg1, float arg2)
        {
            UpdateUI();
        }

        private void OnModelRevived(PlayerModel obj)
        {
            Game.Get<PopupManager>().Open<Overlay>().Forget();
            obj.revive -= OnModelRevived;
        }

        private void Unsubscribe(PlayerModel model)
        {
            model.damageTaken -= OnModelTakenDamage;
            model.killedEnemy -= OnModelKilledEnemy;
        }

        private void UpdateUI()
        {
            playerHealth.text = model.HitPoints.ToString();
            score.text = model.Score.ToString();

            if (model.HitPoints == 0)
            {
                Unsubscribe(model);
                Close();
            }
        }

        public void MoveLeft()
        {
            player.MoveLeft();
        }
        
        public void MoveRight()
        {
            player.MoveRight();
        }

        public void Shoot()
        {
            player.Shoot();
        }
    }
}
