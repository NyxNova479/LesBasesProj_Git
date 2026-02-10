using UnityEngine;

[CreateAssetMenu(menuName = "Numbers/Number Data")]
public class NumberData : ScriptableObject
{
    public string numberName;
    public int buildTime;
    public GameObject prefab;
}
