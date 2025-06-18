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
                
                var tempData = JsonSerializer.Deserialize<JsonElement>(json);
                var listElement = tempData.GetProperty("List");
                var enumerateArray = listElement.EnumerateArray();
                var items = new List<BirthdayItem>();

                foreach (var item in enumerateArray)
                {
                    try
                    {
                        var name = item.GetProperty("Name").GetString() ?? string.Empty;
                        var date = item.GetProperty("Date").GetString() ?? string.Empty;
                        var positionStr = item.GetProperty("Position").GetString() ?? "0";
                        
                        if (int.TryParse(positionStr, out int position))
                        {
                            items.Add(new BirthdayItem
                            {
                                Name = name,
                                Date = date,
                                Position = position
                            });
                        }
                        else
                        {
                            _logger.LogWarning("Invalid Position format in JSON: {Position}", positionStr);
                            items.Add(new BirthdayItem
                            {
                                Name = name,
                                Date = date,
                                Position = enumerateArray.Count() + 2
                            });
                        }
                    }
                    catch (KeyNotFoundException ex)
                    {
                        _logger.LogWarning("Missing required field in JSON item: {FieldName}", ex.Message);
                    }
                }

                return items.OrderBy(x => x.Position).ToList();
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
                var data = JsonSerializer.Deserialize<JsonElement>(json);
                return data.TryGetProperty("List", out var listElement) && listElement.GetArrayLength() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking file system data");
                return false;
            }
        }
    }
} 