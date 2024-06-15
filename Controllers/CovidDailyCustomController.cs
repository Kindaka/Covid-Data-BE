using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ODataCovid.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ODataCovid.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CovidDailyCustomController : ControllerBase
    {
        private readonly CovidContext _dbContext;
        private static readonly DateTime MinDate = new DateTime(2021, 1, 3);
        private static readonly DateTime MaxDate = new DateTime(2022, 2, 22);

        public CovidDailyCustomController(CovidContext covidContext)
        {
            _dbContext = covidContext;
        }

        [HttpGet("GetCovidDataByDate")]
        public async Task<IActionResult> GetCovidDataByDate([FromQuery] DateTime date)
        {
            if (date < MinDate || date > MaxDate)
            {
                return BadRequest($"Date must be between {MinDate:yyyy-MM-dd} and {MaxDate:yyyy-MM-dd}");
            }

            var covidData = await _dbContext.CovidDailies
                .Where(d => d.day.HasValue && d.day.Value.Date == date.Date)
                .ToListAsync();

            return Ok(covidData);
        }
    }
}
