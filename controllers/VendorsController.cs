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
    public class VendorsController : ControllerBase
    {
        private readonly VendorService _vendorService;

        public VendorsController(VendorService vendorService)
        {
            _vendorService = vendorService;
        }

        // Vendor Registration
        [HttpPost("register")]
        public async Task<IActionResult> CreateVendor([FromBody] VendorRegistrationDto vendorDto)
        {
            try
            {
                var token = await _vendorService.RegisterVendorAsync(vendorDto);
                return CreatedAtAction(nameof(GetVendor), new { id = Guid.NewGuid() }, new { token, message = "Account Created Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Vendor Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] VendorLoginDto vendorLogin)
        {
            try
            {
                var token = await _vendorService.LoginVendorAsync(vendorLogin);
                return Ok(new { token, message = "Vendor Logged in successfully" });
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        // Get Vendor by ID
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendor(Guid id)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            if (vendor == null)
            {
                return NotFound("No vendor with ID found.");
            }
            return Ok(new { vendor.Id, vendor.Name, vendor.Email });
        }

        // Get all Vendors
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllVendors()
        {
            var vendors = await _vendorService.GetAllVendorsAsync();
            if (!vendors.Any())
            {
                return NotFound("No vendors found.");
            }
            return Ok(vendors.Select(v => new { v.Id, v.Name, v.Email }));
        }
    }
}
