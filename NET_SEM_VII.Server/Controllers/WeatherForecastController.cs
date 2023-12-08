using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using NET_SEM_VII.Server.db;
using System.Collections;

namespace NET_SEM_VII.Server.Controllers
{
    [ApiController]
    [Route("sensorData")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ISensorService _sensorService;

        public WeatherForecastController(ISensorService sensorService)
        {
            _sensorService = sensorService;
        }

        [HttpGet(Name = "GetSensorData")]
        public IEnumerable<Entity> Get(string? type, string? id, DateTime? minDate = null, DateTime? maxDate = null, string sortOrder = "Ascending", string? sortBy = "Date")
        {
            return _sensorService.GetWithFiltersAndSort(type, id, minDate, maxDate, sortOrder, sortBy).Result.ToArray();
        }
    }
}
