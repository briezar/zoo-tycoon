using UnityEngine;
using UnityEngine.UI;
using ZooTycoon.RuntimeData;

namespace ZooTycoon.UI
{
    public class GameUI : ScreenUI
    {
        [SerializeField] private HabitatBuildStrip _habitatBuildStrip;
        [SerializeField] private Button _buildHabitatBtn;

        protected override void OnStart()
        {            
            _habitatBuildStrip.Hide(true);

            _buildHabitatBtn.onClick.AddListener(HandleOnClickBuildHabitat);
        }
        private void HandleOnClickBuildHabitat()
        {
            if (_habitatBuildStrip.IsShowing)
            {
                _habitatBuildStrip.Hide();
            }
            else
            {
                _habitatBuildStrip.Show();
            }
        }
    }
}
