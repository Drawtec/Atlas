using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Atlas
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct UiElement
    {
        [FieldOffset(0x38)] public IntPtr FirstChild;
        [FieldOffset(0x40)] public IntPtr LastChild;
        [FieldOffset(0x1B8)] public uint Flags;

        public readonly int Length => (int)(this.LastChild.ToInt64() - this.FirstChild.ToInt64()) / 0x8;

        public readonly bool IsVisible => (this.Flags & (1 << 0x0B)) != 0;

        public readonly UiElement GetChild(int index)
        {
            var address = Atlas.Read<IntPtr>(FirstChild + (index * 0x8));
            return Atlas.Read<UiElement>(address);
        }

        public readonly AtlasNode GetAtlasNode(int index)
        {
            var address = Atlas.Read<IntPtr>(FirstChild + (index * 0x8));
            return Atlas.Read<AtlasNode>(address);
        }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct AtlasNode
    {
        [FieldOffset(0x110)] public Vector2 RelativePosition;
        [FieldOffset(0x12C)] public float Zoom;
        [FieldOffset(0x270)] public IntPtr NodeNameAddress;
        [FieldOffset(0x290)] public AtlasNodeState Flags;

        public readonly float Scale => Zoom / 1.5f;
        public readonly Vector2 Position => RelativePosition * Scale;

        public readonly bool IsAttempted => Flags.HasFlag(AtlasNodeState.Attempted);
        public readonly bool IsPristine => Flags.HasFlag(AtlasNodeState.Pristine);
        public readonly bool IsWatchTower => Flags.HasFlag(AtlasNodeState.WatchTower);
        public readonly bool HasCorruption => Flags.HasFlag(AtlasNodeState.Corruption);
        public readonly bool HasBoss => Flags.HasFlag(AtlasNodeState.Boss);
        public readonly bool HasBreach => Flags.HasFlag(AtlasNodeState.Breach);
        public readonly bool HasExpedition => Flags.HasFlag(AtlasNodeState.Expedition);
        public readonly bool HasDelirium => Flags.HasFlag(AtlasNodeState.Delirium);
        public readonly bool HasRitual => Flags.HasFlag(AtlasNodeState.Ritual);
        public readonly bool IsIrradiated => Flags.HasFlag(AtlasNodeState.Irradiated);
        public readonly bool IsCompleted => IsAttempted && IsPristine;
        public readonly bool IsFailedAttempt => IsAttempted && !IsPristine;

        public string MapName
        {
            get
            {
                var address = Atlas.Read<IntPtr>(NodeNameAddress + 0x8);
                return Atlas.ReadWideString(address, 64);
            }
        }
    }

    [Flags]
    public enum AtlasNodeState : ushort
    {
        None = 0,
        Attempted = 1 << 0,
        Pristine = 1 << 1,
        WatchTower = 1 << 2,
        Corruption = 1 << 3,
        Boss = 1 << 4,
        Breach = 1 << 5,
        Expedition = 1 << 6,
        Delirium = 1 << 7,
        Ritual = 1 << 8,
        Irradiated = 1 << 9,
    }
}
