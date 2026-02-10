using UnityEngine;

[CreateAssetMenu(menuName = "Numbers/Number Data")]
public class NumberData : ScriptableObject
{
    public string numberName;
    public float buildTime;
    public GameObject prefab;
}
