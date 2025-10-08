namespace Atlas
{
    using GameHelper.Plugin;
    using System.Collections.Generic;
    using System.Numerics;

    public sealed class AtlasSettings : IPSettings
    {
        public Vector4 DefaultBackgroundColor = new(0.0f, 0.0f, 0.0f, 0.4f);
        public Vector4 DefaultFontColor = new(1.0f, 1.0f, 1.0f, 1.0f);
        public bool HideCompletedMaps = true;
        public float ScaleMultiplier = 1.0f;
        public float PositionOffsetX = 0.0f;
        public float PositionOffsetY = 0.0f;
        public List<MapGroupSettings> MapGroups = [];
        public string GroupNameInput = string.Empty;

        public bool TrackAbyssMaps = false;
        public bool TrackAbyssRevealedOnly = true;

        public AtlasSettings()
        {
            var citadels = new MapGroupSettings("Citadels", new Vector4(1.0f, 0.0f, 0.0f, 0.4f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
            citadels.Maps.Add("The Copper Citadel");
            citadels.Maps.Add("The Iron Citadel");
            citadels.Maps.Add("The Stone Citadel");

            var specials = new MapGroupSettings("High Value Uniques", new Vector4(1.0f, 0.5f, 0.0f, 0.4f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
            specials.Maps.Add("Silent Cave");
            specials.Maps.Add("The Jade Isles");
            specials.Maps.Add("The Viridian Wildwood");

            var good = new MapGroupSettings("Good", new Vector4(0.0f, 1.0f, 0.0f, 0.4f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
            good.Maps.Add("Burial Bog");
            good.Maps.Add("Confluence");
            good.Maps.Add("Creek");
            good.Maps.Add("Rustbowl");
            good.Maps.Add("Sandspit");
            good.Maps.Add("Savannah");
            good.Maps.Add("Steaming Springs");
            good.Maps.Add("Steppe");
            good.Maps.Add("Wetlands");
            good.Maps.Add("Willow");

            var ok = new MapGroupSettings("Ok", new Vector4(1.0f, 1.0f, 0.0f, 0.2f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
            ok.Maps.Add("Backwash");
            ok.Maps.Add("Bloodwood");
            ok.Maps.Add("Blooming Field");
            ok.Maps.Add("Cenotes");
            ok.Maps.Add("Crimson Shores");
            ok.Maps.Add("Decay");
            ok.Maps.Add("Fortress");
            ok.Maps.Add("Hidden Grotto");
            ok.Maps.Add("Hive");
            ok.Maps.Add("Oasis");
            ok.Maps.Add("Ravine");
            ok.Maps.Add("Riverside");
            ok.Maps.Add("Spider Woods");
            ok.Maps.Add("Sulphuric Caverns");
            ok.Maps.Add("Sump");

            var bad = new MapGroupSettings("Bad", new Vector4(0.0f, 0.0f, 0.0f, 0.2f), new Vector4(1.0f, 1.0f, 1.0f, 0.4f));
            bad.Maps.Add("Abyss");
            bad.Maps.Add("Augury");
            bad.Maps.Add("Channel");
            bad.Maps.Add("Crypt");
            bad.Maps.Add("Deserted");
            bad.Maps.Add("Factory");
            bad.Maps.Add("Forge");
            bad.Maps.Add("Foundry");
            bad.Maps.Add("Gothic City");
            bad.Maps.Add("Lofty Summit");
            bad.Maps.Add("Mineshaft");
            bad.Maps.Add("Mire");
            bad.Maps.Add("Necropolis");
            bad.Maps.Add("Penitentiary");
            bad.Maps.Add("Seepage");
            bad.Maps.Add("Sun Temple");
            bad.Maps.Add("Vaal City");
            bad.Maps.Add("Woodland");

            MapGroups.Add(citadels);
            MapGroups.Add(specials);
            MapGroups.Add(good);
            MapGroups.Add(ok);
            MapGroups.Add(bad);
        }
    }

    public class MapGroupSettings(string name, Vector4 backgroundColor, Vector4 fontColor)
    {
        public string Name = name;
        public Vector4 BackgroundColor = backgroundColor;
        public Vector4 FontColor = fontColor;
        public List<string> Maps = [];
        public string MapNameInput = string.Empty;
    }
}
