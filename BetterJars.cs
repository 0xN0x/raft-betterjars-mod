using System.Reflection;
using Harmony;
using Steamworks;

[ModTitle("BetterJars")]
[ModDescription("Give your glass back when you use honey jars.")]
[ModAuthor("Nundir")]
[ModIconUrl("https://github.com/Nundir/raft-betterjars-mod/raw/master/BetterJars_Icon.png")]
[ModWallpaperUrl("https://github.com/Nundir/raft-betterjars-mod/raw/master/BetterJars_Banner.png")]
[ModVersionCheckUrl("https://github.com/Nundir/raft-betterjars-mod/raw/master/version.txt")]
[ModVersion("1.0.1")]
[RaftVersion("10")]
public class GlassHoneyUse : Mod
{
    private const string logPrefix = "[<color=#0000ff>BetterJars</color>] ";
    private const string HARMONY_ID = "com.nundir.betterjars";
    private HarmonyInstance harmonyInstance;

    public void Start()
    {
        ComponentManager<GlassHoneyUse>.Value = this;

        harmonyInstance = HarmonyInstance.Create(HARMONY_ID);
        harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

        RConsole.Log(logPrefix + " loaded!");
    }

    public void OnModUnload()
    {
        RConsole.Log(logPrefix + " unloaded!");
        Destroy(gameObject);
    }

    public void OnModifyTank(Network_Player player, float amount, Item_Base itemType = null)
    {
        if (!itemType || !(itemType.name == "Jar_Honey")) return;

        GiveGlass();
    }

    public void OnConsumeItem(Item_Base item)
    {
        if (!(item.name == "Jar_Honey")) return;

        GiveGlass();
    }

    public void GiveGlass()
    {
        RAPI.GetLocalPlayer().Inventory.AddItem(ItemManager.GetItemByName("Glass").UniqueName, 1);
    }
}

[HarmonyPatch(typeof(Tank)), HarmonyPatch("ModifyTank")]
internal class ModifyTankPatch
{
    private static void Postfix(Tank __instance, Network_Player player, float amount, Item_Base itemType = null)
    {
        if (__instance.GetComponent(typeof(Network_Player)) == RAPI.GetLocalPlayer())
        {
            ComponentManager<GlassHoneyUse>.Value.OnModifyTank(player, amount, itemType);
        }
    }
}

[HarmonyPatch(typeof(PlayerStats)), HarmonyPatch("Consume")]
internal class ConsumePatch
{
    private static void Postfix(PlayerStats __instance, Item_Base edibleItem)
    {
        if (__instance.GetComponent(typeof(Network_Player)) == RAPI.GetLocalPlayer())
        {
            ComponentManager<GlassHoneyUse>.Value.OnConsumeItem(edibleItem);
        }
    }
}