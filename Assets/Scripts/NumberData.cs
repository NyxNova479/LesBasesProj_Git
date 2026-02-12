using UnityEngine;

[CreateAssetMenu(menuName = "Numbers/Number Data")]
public class NumberData : ScriptableObject
{
    [Header("Display")]
    public string numberName;

    [Header("Gameplay Value")]
    [Tooltip("Valeur mathématique du nombre")]
    public int value;

    [Header("Spawn Settings")]
    [Tooltip("Temps avant apparition (utilisé aussi comme valeur si besoin)")]
    public float buildTime;

    [Header("Prefab")]
    public GameObject prefab;
}
