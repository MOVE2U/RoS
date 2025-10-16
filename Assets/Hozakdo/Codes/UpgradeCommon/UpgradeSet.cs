using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeSet", menuName = "Scriptable Objects/UpgradeSet")]
public class UpgradeSet : ScriptableObject
{
    public List<UpgradeData> upgradeDatas;
}
