using IlemlamlaBlazor.Interfaces;

namespace IlemlamlaBlazor.Interfaces
{
    public interface IDataSourceStrategyFactory
    {
        Task<IDataSourceStrategy> GetStrategyAsync();
    }
} 