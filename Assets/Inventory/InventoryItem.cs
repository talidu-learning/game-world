using System;
using System.Collections;
using System.Collections.Generic;
using Interactables;
using ServerConnection;
using Shop;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Inventory
{
    public class InventoryItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Unplaced;
        [SerializeField] private Image ItemImage;

        public List<ItemAttribute> attributes{ private set; get; }
        
        public string itemID { private set; get; }

        public void Awake()
        {
            GetComponent<Button>().onClick.AddListener(PlaceItem);
        }

        private void Start()
        {
            SelectionManager.DESELECT_OBJECT_EVENT.AddListener(StartAsyncUpdate);
            SelectionManager.DELETE_OBJECT_EVENT.AddListener(StartAsyncUpdate);
            
            SelectionManager.SELECT_SOCKET_EVENT.AddListener(OnSelectedSocket);
            SelectionManager.DESELECT_SOCKET_EVENT.AddListener(OnDeselectedSocket);
            SelectionManager.DELETE_SOCKET_EVENT.AddListener(OnDeselectedSocket);
            SelectionManager.DisableDecoration.AddListener(OnDeselectedSocket);
            SelectionManager.EnableDecoration.AddListener(OnSelectedSocket);
        }

        private void OnDeselectedSocket(Socket socket)
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(PlaceItem);
        }
        private void OnDeselectedSocket()
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(PlaceItem);
        }

        private void OnSelectedSocket(Socket socket)
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(PlaceOnSocket);
        }
        private void OnSelectedSocket()
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(PlaceOnSocket);
        }

        private void StartAsyncUpdate()
        {
            gameObject.SetActive(true);
            StartCoroutine(AsyncUpdate());
        }
        
        private IEnumerator AsyncUpdate()
        {
            yield return null;
            UpdateUI();
        }

        private void PlaceItem()
        {
            var uitemID = LocalPlayerData.Instance.GetUidOfUnplacedItem(itemID);
            ShopManager.InitilizePlaceObjectEvent.Invoke(itemID, uitemID);
            ToggleInventoryButton.CloseInventoryUnityEvent.Invoke();
        }

        private void PlaceOnSocket()
        {
            ToggleInventoryButton.CloseInventoryUnityEvent.Invoke();
            SocketPlacement.PlaceItemOnSocket.Invoke(itemID);
        }
        
        public void Initialize(ShopItemData shopItemData)
        {
            itemID = shopItemData.ItemID;
            ItemImage.sprite = shopItemData.ItemSprite;
            attributes = shopItemData.Attributes;
            UpdateUI();
        }
        
        public void UpdateUI()
        {
            int unplaced = LocalPlayerData.Instance.GetCountOfUnplacedItems(itemID);

            Unplaced.text = unplaced.ToString();
            
            if(unplaced != 0) gameObject.SetActive(true);
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}