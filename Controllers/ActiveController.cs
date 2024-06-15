using System.Net;
using ODataCovid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ODataCovid.Controllers
{
    //[ApiController]
    //[Route("api/v1")]
    public class ActiveController : ODataController
    {
        private readonly CovidContext _dbContext;
        public ActiveController(CovidContext dbContext)
        {
            _dbContext = dbContext;
        }
        [EnableQuery]
        //[HttpGet("country-region")]
        public ActionResult<Active> Get([FromRoute] long key)
        {
            var item = _dbContext.Actives.SingleOrDefault(d => d.id.Equals(key));

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }
        // GET: /<controller>/
        //[Route("actives/page/{page}/pageSize/{pageSize}")]
        //[HttpGet]
        [EnableQuery(PageSize =1)]
        public ActionResult Get()
        {
            try
            {
                return Ok(_dbContext.Actives);
            }
            catch (Exception ex)
            {
                return CreatedAtAction("get Active error",
                new
                {
                    data = new List<Active>(),
                    total = 0,
                    status = HttpStatusCode.NotFound,
                    message = "error"
                }
               );
            }

        }
        //[Route("active")]
        //[HttpPost]
        [EnableQuery]
        public async Task<IActionResult> Post([FromBody] Active active)
        {
            try
            {
                long id = -1;
                var check = await _dbContext.Actives.Where(x => x.CountryRegionId == active.CountryRegionId && x.day == active.day).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.value = active.value;
                    id = check.id;
                }
                else
                {
                    var objData = new Active
                    {
                        CountryRegionId = active.CountryRegionId,
                        day = active.day,
                        value = active.value
                    };
                    await _dbContext.Actives.AddAsync(objData);
                    id = objData.id;
                }

                await _dbContext.SaveChangesAsync();
                return Ok(new { id = id, message = "OK", status = StatusCodes.Status200OK });

            }
            catch (Exception ex)
            {
                return NotFound(new { id = -1, message = "Error", status = StatusCodes.Status404NotFound });
            }
        }
        //[Route("confirmed/id/{id}")]
        //[HttpDelete]
        [EnableQuery]
        public async Task<IActionResult> Delete([FromRoute] long id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Not a valid Active id");
                var objDel = await _dbContext.Actives.Where(x => x.id == id).AsNoTracking().FirstOrDefaultAsync();
                if (objDel == null)
                {
                    return NotFound("Data NotFound");
                }
                _dbContext.Actives.Remove(objDel);
                return Ok("Delete Active is success");

            }
            catch (Exception ex)
            {
                return NotFound(new { id = -1, message = "Error", status = StatusCodes.Status404NotFound });
            }
        }
        //[HttpPut("active/id/{id}")]
        [EnableQuery]
        public async Task<IActionResult> Put([FromBody] Active active)
        {
            if (active.id <= 0)
                return BadRequest("Not a valid Confirmed id");
            var objUpd = await _dbContext.Actives.Where(x => x.id == active.id).FirstOrDefaultAsync();
            if (objUpd == null)
            {
                return NotFound("Data NotFound");
            }
            objUpd.CountryRegionId = active.CountryRegionId;
            objUpd.day = active.day;
            objUpd.value = active.value;
            _dbContext.Actives.Update(objUpd);
            await _dbContext.SaveChangesAsync();
            return Ok(objUpd);
        }
    }
}

