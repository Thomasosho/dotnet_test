using Microsoft.AspNetCore.Mvc;
using VendorBiddingApp.Models;
using VendorBiddingApp.Data;
using System.Threading.Tasks;
using VendorBiddingApp.Services;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;

namespace VendorBiddingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;

        public VendorsController(ApplicationDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // Vendor Registration
        [HttpPost("register")]
        public async Task<IActionResult> CreateVendor([FromBody] VendorRegistrationDto vendorDto)
        {
            // Check if vendor with the same email already exists
            if (await _context.Vendors.AnyAsync(v => v.Email == vendorDto.Email))
            {
                return BadRequest("A vendor with this email already exists.");
            }

            // Create a new Vendor object and hash the password
            var vendor = new Vendor
            {
                Id = Guid.NewGuid(),
                Name = vendorDto.Name,
                Email = vendorDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(vendorDto.Password)
            };

            // Save vendor to the database
            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();

            // Generate JWT token for the newly created vendor
            var token = _tokenService.GenerateToken(vendor.Id.ToString());

            return CreatedAtAction(nameof(GetVendor), new { id = vendor.Id }, new { token, vendorId = vendor.Id, message = "Account Created Successfully" });
        }

        // Vendor Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] VendorLoginDto vendorLogin)
        {
            var vendor = await _context.Vendors.SingleOrDefaultAsync(v => v.Email == vendorLogin.Email);
            if (vendor == null || !BCrypt.Net.BCrypt.Verify(vendorLogin.Password, vendor.PasswordHash))
            {
                return Unauthorized("Invalid email or password");
            }

            var token = _tokenService.GenerateToken(vendor.Id.ToString());
            return Ok(new { token, vendorId = vendor.Id, message = "Vendor Logged in successfully" });
        }

        // ind Vendor by ID
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendor(Guid id)
        {
            var vendor = await _context.Vendors.FindAsync(id);
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
            var vendors = await _context.Vendors
                .Select(v => new { v.Id, v.Name, v.Email })
                .ToListAsync();

            if (!vendors.Any())
            {
                return NotFound("No vendors found.");
            }

            return Ok(vendors);
        }
    }
}
