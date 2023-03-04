#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AITrivia.Database;
using AITrivia.DBModels;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.SignalR;
using AITrivia.Hubs;
using System.Text;

namespace chooseSomethingToDo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LobbiesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<LobbyHub> _hub;
        public LobbiesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{url}")]
        public async Task<ActionResult<String>> GetLobby(string url)
        {
            try
            {
                Lobby lobby = await _context.Lobbys.FirstAsync(e => e.UrlString == url);
                if (lobby == null)
                {
                    throw new Exception();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
            }


        }


        [HttpPost]
        [Route("createLobby")]
        public async Task<ActionResult<Lobby>> CreateLobby()
        {
            try
            {
                Lobby lobby = new Lobby();
                Thread.Sleep(1);//make everything unique while looping
                long ticks = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0))).TotalMilliseconds;//EPOCH
                char[] baseChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();

                int i = 32;
                char[] buffer = new char[i];
                int targetBase = baseChars.Length;

                do
                {
                    buffer[--i] = baseChars[ticks % targetBase];
                    ticks = ticks / targetBase;
                }
                while (ticks > 0);

                char[] result = new char[32 - i];
                Array.Copy(buffer, i, result, 0, 32 - i);
                lobby.UrlString = new string(result);
                lobby.users = new List<User>();
                lobby.triviaQuestions = new List<TriviaQuestion>();
                lobby.isDone = false;
                lobby.isStarted = false;
                _context.Lobbys.Add(lobby);


                await _context.SaveChangesAsync();

                return CreatedAtAction("CreateLobby", lobby);
            }
            catch (Exception ex)
            {
                return CreatedAtAction("CreateLobby", ex);
            }

        }








    }
}
