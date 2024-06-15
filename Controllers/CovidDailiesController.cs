using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using ODataCovid.Models;
using System.Collections.Generic;

namespace ODataCovid.Controllers
{
    public class CovidDailiesController : ODataController
    {
        private readonly CovidContext _dbContext;

        public CovidDailiesController(CovidContext covidContext)
        {
            _dbContext = covidContext;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            var result = UseYield();
            return Ok(result);
        }

        public IEnumerable<CovidDaily> UseYield()
        {
            var query = _dbContext.CovidDailies.Include(c => c.CountryRegion).AsNoTracking();

            foreach (var item in query.AsEnumerable())
            {
                yield return item;
            }
        }

        [EnableQuery]
        public ActionResult<CovidDaily> Get([FromRoute] long key)
        {
            var item = _dbContext.CovidDailies.Include(c => c.CountryRegion).SingleOrDefault(d => d.id.Equals(key));

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }
    }

}
