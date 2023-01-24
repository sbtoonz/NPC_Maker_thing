using UnityEngine;
using static NPC_Generator.NPC_Generator;
using Random = UnityEngine.Random;
namespace NPC_Generator.MonoScripts;

public class RandomWeapon: MonoBehaviour
{
    private void Awake()
    {
        var hum = gameObject.GetComponent<Humanoid>();
        var vis = gameObject.GetComponent<VisEquipment>();
        var allItems = ObjectDB.instance.GetAllItems(ItemDrop.ItemData.ItemType.Shield, "Shield");
        hum.m_randomShield = new GameObject[]
        {
            allItems[Random.Range(0, allItems.Count)].gameObject
        };
        vis.SetLeftItem(allItems[Random.Range(0, allItems.Count)].gameObject.name, 0);
        vis.SetLeftHandEquiped(allItems[Random.Range(0, allItems.Count)].gameObject.name.GetStableHashCode(), 0);
        allItems = ObjectDB.instance.GetAllItems(ItemDrop.ItemData.ItemType.OneHandedWeapon, "Axe");
        hum.m_randomWeapon = new[]
        {
            allItems[Random.Range(0, allItems.Count)].gameObject
        };
        vis.SetRightItem(allItems[Random.Range(0, allItems.Count)].gameObject.name);
        vis.SetRightHandEquiped(allItems[Random.Range(0, allItems.Count)].gameObject.name.GetStableHashCode());

        hum.m_faction = Character.Faction.PlainsMonsters;
        hum.m_group = ZNetScene.instance.GetPrefab("Goblin").GetComponent<Humanoid>().m_group;
    }
}