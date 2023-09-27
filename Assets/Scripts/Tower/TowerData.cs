using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Data", menuName = "ScriptableObjects/Tower Data")]
public class TowerData : ScriptableObject
{
    public string towerName;
    public int damage;
    public float range;
    public int cost;
    [Tooltip("The types of bases for each level of the tower")]
    public GameObject[] BaseMeshes;
    public GameObject[] WeaponMeshes;
    public Projectile projectile;

}
