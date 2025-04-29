namespace QuickDictionary
{
    [Flags]
    public enum DeletionMode
    {
        Tombstone = 1 << 0,
        ShiftDelete = 1 << 1,
        ClearGCReferences = 1 << 3,
    }
}