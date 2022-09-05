using System.Collections;
using Hypertonic.GridPlacement;
using Hypertonic.GridPlacement.Models;
using UnityEngine;
using UnityEngine.Events;


public class GameObjectUnityEvent : UnityEvent<GameObject>{}
public class TaliduGridController : MonoBehaviour
{

    public static GameObjectUnityEvent PlaceGameObjectUnityEvent = new GameObjectUnityEvent();
    public static GameObjectUnityEvent DeleteGameObjectUnityEvent = new GameObjectUnityEvent();
    public static GameObjectUnityEvent ModifiyPositionOfGameObjectUnityEvent = new GameObjectUnityEvent();

    public static UnityEvent ConfirmPlacementEvent = new UnityEvent();
    public static UnityEvent CancelPlacementEvent = new UnityEvent();
    
    [SerializeField] private GridManager GridManager;
    [SerializeField] private GridSettings GridSettings;
    [SerializeField] private WebGLInputDefinition WebGLInputDefinition;

    [SerializeField] private GameObject Prefab;

    private void Awake()
    {
        GridSettings.PlatformGridInputsDefinitionMappings.Add(new PlatformGridInputsDefinitionMapping(RuntimePlatform.WebGLPlayer, WebGLInputDefinition));
        GridManager = gameObject.AddComponent<GridManager>();
        GridManager.Setup(GridSettings);
        PlaceGameObjectUnityEvent.AddListener(OnPlaceGameObject);
        DeleteGameObjectUnityEvent.AddListener(OnDeleteGameObject);
        ModifiyPositionOfGameObjectUnityEvent.AddListener(OnModifyPosition);
        ConfirmPlacementEvent.AddListener(OnConfirmPlacement);
        CancelPlacementEvent.AddListener(OnCancelPlacement);
    }
    
    void Start()
    {
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);
        GridManager.EnterPlacementMode(Instantiate(Prefab));
    }

    private void OnModifyPosition(GameObject objectToModify)
    {
        GridManager.ModifyPlacementOfGridObject(objectToModify);
    }

    private void OnDeleteGameObject(GameObject objectToDelete)
    {
        GridManager.DeleteObject(objectToDelete);
    }

    private void OnCancelPlacement()
    {
        GridManager.CancelPlacement();
    }

    private void OnConfirmPlacement()
    {
        GridManager.ConfirmPlacement();
    }

    private void OnPlaceGameObject(GameObject objectToPlace)
    {
        GridManager.EnterPlacementMode(objectToPlace);
    }

}
