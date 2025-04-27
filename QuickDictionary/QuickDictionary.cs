namespace QuickDictionary
{
    public interface IQuickDictionaryConfig
    {

    }

    public struct QuickDictionary<KeyT, ValueF, ConfigT>
        where ConfigT: struct, IQuickDictionaryConfig
    {

    }
}