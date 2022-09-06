// /////////////////////////////////////////////////////////////////////////////
// YOU CAN FREELY MODIFY THE CODE BELOW IN ORDER TO COMPLETE THE TASK
// /////////////////////////////////////////////////////////////////////////////

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController: ControllerBase
    {
        private readonly DataContext Context;

        public TeamController(DataContext context)
        {
            Context = context;
        }

        [HttpPost("process")]
        public async Task<ActionResult<List<Player>>> Process([FromBody] List<ParamModel> models)
        {
            var req = models.GroupBy(a => a.Position)
              .Where(g => g.Count() > 1)
              .Select(x => x.Key)
              .ToList();

            if (req.Count()>0)
            {
                return BadRequest($"Error Multiple position specified. Position:{req.FirstOrDefault()}");
            }

            var players = new List<Player>();

            foreach (var model in models)
            {
                var playSearch =await Context.Players.Where(a => a.PlayerSkills.Any(b => b.Skill == model.MainSkill) 
                && a.Position == model.Position).Include(a=>a.PlayerSkills).ToListAsync();
                if (playSearch != null)
                {

                    if (playSearch.Count() > 1)
                    {
                        //Given a position and the skill desired for that position,
                        //the app should be able to find the
                        //best player in the database with that skill and position.
                        //If there are more than one player
                        //with the highest skill value,
                        //the solution can select any of those players.
                        var best = playSearch.OrderByDescending(a => a.PlayerSkills.Max(b => b.Value)).Take(model.NumberOfPlayers);

                        //The request should allow only one
                        //skill per position. The same skill
                        //can be used in different positions.
                        //For example: you cannot send a request
                        //for defender with highest speed and defense skill.
                        foreach (var item in best)
                        {
                            var requestedSkill = item.PlayerSkills.OrderByDescending(a => a.Value).Take(1).FirstOrDefault();
                            item.PlayerSkills = new List<PlayerSkill>();
                            item.PlayerSkills.Add(requestedSkill);
                            players.Add(item);

                        }
                    }
                }
                else
                {
                    //If there are no players in the database with the desired skill,
                    //the app should find the highest skill value
                    //for any players in the selected position.
                    //For example, if in the database we have
                    //3 defenders with these skills:
                    var playSearch2 = Context.Players.Where(a => a.Position == model.Position).Include(a => a.PlayerSkills);
                    var best = playSearch.OrderByDescending(a => a.PlayerSkills.Max(b => b.Value)).Take(model.NumberOfPlayers);

                    //The request should allow only one
                    //skill per position. The same skill
                    //can be used in different positions.
                    //For example: you cannot send a request
                    //for defender with highest speed and defense skill.
                    foreach (var item in best)
                    {
                        var requestedSkill = item.PlayerSkills.OrderByDescending(a => a.Value).Take(1).FirstOrDefault();
                        item.PlayerSkills = new List<PlayerSkill>();
                        item.PlayerSkills.Add(requestedSkill);
                        players.Add(item);

                    }
                }
            }

            foreach (var model in models)
            {
                int playerCount = players.Where(a => a.Position == model.Position).Count();
                if (playerCount < model.NumberOfPlayers)
                {
                    return NotFound($"Insufficient number of players for position: {model.Position}");
                }
            }
            return Ok(players);

        }
    }
}
