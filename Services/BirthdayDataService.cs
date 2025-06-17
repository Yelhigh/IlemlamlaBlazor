using IlemlamlaBlazor.Interfaces;
using IlemlamlaBlazor.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace IlemlamlaBlazor.Services
{
    public class BirthdayDataService : IBirthdayDataService, IDisposable
    {
        private readonly string _dataFilePath;
        private readonly IMemoryCache _cache;
        private readonly ILogger<BirthdayDataService> _logger;
        private readonly IDynamoDbService _dynamoDbService;
        private const string CacheKey = "BirthdayItems";
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public BirthdayDataService(
            IWebHostEnvironment webHostEnvironment,
            IMemoryCache memoryCache,
            ILogger<BirthdayDataService> logger,
            IDynamoDbService dynamoDbService)
        {
            _dataFilePath = Path.Combine(webHostEnvironment.WebRootPath, "data.json");
            _cache = memoryCache;
            _logger = logger;
            _dynamoDbService = dynamoDbService;
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
        }

        public async Task<List<BirthdayItem>> GetBirthdayItemsAsync()
        {
            try
            {
                if (_cache.TryGetValue(CacheKey, out List<BirthdayItem> cachedItems))
                {
                    return cachedItems;
                }

                // Try to get data from DynamoDB
                try
                {
                    if (await _dynamoDbService.HasDataAsync())
                    {
                        var dynamoItems = await _dynamoDbService.GetBirthdayItemsAsync();
                        var birthdayItems = dynamoItems.Select(item => new BirthdayItem
                        {
                            Name = item.Name,
                            Date = item.Date,
                            Position = item.Position
                        }).ToList();

                        var sortedItems = birthdayItems
                            .Select(x => new { Item = x, Position = int.TryParse(x.Position, out var pos) ? pos : int.MaxValue })
                            .OrderBy(x => x.Position)
                            .Select(x => x.Item)
                            .ToList();

                        _cache.Set(CacheKey, sortedItems, _cacheOptions);
                        return sortedItems;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to retrieve data from DynamoDB, falling back to file-based data");
                }

                // Fallback to file-based data
                if (!File.Exists(_dataFilePath))
                {
                    _logger.LogError("Data file not found at path: {FilePath}", _dataFilePath);
                    return new List<BirthdayItem>();
                }

                var json = await File.ReadAllTextAsync(_dataFilePath);
                var data = JsonSerializer.Deserialize<BirthdayData>(json);

                if (data?.List == null)
                {
                    _logger.LogError("Invalid data format in file: {FilePath}", _dataFilePath);
                    return new List<BirthdayItem>();
                }

                var fileSortedItems = data.List
                    .Select(x => new { Item = x, Position = int.TryParse(x.Position, out var pos) ? pos : int.MaxValue })
                    .OrderBy(x => x.Position)
                    .Select(x => x.Item)
                    .ToList();

                _cache.Set(CacheKey, fileSortedItems, _cacheOptions);

                return fileSortedItems;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing birthday data");
                return new List<BirthdayItem>();
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Error reading birthday data file");
                return new List<BirthdayItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving birthday data");
                return new List<BirthdayItem>();
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
