using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldObject : MonoBehaviour
{
    public World world = new World();
    public TextMeshProUGUI worldName;
    public TextMeshProUGUI worldInfo;
    [SerializeField] Button OpenWordButton;

    private void OnEnable()
    {
        OpenWordButton.onClick.AddListener(OpenWorld);
    }
    private void OpenWorld()
    {
        WorldGenerator.StartWorldGeneratorLoad(gameObject.GetComponent<WorldObject>().world);
    }
}
