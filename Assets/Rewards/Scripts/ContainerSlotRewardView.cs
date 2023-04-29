using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Rewards
{
    internal class ContainerSlotRewardView : MonoBehaviour
    {
        [SerializeField] private Image _originalBackground;
        [SerializeField] private Image _selectBackground;
        [SerializeField] private Image _iconCurrency;
        [SerializeField] private TMP_Text _textDays;
        [SerializeField] private TMP_Text _countReward;

        public void SetData(Reward reward, int countDays, bool isSelected)
        {
            _iconCurrency.sprite = reward.IconCurrency;
            _textDays.text = $"Day {countDays}";
            _countReward.text = reward.CountCurrency.ToString();

            UpdateBackground(isSelected);
        }

        private void UpdateBackground(bool isSelected)
        {
            _originalBackground.gameObject.SetActive(!isSelected);
            _selectBackground.gameObject.SetActive(isSelected);
        }
    }
}