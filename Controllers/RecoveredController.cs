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
    public class RecoveredController : ODataController
    {
        private readonly CovidContext _dbContext;
        public RecoveredController(CovidContext dbContext)
        {
            _dbContext = dbContext;
        }
        // GET: /<controller>/
        //[Route("recovereds/page/{page}/pageSize/{pageSize}")]
        //[HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            try
            {
                // var data =   _dbContext.Positions.Include(c=>c.organization);
                return Ok(_dbContext.Recovereds);
            }
            catch (Exception)
            {
                return CreatedAtAction("get recovereds error",
                new
                {
                    data = new List<Recovered>(),
                    total = 0,
                    status = HttpStatusCode.NotFound,
                    message = "error"
                }
               );
            }

        }
        [EnableQuery]
        public ActionResult<Recovered> Get([FromRoute] long key)
        {
            var item = _dbContext.Recovereds.SingleOrDefault(d => d.id.Equals(key));

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }
        //[Route("recovered")]
        //[HttpPost]
        [EnableQuery]
        public async Task<IActionResult> Post([FromBody] Recovered recovered)
        {
            try
            {
                long id = -1;
                var check = await _dbContext.Recovereds.Where(x => x.CountryRegionId == recovered.CountryRegionId && x.day == recovered.day).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.value = recovered.value;
                    id = check.id;
                }
                else
                {
                    var objData = new Recovered
                    {
                        CountryRegionId = recovered.CountryRegionId,
                        day = recovered.day,
                        value = recovered.value
                    };
                    await _dbContext.Recovereds.AddAsync(objData);
                    id = objData.id;
                }

                await _dbContext.SaveChangesAsync();
                return Ok(new { id, message = "OK", status = StatusCodes.Status200OK });

            }
            catch (Exception)
            {
                return NotFound(new { id = -1, message = "Error", status = StatusCodes.Status404NotFound });
            }
        }
        //[Route("recovered/id/{id}")]
        //[HttpDelete]
        [EnableQuery]
        public async Task<IActionResult> Delete([FromBody] long id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Not a valid recovered id");
                var objDel = await _dbContext.Recovereds.Where(x => x.id == id).AsNoTracking().FirstOrDefaultAsync();
                if (objDel == null)
                {
                    return NotFound("Data NotFound");
                }
                _dbContext.Recovereds.Remove(objDel);
                return Ok("Delete recovered is success");

            }
            catch (Exception)
            {
                return NotFound(new { id = -1, message = "Error", status = StatusCodes.Status404NotFound });
            }
        }
        //[HttpPut("recovered/id/{id}")]
        [EnableQuery]
        public async Task<IActionResult> Put([FromBody] Recovered recovered)
        {
            if (recovered.id <= 0)
                return BadRequest("Not a valid Confirmed id");
            var objUpd = await _dbContext.Recovereds.Where(x => x.id == recovered.id).FirstOrDefaultAsync();
            if (objUpd == null)
            {
                return NotFound("Data NotFound");
            }
            objUpd.CountryRegionId = recovered.CountryRegionId;
            objUpd.day = recovered.day;
            objUpd.value = recovered.value;
            _dbContext.Recovereds.Update(objUpd);
            await _dbContext.SaveChangesAsync();
            return Ok(objUpd);
        }
    }
}

