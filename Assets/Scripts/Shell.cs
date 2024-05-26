using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Shell : MonoBehaviour
{
    [SerializeField] GameObject layerPrefab;
    [SerializeField] Shader shellShader;

    [Range(1, 64), SerializeField] int layerCount = 32;
    [Range(0.1f, 5f), SerializeField] float furLength = 0.5f;
    [Range(1, 512), SerializeField] int density = 256;

    readonly List<Material> layerMaterials = new();

    static readonly int DensityID = Shader.PropertyToID("_Density");
    static readonly int FurLengthID = Shader.PropertyToID("_FurLength");
    static readonly int LayerID = Shader.PropertyToID("_Layer");
    int CurrentLayerCount => transform.childCount;

    void Update()
    {
        var transformUpdate = false;
        if (CurrentLayerCount < layerCount)
        {
            var difference = layerCount - CurrentLayerCount;
            for (int i = 0; i < difference; i++)
            {
                // var instancePosition = new Vector3(0, transform.childCount * offset, 0);
                var layerInstance = Instantiate(layerPrefab, Vector3.zero, Quaternion.identity, transform);
                layerInstance.name = $"Layer_{transform.childCount}";
                layerMaterials.Add(layerInstance.GetComponent<Renderer>().material);
            }

            transformUpdate = true;
        }
        else if (CurrentLayerCount > layerCount)
        {
            var difference = CurrentLayerCount - layerCount;

            for (int i = difference - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
                layerMaterials.RemoveAt(layerMaterials.Count - 1);
            }

            transformUpdate = true;
        }

        if (transformUpdate)
        {
            for (int i = 0; i < layerMaterials.Count; i++)
            {
                layerMaterials[i].SetFloat(LayerID, Mathf.Lerp(0, 1, (float)i / layerMaterials.Count));
            }
        }

        for (int i = 0; i < layerMaterials.Count; i++)
        {
            layerMaterials[i].SetFloat(FurLengthID, furLength);
            layerMaterials[i].SetFloat(DensityID, density);
        }
    }
}