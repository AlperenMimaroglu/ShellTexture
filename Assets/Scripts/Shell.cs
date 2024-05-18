using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] GameObject layerPrefab;
    [SerializeField] Shader shellShader;

    [Range(1, 32), SerializeField] int layerCount;
    [Range(0, 1), SerializeField] float offset = 0.1f;
    List<Material> layerMaterials = new();

    static readonly int SizeID = Shader.PropertyToID("_Size");
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
                var instancePosition = new Vector3(0, transform.childCount * offset, 0);
                var layerInstance = Instantiate(layerPrefab, instancePosition, Quaternion.identity, transform);
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
    }

    void OnValidate()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var newPosition = transform.GetChild(i).transform.position;
            newPosition.y = i * offset;
            transform.GetChild(i).transform.position = newPosition;
        }
    }
}