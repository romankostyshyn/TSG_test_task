using TSG.Model;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TSG.Popups
{
	public class EndPopup : PopupStateModel<EndPopup, PlayerModel>
	{
		[SerializeField] private Button playButton;
		
		public override void Init()
		{
			base.Init();
			playButton.onClick.AddListener(OnPlayButtonClicked);
		}

		private void OnPlayButtonClicked()
		{
			model.Revive();
			Close();
		}
		
		public override void Dispose()
		{
			base.Dispose();
			playButton.onClick.RemoveListener(OnPlayButtonClicked);
		}
	}
}
