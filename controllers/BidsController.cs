using Microsoft.AspNetCore.Mvc;
using VendorBiddingApp.Models;
using VendorBiddingApp.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace VendorBiddingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BidsController : ControllerBase
    {
        private readonly VendorBiddingService _vendorBiddingService;

        public BidsController(VendorBiddingService vendorBiddingService)
        {
            _vendorBiddingService = vendorBiddingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBids()
        {
            var bids = await _vendorBiddingService.GetBidsAsync();
            var bidDtos = bids.Select(b => new
            {
                b.Id,
                b.VendorId,
                b.ProjectId,
                b.Amount,
                b.Status
            }).ToList();

            return Ok(bidDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBid(Guid id)
        {
            var bid = await _vendorBiddingService.GetBidByIdAsync(id);
            if (bid == null)
            {
                return NotFound();
            }

            var bidDto = new
            {
                bid.Id,
                bid.VendorId,
                bid.ProjectId,
                bid.Amount,
                bid.Status
            };

            return Ok(bidDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBid([FromBody] BidDto bidDto)
        {
            if (bidDto == null)
            {
                return BadRequest("Bid data is null.");
            }

            if (string.IsNullOrEmpty(bidDto.VendorId))
            {
                return BadRequest("VendorId is required.");
            }

            var bid = new Bid
            {
                Id = Guid.NewGuid(),
                VendorId = bidDto.VendorId,
                ProjectId = bidDto.ProjectId,
                Amount = bidDto.Amount,
                Status = bidDto.Status
            };

            await _vendorBiddingService.CreateBidAsync(bid);

            var response = new
            {
                bidId = bid.Id,
                message = "Bid Created Successfully"
            };

            return CreatedAtAction(nameof(GetBid), new { id = bid.Id }, response);
        }

        [HttpGet("vendor/{vendorId}")]
        public async Task<IActionResult> GetBidsByVendor(string vendorId)
        {
            var bids = await _vendorBiddingService.GetBidsByVendorAsync(vendorId);
            var bidDtos = bids.Select(b => new
            {
                b.Id,
                b.VendorId,
                b.ProjectId,
                b.Amount,
                b.Status
            }).ToList();

            return Ok(bidDtos);
        }
    }
}
