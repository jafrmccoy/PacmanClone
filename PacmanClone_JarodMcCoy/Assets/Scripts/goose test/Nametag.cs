using TMPro;
using UnityEngine;

public class Nametag : MonoBehaviour
{
    public Nametag Init(Goose gooseParent, string newPrefix, string newSuffix, GameObject prefab)
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        targetObj = gooseParent.gameObject;
        prefix = newPrefix;
        suffix = newSuffix;
        CombineNames();

        return this;
    }

    public Nametag Init(GameObject goose, string newPrefix, string newSuffix, GameObject prefab)
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        targetObj = goose;
        prefix = newPrefix;
        suffix = newSuffix;
        CombineNames();

        return this;
    }

    private string prefix;
    private string suffix;
    private string fullName;
    private TextMeshProUGUI text;

    private GameObject targetObj;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        transform.position = mainCamera.WorldToScreenPoint(targetObj.transform.position);

    }

    private void Update()
    {
        if (text.text != fullName)
        {
            text.text = fullName;
        }


        if (targetObj == null)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position = mainCamera.WorldToScreenPoint(targetObj.transform.position);
        }
    }

    public void CombineNames()
    {
        fullName = prefix + suffix;
    }
}
