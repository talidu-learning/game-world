using Inventory;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Interactables
{
    public class DecorationModeButton : MonoBehaviour
    {
        [SerializeField] private Sprite DecoModeOn;
        [SerializeField] private Sprite DecoModeOff;
    
        public static UnityEvent ToggledDecoModeButtonEvent = new UnityEvent();
        
        private bool isModeEnabled = false;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(ToggleDecoMode);
        }

        private void Start()
        {
            ToggleInventoryButton.OpenedInventoryEvent.AddListener(()=> gameObject.SetActive(false));
            ToggleInventoryButton.ClosedInventoryUnityEvent.AddListener(()=> gameObject.SetActive(true));
            
            SelectionManager.SELECT_OBJECT_EVENT.AddListener(OnSelectedObject);
            SelectionManager.DESELECT_OBJECT_EVENT.AddListener(OnDeselectedObject);
            SelectionManager.DELETE_OBJECT_EVENT.AddListener(OnDeselectedObject);
        }

        private void OnDeselectedObject()
        {
            gameObject.SetActive(true);
        }

        private void OnSelectedObject(Interactable arg0)
        {
            gameObject.SetActive(false);
        }

        public void ToggleDecoMode()
        {
            if (isModeEnabled)
            {
                GetComponent<Image>().sprite = DecoModeOff;
                SelectionManager.DisableDecoration.Invoke();
            }
            else
            {
                GetComponent<Image>().sprite = DecoModeOn;
                SelectionManager.EnableDecoration.Invoke();
            }

            isModeEnabled = !isModeEnabled;
            
            ToggledDecoModeButtonEvent.Invoke();
        }
    }
}