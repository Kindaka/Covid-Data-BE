using Microsoft.AspNetCore.Mvc;
using ODataCovid.Models;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace ODataCovid.Controllers
{
    //[ApiController]
    //[Route("api/v1")]
    public class ConfirmedController : ODataController
    {
        private readonly CovidContext _dbContext;
        public ConfirmedController(CovidContext dbContext)
        {
            _dbContext = dbContext;
        }
        // GET: /<controller>/
        //[Route("confirmeds/page/{page}/pageSize/{pageSize}")]
        //[HttpGet]
        [EnableQuery] 
        public  ActionResult Get()
        {
            try
            {
                return Ok(_dbContext.Confirmeds);

            }
            catch (Exception ex)
            {
                return CreatedAtAction("get Confirmeds error",
                new
                {
                    data = new List<Confirmed>(),
                    total = 0,
                    status = HttpStatusCode.NotFound,
                    message = "error"
                }
               );
            }

        }
        [EnableQuery]
        public ActionResult<Confirmed> Get([FromRoute] long key)
        {
            var item = _dbContext.Confirmeds.SingleOrDefault(d => d.id.Equals(key));

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }
        //[Route("confirmed")]
        //[HttpPost]
        [EnableQuery]
        public async Task<IActionResult> Post([FromBody] Confirmed confirmed)
        {
            try
            {
                long id = -1;
                var check = await _dbContext.Confirmeds.Where(x => x.CountryRegionId == confirmed.CountryRegionId && x.day==confirmed.day).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.value = confirmed.value;
                    id = check.id;
                }
                else
                {
                    var objData = new Confirmed
                    {
                        CountryRegionId = confirmed.CountryRegionId,
                        day = confirmed.day,
                        value = confirmed.value
                    };
                    await _dbContext.Confirmeds.AddAsync(objData);
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
                    return BadRequest("Not a valid confirmed id");
                var objDel = await _dbContext.Confirmeds.Where(x => x.id == id).AsNoTracking().FirstOrDefaultAsync();
                if (objDel == null)
                {
                    return NotFound("Data NotFound");
                }
                _dbContext.Confirmeds.Remove(objDel);
                return Ok("Delete confirmed is success");

            }
            catch (Exception ex)
            {
                return NotFound(new { id = -1, message = "Error", status = StatusCodes.Status404NotFound });
            }
        }
        //[HttpPut("confirmed/id/{id}")]
        [EnableQuery]
        public async Task<IActionResult> Put([FromBody] Confirmed confirmed)
        {
            if (confirmed.id <= 0)
                return BadRequest("Not a valid Confirmed id");
            var objUpd = await _dbContext.Confirmeds.Where(x => x.id == confirmed.id).FirstOrDefaultAsync();
            if (objUpd == null)
            {
                return NotFound("Data NotFound");
            }
            objUpd.CountryRegionId = confirmed.CountryRegionId;
            objUpd.day = confirmed.day;
            objUpd.value = confirmed.value;
            _dbContext.Confirmeds.Update(objUpd);
            await _dbContext.SaveChangesAsync();
            return Ok(objUpd);
        }
    }
}

