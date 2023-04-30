using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rewards
{
    internal class BaseRewardController
    {
        private readonly BaseRewardView _view;
        private List<ContainerSlotRewardView> _slots;
        private Coroutine _coroutine;
        private bool _isInitialized;
        private bool _isGetReward;

        public BaseRewardController(BaseRewardView view) =>
            _view = view;

        public void Init()
        {
            if (_isInitialized)
                return;

            InitSlots();
            RefreshUi();
            StartRewardsUpdating();
            SubscribeButtons();

            _isInitialized = true;
        }

        public void Deinit()
        {
            if (!_isInitialized)
                return;

            DeinitSlots();
            StopRewardsUpdating();
            UnsubscribeButtons();

            _isInitialized = false;
        }

        private void InitSlots()
        {
            _slots = new List<ContainerSlotRewardView>();

            for (int i = 0; i < _view.Rewards.Count; i++)
            {
                ContainerSlotRewardView instanceSlot = CreateSlotRewardView();
                _slots.Add(instanceSlot);
            }
        }

        private ContainerSlotRewardView CreateSlotRewardView() =>
            Object.Instantiate(_view.ContainerSlotRewardPrefab, _view.MountRootSlotsReward, false)
            .GetComponent<ContainerSlotRewardView>();

        private void DeinitSlots()
        {
            foreach (ContainerSlotRewardView slot in _slots)
                Object.Destroy(slot.gameObject);

            _slots.Clear();
        }

        private void StartRewardsUpdating() =>
            _coroutine = _view.StartCoroutine(RewardStateUpdater());

        private void StopRewardsUpdating()
        {
            if (_coroutine == null)
                return;

            _view.StopCoroutine(_coroutine);
            _coroutine = null;
        }

        private IEnumerator RewardStateUpdater()
        {
            var waitForSeconds = new WaitForSeconds(1);
            while (true)
            {
                RefreshRewardState();
                RefreshUi();
                yield return waitForSeconds;
            }
        }

        private void SubscribeButtons()
        {
            _view.GetRewardButton.onClick.AddListener(ClaimReward);
            _view.ResetButton.onClick.AddListener(ResetRewardState);
        }

        private void UnsubscribeButtons()
        {
            _view.GetRewardButton.onClick.RemoveListener(ClaimReward);
            _view.ResetButton.onClick.RemoveListener(ResetRewardState);
        }

        private void ClaimReward()
        {
            if (!_isGetReward)
                return;

            Reward reward = _view.Rewards[_view.CurrentSlotInActive];

            switch (reward.RewardType)
            {
                case RewardType.Wood:
                    CurrencyView.Instance.AddWood(reward.CountCurrency);
                    break;

                case RewardType.Diamond:
                    CurrencyView.Instance.AddDiamond(reward.CountCurrency);
                    break;
            }

            _view.TimeGetReward = DateTime.UtcNow;
            _view.CurrentSlotInActive++;

            RefreshRewardState();
        }

        private void RefreshRewardState()
        {
            bool gotRewardEarlier = _view.TimeGetReward.HasValue;
            if (!gotRewardEarlier)
            {
                _isGetReward = true;
                return;
            }

            TimeSpan timeFromLastRewardGetting = DateTime.UtcNow - _view.TimeGetReward.Value;

            bool isDeadLineElapsed = timeFromLastRewardGetting.Seconds >= _view.TimeDeadline;

            bool isTimetoGetNewReward = timeFromLastRewardGetting.Seconds >= _view.TimeCooldown;

            if (isDeadLineElapsed)
                ResetRewardState();

            _isGetReward = isTimetoGetNewReward;

        }

        private void ResetRewardState()
        {
            _view.TimeGetReward = null;
            _view.CurrentSlotInActive = default;
        }

        private void RefreshUi()
        {
            _view.GetRewardButton.interactable = _isGetReward;
            _view.TimerNewReward.text = GetTimerNewRewardText();
            RefreshSlots();
        }

        private string GetTimerNewRewardText()
        {
            if (_isGetReward)
                return "The reward is ready to be received!";

            if (_view.TimeGetReward.HasValue)
            {
                DateTime nextClaimTime = _view.TimeGetReward.Value.AddSeconds(_view.TimeCooldown);
                TimeSpan currentClaimCooldown = nextClaimTime - DateTime.UtcNow;

                string timeGetReward =
                    $"{currentClaimCooldown.Days:D2}:{currentClaimCooldown.Hours:D2}:" +
                    $"{currentClaimCooldown.Minutes:D2}:{currentClaimCooldown.Seconds:D2}";

                return $"Time to get next reward: {timeGetReward}";
            }

            return string.Empty;
        }

        private void RefreshSlots()
        {
            for (int i = 0; i < _view.Rewards.Count; i++)
            {
                Reward reward = _view.Rewards[i];
                int countDay = i + 1;
                bool isSelected = i == _view.CurrentSlotInActive;

                _slots[i].SetData(reward, countDay, isSelected);
            }
        }
    }
}
