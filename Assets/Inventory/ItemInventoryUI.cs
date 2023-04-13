﻿using System.Collections.Generic;
using System.Linq;
using Enumerations;
using GameModes;
using Interactables;
using ServerConnection;
using Shop;
using UnityEngine;

namespace Inventory
{
    public class ItemInventoryUI : MonoBehaviour
    {
        public static readonly StringUnityEvent OnBoughtItemEvent = new StringUnityEvent();

        [SerializeField] private ShopInventory ShopInventory;
        [SerializeField] private GameObject InventoryItemPrefab;
        [SerializeField] private GameObject InventoryUIContent;
        [SerializeField] private GameObject FilterPanel;
        [SerializeField] private ToggleInventoryButton ToggleButton;

        private readonly Dictionary<string, GameObject> inventoryItems = new Dictionary<string, GameObject>();

        private void Awake()
        {
            OnBoughtItemEvent.AddListener(OnBoughtItem);
        }

        private void Start()
        {
            SaveGame.LoadedPlayerData.AddListener(UpdateItemStates);

            UIManager.FILTER_EVENT.AddListener(OnFilterToggled);

            SelectionManager.SELECT_SOCKET_EVENT.AddListener(OnSelectedSocket);

            GameModeSwitcher.OnSwitchedGameMode.AddListener(OnSwitchedGameMode);

            LocalPlayerData.ChangedItemDataEvent.AddListener(UpdateItemState);
        }

        private void OnSwitchedGameMode(GameMode gameMode)
        {
            if (gameMode == GameMode.DecoInventory)
            {
                OnFilterToggled(UIType.Inventory,
                    new List<ItemAttribute> { ItemAttribute.Wide, ItemAttribute.VeryHuge },
                    true);
                FilterPanel.SetActive(false);
            }
            else if(gameMode == GameMode.Default)
            {
                FilterPanel.SetActive(true);
                OnFilterToggled(UIType.Inventory,
                    new List<ItemAttribute> { ItemAttribute.Wide, ItemAttribute.VeryHuge },
                    true);
            }
        }

        private void UpdateItemState(string id)
        {
            var item = inventoryItems.FirstOrDefault(i => i.Key == id);
            var itemData = ShopInventory.ShopItems.FirstOrDefault(i => i.ItemID == id);
            if (item.Value == null) return;
            item.Value.SetActive(true);
            item.Value.GetComponent<InventoryItem>().Initialize(itemData);

            if (LocalPlayerData.Instance.GetCountOfUnplacedItems(id) == 0) item.Value.SetActive(false);
        }

        private void OnSelectedSocket(Socket socket)
        {
            if (socket.IsUsed) return;
            GameModeSwitcher.SwitchGameMode.Invoke(GameMode.SelectedSocket);
        }

        private void OnFilterToggled(UIType uiType, List<ItemAttribute> attributes, bool isActive)
        {
            if (uiType == UIType.Shop) return;

            foreach (var item in inventoryItems)
            {
                if (item.Value.GetComponent<InventoryItem>().attributes.Intersect(attributes).Any()
                    && LocalPlayerData.Instance.GetCountOfUnplacedItems(item.Key) > 0)
                {
                    item.Value.SetActive(!isActive);
                }
            }
        }

        private void UpdateItemStates()
        {
            var ownedItems = LocalPlayerData.Instance.GetOwnedItems();

            var uniqueItems =
                ownedItems.GroupBy(i => i.id, o => o.uid, (key, uid) => new { Id = key, UIDs = uid.ToList() });

            foreach (var item in uniqueItems)
            {
                CreateInventoryItem(item.Id);
            }
        }

        private void OnBoughtItem(string itemId)
        {
            if (inventoryItems.TryGetValue(itemId, out GameObject key))
            {
                key.GetComponent<InventoryItem>().UpdateUI();
            }
            else
            {
                CreateInventoryItem(itemId);
            }
        }

        private void CreateInventoryItem(string itemId)
        {
            var itemGO = Instantiate(InventoryItemPrefab, InventoryUIContent.transform);
            var inventoryItem = itemGO.GetComponent<InventoryItem>();
            var itemData = ShopInventory.ShopItems.FirstOrDefault(i => i.ItemID == itemId);
            inventoryItem.Initialize(itemData);

            inventoryItems.Add(itemId, itemGO);
        }
    }
}