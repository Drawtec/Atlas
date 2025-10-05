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
            var atlasNode = Atlas.Read<AtlasNode>(address);
            atlasNode.Address = address;
            return atlasNode;
        }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct AtlasNode
    {
        [FieldOffset(0x0)] public IntPtr Address;
        [FieldOffset(0x110)] public Vector2 RelativePosition;
        [FieldOffset(0x12C)] public float Zoom;
        [FieldOffset(0x1B9)] public AtlasNodeFogState FogFlags;
        [FieldOffset(0x210)] public IntPtr InvalidMapAddress;
        [FieldOffset(0x270)] public IntPtr NodeNameAddress;
        [FieldOffset(0x290)] public AtlasNodeState Flags;

        public readonly float Scale => Zoom / 1.5f;
        public readonly Vector2 Position => RelativePosition * Scale;

        public readonly bool IsConnected => Flags.HasFlag(AtlasNodeState.Connected);
        public readonly bool IsAttempted => Flags.HasFlag(AtlasNodeState.Attempted);
        public readonly bool IsAbyss => Flags.HasFlag(AtlasNodeState.Abyss);
        public readonly bool IsRevealed => FogFlags.HasFlag(AtlasNodeFogState.Revealed);
        public readonly bool IsCompleted => IsAttempted && IsConnected;
        public readonly bool IsFailedAttempt => IsAttempted && !IsConnected;

        public readonly bool IsInvalidMapStructure { get { return InvalidMapAddress != IntPtr.Zero; } }

        public readonly string MapName
        {
            get
            {
                var address = Atlas.Read<IntPtr>(NodeNameAddress + 0x8);
                return Atlas.ReadWideString(address, 64);
            }
        }
    }

    [Flags]
    public enum AtlasNodeState : ulong
    {
        None = 0,
        Connected = 1UL << 0,
        Attempted = 1UL << 1,
        Abyss = 1UL << 12,
    }

    [Flags]
    public enum AtlasNodeFogState : ushort
    {
        None = 0,
        Revealed = 1 << 3
    }
}
