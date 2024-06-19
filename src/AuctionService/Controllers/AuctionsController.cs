using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController(AuctionDbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(){
        var auctions = await context.Auctions
            .Include(x => x.Item)
            .OrderBy(x => x.Item.Make)
            .ToListAsync();
            
        return mapper.Map<List<AuctionDto>>(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (auction == null) return NotFound();

        return mapper.Map<AuctionDto>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
    {
        var auction = mapper.Map<Auction>(auctionDto);
        // TODO: add current user as a seller
        auction.Seller = "test";

        context.Auctions.Add(auction);
        
        var result = await context.SaveChangesAsync() > 0;

        if(!result) return BadRequest("Could not save changes to the DB");

        return CreatedAtAction(nameof(GetAuctionById), new {auction.Id}, mapper.Map<AuctionDto>(auction));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AuctionDto>> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (auction == null) return NotFound();

        // TODO: Check current seller name.

        auction.Item.Make = updateAuctionDto?.Make ?? auction.Item?.Make;
        auction.Item.Model = updateAuctionDto?.Model ?? auction.Item?.Model;
        auction.Item.Color = updateAuctionDto?.Color ?? auction.Item?.Color;
        auction.Item.Mileage = updateAuctionDto?.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto?.Year ?? auction.Item.Year;

        var result = await context.SaveChangesAsync() > 0;

        if(!result) return BadRequest("There was no changes in DB.");

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<AuctionDto>> DeleteAuction(Guid id){
        var auction = await context.Auctions.FindAsync(id);

        if (auction == null) return NotFound();

        context.Auctions.Remove(auction);

        var result = await context.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Could not update DB.");

        return Ok(auction);
    }

}
