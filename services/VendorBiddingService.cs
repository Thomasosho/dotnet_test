using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VendorBiddingApp.Data;
using VendorBiddingApp.Models;

namespace VendorBiddingApp.Services
{
    public class VendorBiddingService
    {
        private readonly ApplicationDbContext _context;

        public VendorBiddingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Bid>> GetBidsAsync()
        {
            return await _context.Bids
                .Select(b => new Bid
                {
                    Id = b.Id,
                    VendorId = b.VendorId,
                    ProjectId = b.ProjectId,
                    Amount = b.Amount,
                    Status = b.Status
                })
                .ToListAsync();
        }

        public async Task<Bid?> GetBidByIdAsync(Guid id)
        {
            return await _context.Bids
                .Where(b => b.Id == id)
                .Select(b => new Bid
                {
                    Id = b.Id,
                    VendorId = b.VendorId,
                    ProjectId = b.ProjectId,
                    Amount = b.Amount,
                    Status = b.Status
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateBidAsync(Bid bid)
        {
            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Bid>> GetBidsByVendorAsync(string vendorId)
        {
            return await _context.Bids
                .Where(b => b.VendorId == vendorId)
                .Select(b => new Bid
                {
                    Id = b.Id,
                    VendorId = b.VendorId,
                    ProjectId = b.ProjectId,
                    Amount = b.Amount,
                    Status = b.Status
                })
                .ToListAsync();
        }
    }
}
