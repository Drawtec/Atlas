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
        [FieldOffset(0x165)] public byte Flags;

        public readonly int Length => (int)(this.LastChild.ToInt64() - this.FirstChild.ToInt64()) / 0x8;

        public readonly bool IsVisible => (this.Flags & 0x8) != 0;

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
        [FieldOffset(0xD0)] public Vector2 RelativePosition;
        [FieldOffset(0xE8)] public float Zoom;
        [FieldOffset(0x2A8)] public IntPtr NodeNameAddress;
        [FieldOffset(0x298)] public byte Flags1;
        [FieldOffset(0x299)] public byte Flags2;
        [FieldOffset(0x29A)] public byte Flags3;
        [FieldOffset(0x29F)] public byte ByteAvailable;

        public readonly float Scale => Zoom / 1.5f;
        public readonly Vector2 Position => RelativePosition * Scale;

        public readonly bool IsCompleted => (Flags1 & 0b0100_0000) != 0;
        public readonly bool IsAttempted => (Flags2 & 0b0000_0001) != 0;
        public readonly bool IsPristine => (Flags2 & 0b0000_0010) != 0;
        public readonly bool IsWatchTower => (Flags2 & 0b0000_0100) != 0;
        public readonly bool HasCorruption => (Flags2 & 0b0000_1000) != 0;
        public readonly bool HasBoss => (Flags2 & 0b0001_0000) != 0;
        public readonly bool HasBreach => (Flags2 & 0b0010_0000) != 0;
        public readonly bool HasExpedition => (Flags2 & 0b0100_0000) != 0;
        public readonly bool HasDelirium => (Flags2 & 0b1000_0000) != 0;
        public readonly bool HasRitual => (Flags3 & 0b0000_0001) != 0;
        public readonly bool HasIrradiated => (Flags3 & 0b0000_0010) != 0;
        //public readonly bool IsCompleted => IsAttempted && IsPristine;
        public readonly bool IsFailedAttempt => IsAttempted && !IsCompleted;
        public readonly bool IsAvailable => !IsAttempted && IsPristine;

        public string MapName
        {
            get
            {
                var address = Atlas.Read<IntPtr>(NodeNameAddress + 0x8);
                return Atlas.ReadWideString(address, 64);
            }
        }
    }
}
