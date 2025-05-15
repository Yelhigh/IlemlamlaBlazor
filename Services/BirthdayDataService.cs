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
        private const string CacheKey = "BirthdayItems";
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public BirthdayDataService(
            IWebHostEnvironment webHostEnvironment,
            IMemoryCache memoryCache,
            ILogger<BirthdayDataService> logger)
        {
            _dataFilePath = Path.Combine(webHostEnvironment.WebRootPath, "data.json");
            _cache = memoryCache;
            _logger = logger;
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

                var sortedItems = data.List
                    .Select(x => new { Item = x, Position = int.TryParse(x.Position, out var pos) ? pos : int.MaxValue })
                    .OrderBy(x => x.Position)
                    .Select(x => x.Item)
                    .ToList();

                _cache.Set(CacheKey, sortedItems, _cacheOptions);

                return sortedItems;
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
