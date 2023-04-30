using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rewards
{
    internal struct RewardsPeriodType
    {
        public float TimeCooldown;
        public float TimeDeadline;

        public RewardsPeriodType(float timeCooldown, float timeDeadline)
        {
            TimeCooldown = timeCooldown;
            TimeDeadline = timeDeadline;
        }
    }

    internal class BaseRewardView : MonoBehaviour
    {
        private const string CurrentSlotInActiveKey = nameof(CurrentSlotInActiveKey);
        private const string TimeGetRewardKey = nameof(TimeGetRewardKey);

        private readonly RewardsPeriodType _dailyReward = new(86400, 172800);
        private readonly RewardsPeriodType _weeklyReward = new(604800, 2419200);

        private float _timeCooldown;
        private float _timeDeadline;

        [Header("Settings Time Get Reward")]
        [SerializeField] private EPeriodType Period;

        [field: Header("Settings Reward")]
        [field: SerializeField] public List<Reward> Rewards { get; private set; }

        [field: Header("Ui Elements")]
        [field: SerializeField] public TMP_Text TimerNewReward { get; private set; }
        [field: SerializeField] public Transform MountRootSlotsReward { get; private set; }
        [field: SerializeField] public ContainerSlotRewardView ContainerSlotRewardPrefab { get; private set; }
        [field: SerializeField] public Button GetRewardButton { get; private set; }
        [field: SerializeField] public Button ResetButton { get; private set; }

        public float TimeCooldown { get => _timeCooldown; private set => _timeCooldown = value; }
        public float TimeDeadline { get => _timeDeadline; private set => _timeDeadline = value; }

        private void Start()
        {
            switch(Period)
            {
                case EPeriodType.Daily:
                    _timeCooldown = _dailyReward.TimeCooldown;
                    _timeDeadline = _dailyReward.TimeDeadline;
                    break;
                case EPeriodType.Weekly:
                    _timeCooldown = _weeklyReward.TimeCooldown;
                    _timeDeadline = _weeklyReward.TimeDeadline;
                    break;
            }
        }

        public int CurrentSlotInActive
        {
            get => PlayerPrefs.GetInt(CurrentSlotInActiveKey);
            set => PlayerPrefs.SetInt(CurrentSlotInActiveKey, value);
        }

        public DateTime? TimeGetReward
        {
            get
            {
                string data = PlayerPrefs.GetString(TimeGetRewardKey);
                return !string.IsNullOrEmpty(data) ? DateTime.Parse(data) : null;
            }
            set
            {
                if (value != null)
                    PlayerPrefs.SetString(TimeGetRewardKey, value.ToString());
                else
                    PlayerPrefs.DeleteKey(TimeGetRewardKey);
            }
        }

    }
}