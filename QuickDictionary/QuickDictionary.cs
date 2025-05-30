﻿using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuickDictionary
{
    public struct QuickDictionary<KeyT, ValueF, ConfigT>
        where ConfigT: struct, IQuickDictionaryConfig<KeyT>
    {
        [StructLayout(LayoutKind.Auto)]
        public struct Entry
        {
            public KeyT Key;

            public ValueF Value;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Entry(KeyT key)
            {
                Key = key;
                Unsafe.SkipInit(out Value);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Entry(KeyT key, ValueF value): this(key)
            {
                Value = value;
            }
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
            Debug.Assert(unchecked((int) capacity) > 0);

            if (BUILT_CONFIG.IsPow2Sized)
            {
                // TODO:
                throw new NotImplementedException();

                // Round to next power of 2
                // capacity =
            }

            entries = AllocateInternal<Entry>(capacity);

            freeEntryMarkers = BUILT_CONFIG.ShouldAllocateFreeEntryMarkers ?
                AllocateInternal<byte>(capacity) :
                [];

            if (BUILT_CONFIG.ShouldFillEntriesArrayWithNullKey)
            {
                var nullEntry = new Entry(BUILT_CONFIG.NullKey);

                entries.AsSpan().Fill(nullEntry);
            }

            return;

            static T[] AllocateInternal<T>(uint capacity)
            {
                return GC.AllocateUninitializedArray<T>(unchecked((int) capacity));
            }
        }
    }
}