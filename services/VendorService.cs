using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using VendorBiddingApp.Data;
using VendorBiddingApp.Models;

namespace VendorBiddingApp.Services
{
    public class VendorService
    {
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;

        public VendorService(ApplicationDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<string> RegisterVendorAsync(VendorRegistrationDto vendorDto)
        {
            if (await _context.Vendors.AnyAsync(v => v.Email == vendorDto.Email))
            {
                throw new Exception("A vendor with this email already exists.");
            }

            var vendor = new Vendor
            {
                Id = Guid.NewGuid(),
                Name = vendorDto.Name,
                Email = vendorDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(vendorDto.Password)
            };

            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();

            return _tokenService.GenerateToken(vendor.Id.ToString());
        }

        public async Task<string> LoginVendorAsync(VendorLoginDto vendorLogin)
        {
            var vendor = await _context.Vendors.SingleOrDefaultAsync(v => v.Email == vendorLogin.Email);
            if (vendor == null || !BCrypt.Net.BCrypt.Verify(vendorLogin.Password, vendor.PasswordHash))
            {
                throw new Exception("Invalid email or password");
            }

            return _tokenService.GenerateToken(vendor.Id.ToString());
        }

        public async Task<Vendor?> GetVendorByIdAsync(Guid id)
        {
            return await _context.Vendors.FindAsync(id);
        }

        public async Task<List<Vendor>> GetAllVendorsAsync()
        {
            return await _context.Vendors.ToListAsync();
        }
    }
}
