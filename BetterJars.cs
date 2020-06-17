using System.Reflection;
using Harmony;

[ModTitle("BetterJars")]
[ModDescription("Give your glass back when you use honey jars.")]
[ModAuthor("Nundir")]
[ModIconUrl("https://github.com/Nundir/raft-betterjars-mod/raw/master/BetterJars_Icon.png")]
[ModWallpaperUrl("https://github.com/Nundir/raft-betterjars-mod/raw/master/BetterJars_Banner.png")]
[ModVersionCheckUrl("https://github.com/Nundir/raft-betterjars-mod/raw/master/version.txt")]
[ModVersion("1.0.0")]
[RaftVersion("10")]
public class GlassHoneyUse : Mod
{
    private const string logPrefix = "[<color=#0000ff>BetterJars</color>] ";
    private const string HARMONY_ID = "com.nundir.betterjars";
    private HarmonyInstance harmonyInstance;
    private PlayerInventory inventory;

    public void Start()
    {
        ComponentManager<GlassHoneyUse>.Value = this;

        harmonyInstance = HarmonyInstance.Create(HARMONY_ID);
        harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

        RConsole.Log(logPrefix + "BetterJars loaded!");
    }

    public void OnModUnload()
    {
        RConsole.Log(logPrefix + "BetterJars unloaded!");
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
        // make sure player inventory is available
        if (this.inventory == null) 
            this.inventory = ComponentManager<PlayerInventory>.Value;

        inventory.AddItem(ItemManager.GetItemByName("Glass").UniqueName, 1);
    }
}

[HarmonyPatch(typeof(Tank)), HarmonyPatch("ModifyTank")]
internal class ModifyTankPatch
{
    private static void Postfix(Network_Player player, float amount, Item_Base itemType = null)
    {
        ComponentManager<GlassHoneyUse>.Value.OnModifyTank(player, amount, itemType);
    }
}

[HarmonyPatch(typeof(PlayerStats)), HarmonyPatch("Consume")]
internal class ConsumePatch
{
    private static void Postfix(Item_Base edibleItem)
    {
        ComponentManager<GlassHoneyUse>.Value.OnConsumeItem(edibleItem);
    }
}