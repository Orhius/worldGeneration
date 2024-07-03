using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EquatorWidthChanger : MonoBehaviour
{
    private TextureGenerator generator;

    [SerializeField] private Button button;

    private void OnEnable()
    {
        button.onClick.AddListener(ChanngeEquatorWidth);
    }
    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }
    private void Awake()
    {
        generator = GetComponent<TextureGenerator>();
    }
    public void ChanngeEquatorWidth()
    {
        generator.EquatorWidth = generator.newEquatorWidth;
        generator.GenerateHeatTexture();
    }
}
