// /////////////////////////////////////////////////////////////////////////////
// YOU CAN FREELY MODIFY THE CODE BELOW IN ORDER TO COMPLETE THE TASK
// /////////////////////////////////////////////////////////////////////////////

namespace WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Helpers;
using WebApi.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
  private readonly DataContext Context;

  public PlayerController(DataContext context)
  {
    Context = context;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<Player>>> GetAll()
  {
        var data =  await Context.Players.ToListAsync();
        return data;
  }

  [HttpPost]
  public async Task<ActionResult<Player>> PostPlayer(Player model)
    {

        Context.Players.Add(model);
        await Context.SaveChangesAsync();
        return Ok(model);

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPlayer(int id, Player player)
    {

        player.Id = id;
        Context.Attach(player);
        Context.Entry(player).State = EntityState.Modified;
        await Context.SaveChangesAsync();
        return Ok(player);



    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Player>> DeletePlayer(int id)
    {
        string header = this.HttpContext.Request.Headers["Authorization"];
        if (header != null && header.StartsWith("Bearer "))
        {

            string token = header.Substring("Basic ".Length).Trim();
            if (token == "SkFabTZibXE1aE14ckpQUUxHc2dnQ2RzdlFRTTM2NFE2cGI4d3RQNjZmdEFITmdBQkE=")
            {
                var data = await Context.Players.FindAsync(id);
                if (data != null)
                {
                    Context.Players.Remove(data);
                    await Context.SaveChangesAsync();
                    return Ok(data);
                }
                else
                {
                    return NotFound($"Could not find the specified play ID:{id}");
                }
            }
            else
            {
                return Unauthorized("UnAuthorized Request");
            }
        }
        else
        {
            return BadRequest("No bearer token provided");
        }


    }
}