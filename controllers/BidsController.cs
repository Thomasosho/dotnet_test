using Microsoft.AspNetCore.Mvc;
using VendorBiddingApp.Models;
using VendorBiddingApp.Data;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace VendorBiddingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BidsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BidsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetBids()
        {
            var bids = _context.Bids
                .Select(b => new
                {
                    b.Id,
                    b.VendorId,
                    b.ProjectId,
                    b.Amount,
                    b.Status
                })
                .ToList();
            return Ok(bids);
        }

        [HttpGet("{id}")]
        public IActionResult GetBid(Guid id)
        {
            var bid = _context.Bids
                .Where(b => b.Id == id)
                .Select(b => new
                {
                    b.Id,
                    b.VendorId,
                    b.ProjectId,
                    b.Amount,
                    b.Status
                })
                .FirstOrDefault();

            if (bid == null)
            {
                return NotFound();
            }
            return Ok(bid);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBid([FromBody] BidDto bidDto)
        {
            if (bidDto == null)
            {
                return BadRequest("Bid data is null.");
            }

            var bid = new Bid
            {
                Id = Guid.NewGuid(),
                VendorId = bidDto.VendorId,
                ProjectId = bidDto.ProjectId,
                Amount = bidDto.Amount,
                Status = bidDto.Status
            };

            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();

            var response = new
            {
                bidId = bid.Id,
                message = "Bid Created Successfully"
            };

            return CreatedAtAction(nameof(GetBid), new { id = bid.Id }, response);
        }

        [HttpGet("vendor/{vendorId}")]
        public IActionResult GetBidsByVendor(int vendorId)
        {
            var bids = _context.Bids
                .Where(b => b.VendorId == vendorId)
                .Select(b => new
                {
                    b.Id,
                    b.VendorId,
                    b.ProjectId,
                    b.Amount,
                    b.Status
                })
                .ToList();
            return Ok(bids);
        }
    }
}
