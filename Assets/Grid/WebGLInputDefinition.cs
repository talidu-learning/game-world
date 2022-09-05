using Hypertonic.GridPlacement.GridInput;
using UnityEngine;

[CreateAssetMenu(fileName = "WebGL Input Definition", menuName = "Grid/WebGL Input Definition")]
public class WebGLInputDefinition : GridInputDefinition
{
    
    public override bool ShouldInteract()
    {
        return Input.GetMouseButtonDown(0);
    }

    public override Vector3? InputPosition()
    {
        return Input.mousePosition;
    }
}
