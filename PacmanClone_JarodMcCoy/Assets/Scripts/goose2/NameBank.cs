using UnityEngine;

public class NameBank : MonoBehaviour
{
    [SerializeField] private string[] prefixes;
    [SerializeField] private string[] suffixes;

    public static NameBank nameBank;

    private void OnEnable()
    {
        nameBank = this;
    }

    public string GetPrefix()
    {
        string prefix = "";

        int roll = Random.Range(0, prefixes.Length);

        prefix = prefixes[roll];

        return prefix;
    }

    public string GetSuffix()
    {
        string suffix = "";

        int roll = Random.Range(0, suffixes.Length);

        suffix = suffixes[roll];

        return suffix;
    }
}
