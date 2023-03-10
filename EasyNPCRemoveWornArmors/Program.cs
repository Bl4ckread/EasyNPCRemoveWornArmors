using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;

namespace EasyNPCRemoveWornArmors
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "NoWornArmor.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            foreach (var actorGetter in state.LoadOrder.PriorityOrder.Npc().WinningContextOverrides())
            {
                var actor = actorGetter.Record;
                var wornArmor = actor.WornArmor;
                wornArmor.TryResolve<IArmorGetter>(state.LinkCache, out var armor);
                wornArmor.TryGetModKey(out var wornArmorKey);
                if (!wornArmorKey.Equals("NPC Appearances Merged.esp") || armor == null || armor.EditorID == null || !armor.EditorID.Equals("SkinNakedPatched"))
                {
                    continue;
                }
                var modifiedActor = actorGetter.GetOrAddAsOverride(state.PatchMod);
                modifiedActor.WornArmor.Clear();
            }
        }
    }
}
