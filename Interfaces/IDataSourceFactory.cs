namespace IlemlamlaBlazor.Interfaces
{
    public interface IDataSourceFactory
    {
        IDataSourceStrategy CreateStrategy(string sourceType);
    }
} 