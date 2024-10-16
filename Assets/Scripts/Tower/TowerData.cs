using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Data", menuName = "ScriptableObjects/Tower Data")]
public class TowerData : ScriptableObject
{
    public string towerName;
    public float damage;
    public float range;
    public int cost;
    public float upgradeScale;
    public float fireRate;

    [Tooltip("The types of bases for each level of the tower")]
    public Mesh[] BaseMeshes;
    public Mesh[] WeaponMeshes;
    public Projectile projectile;
}
