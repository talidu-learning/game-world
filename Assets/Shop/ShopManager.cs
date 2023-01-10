﻿using System;
using System.Linq;
using Enumerations;
using Interactables;
using ServerConnection;
using UnityEngine;
using UnityEngine.Events;

namespace Shop
{
    public class BoolGameObjectUnityEvent : UnityEvent<bool, GameObject>{}
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] private ItemCreator ItemCreator;
        [SerializeField] private ShopInventoryDisplay ShopInventoryDisplay;

        public static StringGuidUnityEvent InitilizePlaceObjectEvent = new StringGuidUnityEvent();

        public static BoolGameObjectUnityEvent OnTriedPlacingGameObjectEvent = new BoolGameObjectUnityEvent();

        private void Awake()
        {
            InitilizePlaceObjectEvent.AddListener(OnPlaceObject);
            OnTriedPlacingGameObjectEvent.AddListener(OnTriedPlacingGameObject);
        }

        private void OnTriedPlacingGameObject(bool wasPlacedSuccessfully, GameObject placedObject)
        {
            var uid = placedObject.GetComponent<ItemID>().uid;
            var id = placedObject.GetComponent<ItemID>().id;

            if (wasPlacedSuccessfully)
            {
                GameAudio.PlaySoundEvent.Invoke(SoundType.Place);
                LocalPlayerData.Instance.OnPlacedItem(uid, placedObject.transform.position.x, placedObject.transform.position.z);
            }
            else
            {
                if (placedObject.GetComponent<SocketCollection>())
                {
                    Debug.Log(placedObject.GetComponent<SocketCollection>());
                    var sockets = placedObject.GetComponent<SocketCollection>().Sockets.Where(s => s.IsUsed);

                    foreach (var socket in sockets)
                    {
                        LocalPlayerData.Instance.OnWithdrewItem(socket.Uid, uid, socket.transform.GetSiblingIndex());
                        socket.Withdraw();
                    }
                    
                }

                LocalPlayerData.Instance.OnWithdrewItem(uid);
                Destroy(placedObject);
            }
            
            ShopInventoryDisplay.OnPlacedItem(id, wasPlacedSuccessfully, LocalPlayerData.Instance.IsItemPlaceable(id));
        }

        private void OnPlaceObject(string itemId, Guid uid)
        {
            if(!LocalPlayerData.Instance.IsItemPlaceable(itemId)) return;
            var go = ItemCreator.CreateItem(itemId, LocalPlayerData.Instance.GetUIDOfUnplacedItem(itemId));
            SelectionManager.SELECT_OBJECT_EVENT.Invoke(go.GetComponent<Interactable>());
        }
    }
}