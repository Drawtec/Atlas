namespace Atlas
{
    using GameHelper;
    using GameHelper.Plugin;
    using GameHelper.Settings;
    using GameHelper.RemoteObjects.Components;
    using GameHelper.Utils;
    using ImGuiNET;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Numerics;
    using System.Runtime.InteropServices;
    using System.Text;

    public sealed class Atlas : PCore<AtlasSettings>
    {
        private string SettingPathname => Path.Join(this.DllDirectory, "config", "settings.txt");
        private string NewGroupName = string.Empty;
        private string Search = string.Empty;

        public override void OnDisable()
        {
        }

        public override void OnEnable(bool isGameOpened)
        {
            if (File.Exists(this.SettingPathname))
            {
                var content = File.ReadAllText(this.SettingPathname);
                var serializerSettings = new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace };
                this.Settings = JsonConvert.DeserializeObject<AtlasSettings>(content, serializerSettings);
            }
        }

        public override void SaveSettings()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(this.SettingPathname));
            var settingsData = JsonConvert.SerializeObject(this.Settings, Formatting.Indented);
            File.WriteAllText(this.SettingPathname, settingsData);
        }

        public override void DrawSettings()
        {
            /* ImGui.Checkbox($"##ControllerMode", ref Settings.ControllerMode);
             ImGui.SameLine();
             ImGui.Text($"ControllerMode"); */
            #region Colorsettings
            ImGui.InputText($"##Search", ref Search, 256);
            ImGui.SameLine();
            ImGui.Text($"Search");

            ImGui.Checkbox($"##HideCompletedMaps", ref Settings.HideCompletedMaps);
            ImGui.SameLine();
            ImGui.Text($"Hide Completed Maps");
            ImGui.SameLine();
            ImGui.Checkbox($"##HideFailedMaps", ref Settings.HideFailedMaps);
            ImGui.SameLine();
            ImGui.Text($"Hide Failed Maps");

            ColorSwatch($"##DefaultBackgroundColor", ref Settings.DefaultBackgroundColor);
            ImGui.SameLine();
            ImGui.Text($"Default Background Color");
            ImGui.SameLine();
            ColorSwatch($"##DefaultFontColor", ref Settings.DefaultFontColor);
            ImGui.SameLine();
            ImGui.Text($"Default Font Color");

            ImGui.Checkbox($"##ShowMapFlags", ref Settings.ShowMapFlags);
            ImGui.SameLine();
            ImGui.Text($"Show Map Flags");

            ColorSwatch($"##BossBackgroundColor", ref Settings.DefaultBackgroundColor);
            ImGui.SameLine();
            ImGui.Text($"Boss Background Color");
            ImGui.SameLine();
            ColorSwatch($"##BossFontColor", ref Settings.DefaultFontColor);
            ImGui.SameLine();
            ImGui.Text($"Boss Font Color");

            ColorSwatch($"##BreachBackgroundColor", ref Settings.DefaultBackgroundColor);
            ImGui.SameLine();
            ImGui.Text($"Breach Background Color");
            ImGui.SameLine();
            ColorSwatch($"##BreachFontColor", ref Settings.DefaultFontColor);
            ImGui.SameLine();
            ImGui.Text($"Breach Font Color");

            ColorSwatch($"##DeliriumBackgroundColor", ref Settings.DeliriumBackgroundColor);
            ImGui.SameLine();
            ImGui.Text($"Delirium Background Color");
            ImGui.SameLine();
            ColorSwatch($"##DeliriumFontColor", ref Settings.DeliriumFontColor);
            ImGui.SameLine();
            ImGui.Text($"Delirium Font Color");

            ColorSwatch($"##ExpiditionBackgroundColor", ref Settings.ExpiditionBackgroundColor);
            ImGui.SameLine();
            ImGui.Text($"Expidition Background Color");
            ImGui.SameLine();
            ColorSwatch($"##ExpiditionFontColor", ref Settings.ExpiditionFontColor);
            ImGui.SameLine();
            ImGui.Text($"Expidition Font Color");

            ColorSwatch($"##RitualBackgroundColor", ref Settings.RitualBackgroundColor);
            ImGui.SameLine();
            ImGui.Text($"Ritual Background Color");
            ImGui.SameLine();
            ColorSwatch($"##RitualFontColor", ref Settings.RitualFontColor);
            ImGui.SameLine();
            ImGui.Text($"Ritual Font Color");

            ImGui.SliderFloat("##ScaleMultiplier", ref Settings.ScaleMultiplier, 0.5f, 2.0f);
            ImGui.SameLine();
            ImGui.Text($"Scale Multiplier");

            ImGui.SliderFloat("##XSlider", ref Settings.XSlider, 0.0f, 3000.0f);
            ImGui.SameLine();
            ImGui.Text($"Move X Axis");
            ImGui.SliderFloat("##YSlider", ref Settings.YSlider, 0.0f, 3000.0f);
            ImGui.SameLine();
            ImGui.Text($"Move Y Axis");

            ImGui.InputText($"##MapGroupName", ref Settings.GroupNameInput, 256);
            ImGui.SameLine();
            if (ImGui.Button("Add new map group"))
            {
                Settings.MapGroups.Add(new MapGroupSettings(Settings.GroupNameInput, Settings.DefaultBackgroundColor, Settings.DefaultFontColor));
                Settings.GroupNameInput = string.Empty;
            }

            for (int i = 0; i < Settings.MapGroups.Count; i++)
            {
                var mapGroup = Settings.MapGroups[i];

                if (ImGui.CollapsingHeader($"{mapGroup.Name}##MapGroup{i}"))
                {
                    float buttonSize = ImGui.GetFrameHeight();

                    if (TriangleButton($"##Up{i}", buttonSize, new Vector4(1, 1, 1, 1), true)) { MoveMapGroup(i, -1); }
                    ImGui.SameLine();
                    if (TriangleButton($"##Down{i}", buttonSize, new Vector4(1, 1, 1, 1), false)) { MoveMapGroup(i, 1); }
                    ImGui.SameLine();
                    if (ImGui.Button($"Rename Group##{i}")) { NewGroupName = mapGroup.Name; ImGui.OpenPopup($"RenamePopup##{i}"); }
                    ImGui.SameLine();
                    if (ImGui.Button($"Delete Group##{i}")) { DeleteMapGroup(i); }
                    ImGui.SameLine();
                    ColorSwatch($"##MapGroupBackgroundColor{i}", ref mapGroup.BackgroundColor);
                    ImGui.SameLine();
                    ImGui.Text($"Background Color");
                    ImGui.SameLine();
                    ColorSwatch($"##MapGroupFontColor{i}", ref mapGroup.FontColor);
                    ImGui.SameLine();
                    ImGui.Text($"Font Color");

                    for (int j = 0; j < mapGroup.Maps.Count; j++)
                    {
                        var mapName = mapGroup.Maps[j];

                        if (ImGui.InputText($"##MapName{i}-{j}", ref mapName, 256)) mapGroup.Maps[j] = mapName;

                        ImGui.SameLine();
                        if (ImGui.Button($"Delete##MapNameDelete{i}-{j}"))
                        {
                            mapGroup.Maps.RemoveAt(j);
                            break;
                        }
                    }

                    if (ImGui.Button($"Add new map##AddNewMap{i}"))
                        mapGroup.Maps.Add(string.Empty);

                    if (ImGui.BeginPopupModal($"RenamePopup##{i}", ImGuiWindowFlags.AlwaysAutoResize))
                    {
                        ImGui.InputText("New Name", ref NewGroupName, 256);
                        if (ImGui.Button("OK")) { mapGroup.Name = NewGroupName; ImGui.CloseCurrentPopup(); }
                        ImGui.SameLine();
                        if (ImGui.Button("Cancel")) ImGui.CloseCurrentPopup();
                        ImGui.EndPopup();
                    }
                }
            }
            #endregion
        }

        public override void DrawUI()
        {
            var rightPanel = RightPanel();
            var isGameHelperForeground = Process.GetCurrentProcess().MainWindowHandle == GetForegroundWindow();
            if (!Core.Process.Foreground && !isGameHelperForeground) return;
            
            if (ProcessHandle == 0)
                ProcessHandle = ProcessMemoryUtilities.Managed.NativeWrapper.OpenProcess(ProcessMemoryUtilities.Native.ProcessAccessFlags.Read, (int)Core.Process.Pid);
            var controllerMode = Core.GHSettings.EnableControllerMode;
            var player = Core.States.InGameStateObject.CurrentAreaInstance.Player;
            if (!player.TryGetComponent<Render>(out var playerRender)) return;
            var drawList = ImGui.GetBackgroundDrawList();
            var atlasNodes =new List<AtlasNode>();
            var atlasNodesController = GetAtlasNodesController();
            var playerlocation = Core.States.InGameStateObject.CurrentWorldInstance.WorldToScreen(playerRender.WorldPosition);
            if (rightPanel) return;
            if(controllerMode == true)
            {
               atlasNodes = GetAtlasNodesController();
            }
            else
            {
              atlasNodes = GetAtlasNodes();
            }

            foreach (var atlasNode in atlasNodes)
            {
                var mapName = atlasNode.MapName;
                if (string.IsNullOrWhiteSpace(mapName)) continue;
                if (Settings.HideCompletedMaps && (atlasNode.IsCompleted)) continue;
                if (Settings.HideFailedMaps && (atlasNode.IsFailedAttempt)) continue;


                var hasBoss = atlasNode.HasBoss;
                var hasBreach = atlasNode.HasBreach;
                var hasDelirium = atlasNode.HasDelirium;
                var hasExpidition = atlasNode.HasExpedition;
                var hasRitual = atlasNode.HasRitual;
              
                var textSize = ImGui.CalcTextSize(mapName);
                var backgroundColor = Settings.MapGroups.Find(group => group.Maps.Exists(map => map.Equals(mapName, StringComparison.OrdinalIgnoreCase)))?.BackgroundColor ?? Settings.DefaultBackgroundColor;
                var fontColor = Settings.MapGroups.Find(group => group.Maps.Exists(map => map.Equals(mapName, StringComparison.OrdinalIgnoreCase)))?.FontColor ?? Settings.DefaultFontColor;

                var showMapFlags = Settings.ShowMapFlags;
                var mapPosition = atlasNode.Position * Settings.ScaleMultiplier + new Vector2(25, 0);
                var positionSlider = new Vector2(Settings.XSlider - 1500, Settings.YSlider - 1500);
                var drawPosition = (mapPosition - textSize / 2) + positionSlider;
                var drawFlagPosition = drawPosition - new Vector2 (-10 , - 32);
                var padding = new Vector2(5, 2);
                var fpadding = new Vector2(4, 10);
                var bgPos = drawPosition - padding;
                var bgFlagPos = drawFlagPosition - fpadding;
                var bgSize = textSize + padding * 2;

                drawList.AddRectFilled(bgPos, bgPos + bgSize, ImGuiHelper.Color(backgroundColor));
                drawList.AddText(drawPosition, ImGuiHelper.Color(fontColor), mapName);
                


                
                
                
                
                
               
                if (showMapFlags)
                {
                    var i = 0;
                    var r = 11;
                    if (hasBoss)
                    {
                        drawList.AddCircleFilled(drawFlagPosition, r, ImGuiHelper.Color(Settings.BossBackgroundColor));
                        drawList.AddText(bgFlagPos, ImGuiHelper.Color(Settings.BossFontColor), "B");
                        i = i + 2;
                    }
                    if (hasBreach)
                    {
                        drawList.AddCircleFilled(drawFlagPosition + new Vector2(r * i + 1, 0), r, ImGuiHelper.Color(Settings.BreachBackgroundColor));
                        drawList.AddText(bgFlagPos + new Vector2(r * i + 1, 0), ImGuiHelper.Color(Settings.BreachFontColor), "B");
                        i = i + 2;
                    }
                    if (hasDelirium)
                    {
                        drawList.AddCircleFilled(drawFlagPosition + new Vector2(r * i + 1, 0), r, ImGuiHelper.Color(Settings.DeliriumBackgroundColor));
                        drawList.AddText(bgFlagPos + new Vector2(r * i + 1, 0), ImGuiHelper.Color(Settings.DeliriumFontColor), "D");
                        i = i + 2;
                    }
                    if (hasExpidition)
                    {
                        drawList.AddCircleFilled(drawFlagPosition + new Vector2(r * i + 1, 0), r, ImGuiHelper.Color(Settings.ExpiditionBackgroundColor));
                        drawList.AddText(bgFlagPos + new Vector2(r * i + 1, 0), ImGuiHelper.Color(Settings.ExpiditionFontColor), "E");
                        i = i + 2;
                    }
                    if (hasRitual)
                    {
                        drawList.AddCircleFilled(drawFlagPosition + new Vector2(r * i + 1, 0), r, ImGuiHelper.Color(Settings.RitualBackgroundColor));
                        drawList.AddText(bgFlagPos + new Vector2(r * i + 1, 0), ImGuiHelper.Color(Settings.RitualFontColor), "R");
                        i = i + 2;
                    }
                }

                if (!string.IsNullOrWhiteSpace(Search) && mapName.Contains(Search, StringComparison.OrdinalIgnoreCase))
                    drawList.AddLine(playerlocation, drawPosition, 0xFFFFFFFF);
            }
        }

        private static void ColorSwatch(string label, ref System.Numerics.Vector4 color)
        {

            if (ImGui.ColorButton(label, color))
                ImGui.OpenPopup(label);

            if (ImGui.BeginPopup(label))
            {
                ImGui.ColorPicker4(label, ref color);
                ImGui.EndPopup();
            }

            if (!label.StartsWith("##"))
            {
                ImGui.SameLine();
                ImGui.Text(label);
            }
        }

        private static bool TriangleButton(string id, float buttonSize, Vector4 color, bool isUp)
        {
            var pressed = ImGui.Button($"{id}", new Vector2(buttonSize, buttonSize));
            var drawList = ImGui.GetWindowDrawList();
            var pos = ImGui.GetItemRectMin();
            var triangleSize = buttonSize * 0.5f;
            var center = new Vector2(pos.X + buttonSize * 0.5f, pos.Y + buttonSize * 0.5f);
            Vector2 p1, p2, p3;

            if (isUp)
            {
                p1 = new Vector2(center.X, center.Y - triangleSize * 0.5f);
                p2 = new Vector2(center.X - triangleSize * 0.5f, center.Y + triangleSize * 0.5f);
                p3 = new Vector2(center.X + triangleSize * 0.5f, center.Y + triangleSize * 0.5f);
            }
            else
            {
                p1 = new Vector2(center.X - triangleSize * 0.5f, center.Y - triangleSize * 0.5f);
                p2 = new Vector2(center.X + triangleSize * 0.5f, center.Y - triangleSize * 0.5f);
                p3 = new Vector2(center.X, center.Y + triangleSize * 0.5f);
            }

            drawList.AddTriangleFilled(p1, p2, p3, ImGuiHelper.Color(color));
            return pressed;
        }

        private void MoveMapGroup(int index, int direction)
        {
            if (index < 0 || index >= Settings.MapGroups.Count || index + direction < 0 || index + direction >= Settings.MapGroups.Count)
                return;

            var item = Settings.MapGroups[index];
            Settings.MapGroups.RemoveAt(index);
            Settings.MapGroups.Insert(index + direction, item);
        }

        private void DeleteMapGroup(int index)
        {
            if (index < 0 || index >= Settings.MapGroups.Count)
                return;

            Settings.MapGroups.RemoveAt(index);
        }

        // ==================================================
        // TEMPORARY CODE
        // ==================================================
        public static IntPtr ProcessHandle { get; set; }


        public static T Read<T>(IntPtr address) where T : unmanaged
        {
            if (address == IntPtr.Zero) return default;

            T result = default;
            ProcessMemoryUtilities.Managed.NativeWrapper.ReadProcessMemory(ProcessHandle, address, ref result);
            return result;
        }

        public static string ReadWideString(nint address, int stringLength)
        {
            if (address == IntPtr.Zero || stringLength <= 0) return string.Empty;

            byte[] result = new byte[stringLength * 2];
            ProcessMemoryUtilities.Managed.NativeWrapper.ReadProcessMemoryArray(ProcessHandle, address, result);
            return Encoding.Unicode.GetString(result).Split('\0')[0];
        }

        
        
        private static List<AtlasNode> GetAtlasNodes()
        {
            
            var nodes = new List<AtlasNode>();
            var uiElement = Atlas.Read<UiElement>(Core.States.InGameStateObject.GameUi.Address);
     
            
            uiElement = uiElement.GetChild(24);
            uiElement = uiElement.GetChild(0);
            uiElement = uiElement.GetChild(6);
            

            if (!uiElement.IsVisible || uiElement.FirstChild == IntPtr.Zero || uiElement.Length > 10000) return nodes;

            for (var i = 0; i < uiElement.Length; i++)
                nodes.Add(uiElement.GetAtlasNode(i));

            return nodes;
        }
        private static List<AtlasNode> GetAtlasNodesController()
        {

            var nodes = new List<AtlasNode>();
            var uiElement = Atlas.Read<UiElement>(Core.States.InGameStateObject.GameUi.Address);


            uiElement = uiElement.GetChild(17);
            uiElement = uiElement.GetChild(2);
            uiElement = uiElement.GetChild(3);
            uiElement = uiElement.GetChild(0);
            uiElement = uiElement.GetChild(0);
            uiElement = uiElement.GetChild(6);

            if (!uiElement.IsVisible || uiElement.FirstChild == IntPtr.Zero || uiElement.Length > 10000) return nodes;

            for (var i = 0; i < uiElement.Length; i++)
                nodes.Add(uiElement.GetAtlasNode(i));

            return nodes;
        }

       private static bool RightPanel()
        {
            var uiElement = Atlas.Read<UiElement>(Core.States.InGameStateObject.GameUi.Address);

            var rightPanel = uiElement.GetChild(33);
            return rightPanel.IsVisible;

        }

        [DllImport("user32.dll")]
        private static extern nint GetForegroundWindow();
    }
}
