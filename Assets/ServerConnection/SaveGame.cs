﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Interactables;
using Shop;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace ServerConnection
{
    public class SaveGame : MonoBehaviour
    {
        [SerializeField] private ItemCreator itemCreator;
        [SerializeField] private GameObject SocketItem;
        [SerializeField] private ShopInventory shopInventory;
        
        public static UnityEvent LoadedPlayerData = new UnityEvent();

        private LocalPlayerData _localPlayerData;
        
        private void Awake()
        {
            // TODO Load Data from Server
            _localPlayerData = gameObject.AddComponent<LocalPlayerData>();
        }

        private void Start()
        {
            StartCoroutine(LoadGameData());
        }

        private void SaveGameData()
        {
            string json = _localPlayerData.GetJsonData();
            
            File.WriteAllText(Application.persistentDataPath + "/gamedata.json", json);
        }

        private IEnumerator LoadGameData()
        {
            if (!File.Exists(Application.persistentDataPath + "/gamedata.json"))
            {
                SaveGameData();
            }
            
            _localPlayerData.Initilize(
                JsonUtility.FromJson<PlayerDataContainer>(
                    File.ReadAllText(Application.persistentDataPath + "/gamedata.json")));

            var itemDatas = _localPlayerData.GetPlacedItems().ToList();

            List<GameObject> gos = new List<GameObject>();
            List<int> uids = new List<int>();

            var itemswithsockets = itemDatas.Where(i => i.itemsPlacedOnSockets.Length > 0);
            var itemswithoutsockets = itemDatas.Where(i => i.itemsPlacedOnSockets.Length == 0).ToList();
            Debug.Log(itemswithoutsockets.Count);
            
            foreach (var item in itemswithsockets)
            {
                if(uids.Contains(item.uid)) continue;
                var go = itemCreator.CreateItem(item.id, item.uid);
                go.transform.position = new Vector3(item.x, 0, item.z);
                gos.Add(go);
                uids.Add(item.uid);

                var sockets = go.transform.GetChild(0).GetComponentsInChildren<Socket>();

                for (int i = 0; i < sockets.Length; i++)
                {
                    Debug.Log(item.itemsPlacedOnSockets[i]);
                    if (item.itemsPlacedOnSockets[i] != 0)
                    {
                        sockets[i].Place(item.itemsPlacedOnSockets[i]);
                        var data = itemDatas.FirstOrDefault(idata => idata.uid == item.itemsPlacedOnSockets[i]);
                        CreateSocketItem(data.id, item.itemsPlacedOnSockets[i], sockets[i]);
                        uids.Add(item.itemsPlacedOnSockets[i]);
                    }
                }
            }
            
            BuildingSystem.BuildingSystem.Current.OnLoadedGame(gos.ToArray());
            
            yield return null;
            LoadedPlayerData.Invoke();
        }
        
        private GameObject CreateSocketItem(string itemId, int uid, Socket currentSocket)
        {
            var socketItem = Instantiate(SocketItem, currentSocket.gameObject.transform, false);

            var component = socketItem.AddComponent<ItemID>();
            component.id = itemId;
            component.uid = uid;
            component.ItemAttributes = shopInventory.ShopItems.FirstOrDefault(i => i.ItemID == itemId)?.Attributes;

            var spriteRenderer = socketItem.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = shopInventory.ShopItems.FirstOrDefault(i => i.ItemID == itemId)?.ItemSprite;

            return socketItem;
        }

        private void OnApplicationQuit()
        {
            SaveGameData();
        }
    }
}