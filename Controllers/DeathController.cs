using System.Net;
using ODataCovid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace ODataCovid.Controllers
{
    //[ApiController]
    //[Route("api/v1")]
    public class DeathController : ODataController
    {
        private readonly CovidContext _dbContext;
        public DeathController(CovidContext dbContext)
        {
            _dbContext = dbContext;
        }
        // GET: /<controller>/
        //[Route("deaths/page/{page}/pageSize/{pageSize}")]
        //[HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(_dbContext.Deaths);
            }
            catch (Exception ex)
            {
                return CreatedAtAction("get deaths error",
                new
                {
                    data = new List<Death>(),
                    total = 0,
                    status = HttpStatusCode.NotFound,
                    message = "error"
                }
               );
            }

        }
        [EnableQuery]
        public ActionResult<Death> Get([FromRoute] long key)
        {
            var item = _dbContext.Deaths.SingleOrDefault(d => d.id.Equals(key));

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }
        //[Route("death")]
        //[HttpPost]
        [EnableQuery]
        public async Task<IActionResult> Post([FromBody] Active death)
        {
            try
            {
                long id = -1;
                var check = await _dbContext.Deaths.Where(x => x.CountryRegionId == death.CountryRegionId && x.day == death.day).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.value = death.value;
                    id = check.id;
                }
                else
                {
                    var objData = new Death
                    {
                        CountryRegionId = death.CountryRegionId,
                        day = death.day,
                        value = death.value
                    };
                    await _dbContext.Deaths.AddAsync(objData);
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
        //[Route("death/id/{id}")]
        //[HttpDelete]
        [EnableQuery]
        public async Task<IActionResult> Delete([FromRoute] long id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Not a valid Active id");
                var objDel = await _dbContext.Deaths.Where(x => x.id == id).AsNoTracking().FirstOrDefaultAsync();
                if (objDel == null)
                {
                    return NotFound("Data NotFound");
                }
                _dbContext.Deaths.Remove(objDel);
                return Ok("Delete Active is success");

            }
            catch (Exception ex)
            {
                return NotFound(new { id = -1, message = "Error", status = StatusCodes.Status404NotFound });
            }
        }
        //[HttpPut("death/id/{id}")]
        [EnableQuery]
        public async Task<IActionResult> Put([FromBody] Death death)
        {
            if (death.id <= 0)
                return BadRequest("Not a valid Confirmed id");
            var objUpd = await _dbContext.Deaths.Where(x => x.id == death.id).FirstOrDefaultAsync();
            if (objUpd == null)
            {
                return NotFound("Data NotFound");
            }
            objUpd.CountryRegionId = death.CountryRegionId;
            objUpd.day = death.day;
            objUpd.value = death.value;
            _dbContext.Deaths.Update(objUpd);
            await _dbContext.SaveChangesAsync();
            return Ok(objUpd);
        }
    }
}

