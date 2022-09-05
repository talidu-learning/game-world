using System;
using System.Linq;
using Hypertonic.GridPlacement;
using Hypertonic.GridPlacement.Models;
using UnityEngine;

namespace Grid
{
    [CreateAssetMenu(fileName = "Custom Grid Settings", menuName = "Grid/Custom Grid Settings")]
    public class CustomGridSettings : GridSettings
    {
        [SerializeField] private WebGLInputDefinition WebGLInputDefinition;
        
        public void AddWebGLSettings()
        {
            PlatformGridInputsDefinitionMappings.Add(new PlatformGridInputsDefinitionMapping(RuntimePlatform.WebGLPlayer, WebGLInputDefinition));
        }

    }
}