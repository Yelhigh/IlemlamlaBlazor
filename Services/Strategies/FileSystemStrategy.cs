using IlemlamlaBlazor.Interfaces;
using IlemlamlaBlazor.Models;
using System.Text.Json;

namespace IlemlamlaBlazor.Services.Strategies
{
    public class FileSystemStrategy : IDataSourceStrategy
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<FileSystemStrategy> _logger;
        private readonly string _dataFilePath;

        public string SourceName => "File System";

        public FileSystemStrategy(
            IWebHostEnvironment webHostEnvironment,
            ILogger<FileSystemStrategy> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _dataFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "data.json");
        }

        public async Task<List<BirthdayItem>> GetDataAsync()
        {
            try
            {
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

                return data.List
                    .Select(x => new { Item = x, Position = int.TryParse(x.Position, out var pos) ? pos : int.MaxValue })
                    .OrderBy(x => x.Position)
                    .Select(x => x.Item)
                    .ToList();
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

        public async Task<bool> HasDataAsync()
        {
            try
            {
                if (!File.Exists(_dataFilePath))
                {
                    return false;
                }

                var json = await File.ReadAllTextAsync(_dataFilePath);
                var data = JsonSerializer.Deserialize<BirthdayData>(json);
                return data?.List != null && data.List.Any();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking file system data");
                return false;
            }
        }
    }
} 