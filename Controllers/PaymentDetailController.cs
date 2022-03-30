using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Models;
using PaymentAPI.Data;

// 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
// 

namespace PaymentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaymentDetailController : ControllerBase
    {
        [Route("Biodata")]
        [HttpGet]
        [Produces("text/html")]
        public ActionResult<string> Biodata()
        {
            string _result = "Kevin Hadinata";
            return _result;
        }
        private readonly ApiDbContext _context;

        public PaymentDetailController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPayments()
        {
            var payments = await _context.Payments.ToListAsync();
            return Ok(payments);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(PaymentData data)
        {
            if(ModelState.IsValid)
            {
                await _context.Payments.AddAsync(data);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetPayments", new {data.paymentDetailId}, data);
            }

            return new JsonResult("Something went wrong") {StatusCode = 500};
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayment(int id)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(x => x.paymentDetailId == id);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, PaymentData payment)
        {
            if (id != payment.paymentDetailId)
                return BadRequest();

            var existPayment = await _context.Payments.FirstOrDefaultAsync(x => x.paymentDetailId == id);

            if (existPayment == null)
                return NotFound();

            existPayment.cardOwnerName = payment.cardOwnerName;
            existPayment.cardNumber = payment.cardNumber;
            existPayment.expirationDate = payment.expirationDate;
            existPayment.securityCode = payment.securityCode;

            // Implement the changes on the databbase level
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var existPayment = await _context.Payments.FirstOrDefaultAsync(x => x.paymentDetailId == id);

            if(existPayment == null)
                return NotFound();

            _context.Payments.Remove(existPayment);
            await _context.SaveChangesAsync();

            return Ok(existPayment);
        }
        //

    }
}