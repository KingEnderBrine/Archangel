using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Archangel
{
    public class SwordsLocator : MonoBehaviour
    {
        public SwordsController ControllerInstance { get; private set; }
        public GameObject instance;

        private CharacterModel characterModel;

        private void Awake()
        {
            TryInit();
        }

        private void Start()
        {
            TryInit();

            characterModel = GetComponent<CharacterModel>();
            var display = new CharacterModel.ParentedPrefabDisplay
            {
                equipmentIndex = EquipmentIndex.None,
                itemIndex = ItemIndex.None,
            };

            display.instance = instance;
            display.itemDisplay = instance.GetComponent<ItemDisplay>();
            characterModel.parentedPrefabDisplays.Add(display);
        }

        private void TryInit()
        {
            if (!instance)
            {
                return;
            }

            if (!ControllerInstance)
            {
                ControllerInstance = instance.GetComponent<SwordsController>();
            }
        }

        private void OnDestroy()
        {
            if (characterModel && instance)
            {
                var displayIndex = characterModel.parentedPrefabDisplays.FindIndex(el => el.instance == instance);
                if (displayIndex != -1)
                {
                    characterModel.parentedPrefabDisplays.RemoveAt(displayIndex);
                    Destroy(instance);
                }
            }
        }
    }
}