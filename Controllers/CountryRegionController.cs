using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using ODataCovid.Models;
using System.Collections.Generic;

namespace ODataCovid.Controllers
{
    public class CountryRegionsController : ODataController
    {
        private readonly CovidContext _dbContext;

        public CountryRegionsController(CovidContext covidContext)
        {
            _dbContext = covidContext;
        }

        [EnableQuery]
        [HttpGet("country-regions")]
        public IActionResult Get()
        {
            var result = UseYield();
            return Ok(result);
        }

        public IEnumerable<CountryRegion> UseYield()
        {
            var query = _dbContext.CountryRegions.AsNoTracking();

            foreach (var item in query.AsEnumerable())
            {
                yield return item;
            }
        }

        [EnableQuery]
        [HttpGet("country-region")]
        public ActionResult<CountryRegion> Get([FromRoute] long key)
        {
            var item = _dbContext.CountryRegions.Include(c => c.CovidDailies).SingleOrDefault(d => d.id.Equals(key));

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }
    }
}


