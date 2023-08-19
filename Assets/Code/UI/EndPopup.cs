using TMPro;
using TSG.Model;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TSG.Popups
{
	public class EndPopup : PopupStateModel<EndPopup, PlayerModel>
	{
		[SerializeField] private TextMeshProUGUI currentScore;
		[SerializeField] private TextMeshProUGUI highScore;
		[SerializeField] private Button playButton;

		private DataManager dataManager;
		
		public override void Init()
		{
			base.Init();
			playButton.onClick.AddListener(OnPlayButtonClicked);
		}

		public override void Setup(PlayerModel model)
		{
			base.Setup(model);
			
			dataManager = Game.Game.Get<DataManager>();
			UpdateUI();
		}

		private void OnPlayButtonClicked()
		{
			model.Revive();
			Close();
		}
		
		private void UpdateUI()
		{
			highScore.text = $"Your highest score: {dataManager.HighScore}";
			currentScore.text = $"You lost with score: {model.Score.ToString()}";
		}
		
		public override void Dispose()
		{
			base.Dispose();
			playButton.onClick.RemoveListener(OnPlayButtonClicked);
		}
	}
}
