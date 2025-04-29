using System.Runtime.CompilerServices;

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

        public static virtual DeletionMode DeletionMode
            => DeletionMode.Tombstone | DeletionMode.ClearGCReferences;
    }

    internal struct BuiltConfig<KeyT, ConfigT>
        where ConfigT: IQuickDictionaryConfig<KeyT>
    {
        public bool HasNullKey;

        public KeyT NullKey;

        public bool IsPow2Sized;

        public DeletionMode DeletionMode;

        public bool KeyIsClass;

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
            KeyIsClass = typeof(KeyT).IsClass;
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

        // For ref-types, the uninitialized array will be filled with nulls
        public bool ShouldFillEntriesArrayWithNullKey
        {
            // Conditions:
            // - There is a null key specified
            // - The key is not a ref-type or it is a ref-type that is not null
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => HasNullKey && (!KeyIsClass || NullKey != null);
        }
    }
}