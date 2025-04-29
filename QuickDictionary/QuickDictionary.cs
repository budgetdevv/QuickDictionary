using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuickDictionary
{
    public interface IQuickDictionaryConfig<KeyT>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static virtual bool TryGetNullKey(out KeyT? nullKey)
        {
            nullKey = default;

            return typeof(KeyT).IsClass;
        }

        public static virtual bool IsPow2Sized => false;

        public static virtual DeletionMode DeletionMode => DeletionMode.Tombstone;
    }

    public enum DeletionMode
    {
        Tombstone,
        ShiftDelete,
    }

    internal struct BuiltConfig<KeyT, ConfigT>
        where ConfigT: IQuickDictionaryConfig<KeyT>
    {
        public bool HasNullKey;

        public KeyT NullKey;

        public bool IsPow2Sized;

        public DeletionMode DeletionMode;

        private BuiltConfig(
            bool hasNullKey,
            KeyT? nullKey,
            bool isPow2Sized,
            DeletionMode deletionMode)
        {
            HasNullKey = hasNullKey;
            NullKey = nullKey!;
            IsPow2Sized = isPow2Sized;
            DeletionMode = deletionMode;
        }

        public static BuiltConfig<KeyT, ConfigT> Build()
        {
            var hasNullKey = ConfigT.TryGetNullKey(out var nullKey);

            return new(
                hasNullKey,
                nullKey,
                ConfigT.IsPow2Sized,
                ConfigT.DeletionMode
            );
        }

        public bool ShouldAllocateFreeEntryMarkers
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => !HasNullKey;
        }
    }

    public struct QuickDictionary<KeyT, ValueF, ConfigT>
        where ConfigT: struct, IQuickDictionaryConfig<KeyT>
    {
        [StructLayout(LayoutKind.Auto)]
        public struct Entry
        {
            public KeyT Key;

            public ValueF Value;
        }

        private static readonly BuiltConfig<KeyT, ConfigT> BUILT_CONFIG = BuiltConfig<KeyT, ConfigT>.Build();

        private Entry[] Entries;

        private byte[] FreeEntryMarkers;

        public uint Count { get; private set; }

        // This value should be recomputed on resize
        private uint LongestRecordedProbeLength;

        public QuickDictionary(uint initialCapacity)
        {
            AllocateAtLeast(
                initialCapacity,
                out Entries,
                out FreeEntryMarkers
            );

            Count = LongestRecordedProbeLength = 0;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void AllocateAtLeast(
            uint capacity,
            out Entry[] entries,
            out byte[] freeEntryMarkers)
        {
            throw new NotImplementedException();

            // Debug.Assert(unchecked((int) capacity) > 0);
            //
            // if (BUILT_CONFIG.IsPow2Sized)
            // {
            //     // Round to next power of 2
            //     // capacity =
            // }
            //
            // entries = AllocateInternal<Entry>(capacity);
            //
            // freeEntryMarkers = BUILT_CONFIG.ShouldAllocateFreeEntryMarkers ?
            //     AllocateInternal<byte>(capacity) :
            //     [];
            //
            // return;
            //
            // static T[] AllocateInternal<T>(uint capacity)
            // {
            //     return GC.AllocateUninitializedArray<T>(unchecked((int) capacity));
            // }
        }
    }
}