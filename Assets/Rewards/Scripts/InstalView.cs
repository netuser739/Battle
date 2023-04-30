using UnityEngine;

namespace Rewards
{
    internal class InstalView : MonoBehaviour
    {
        [SerializeField] private BaseRewardView _rewardView;

        private BaseRewardController _rewardController;

        private void Awake() =>
            _rewardController = new BaseRewardController(_rewardView);

        private void Start() =>
            _rewardController.Init();

        private void OnDestroy() =>
            _rewardController.Deinit();
    }
}
