using System.Threading.Tasks;
using AITrivia.Database;
using AITrivia.HubModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using AITrivia.Controllers;
using AITrivia.DBModels;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AITrivia.Hubs
{
    public class LobbyHub : Hub
    {
        private readonly AppDbContext dbContext;
        
        public LobbyHub(AppDbContext context)
        {
            dbContext = context;
        }
        public async Task JoinLobby(JoinLobbyMessage message)
        {
            try
            {
                Lobby lobby = await dbContext.Lobbys.Include(x => x.users).FirstAsync(m => m.UrlString == message.UrlString);

                await Groups.AddToGroupAsync(Context.ConnectionId, message.UrlString);
                if (lobby.isStarted)
                {
                    //lobby started, so error it up
                    Clients.Client(Context.ConnectionId).SendAsync("ErrorConnection");
                }
                else
                {
                    User user = new User();
                    user.Name = message.UserName;

                    user.ConnectionID = Context.ConnectionId;
                   

                    if (lobby.users.Count == 0)
                    {
                        //assume user count 0 means the leaer is joining
                        user.IsLeader = true;
                    }
                    else
                    {
                        user.IsLeader = false;


                    }
                    lobby.users.Add(user);
                    await dbContext.SaveChangesAsync();
                    await Clients.Client(Context.ConnectionId).SendAsync("UserCreated", user);
                    await Clients.Group(message.UrlString).SendAsync("JoinedLobby", lobby.users);
                }
            }
            catch (Exception ex)
            {
                Clients.Client(Context.ConnectionId).SendAsync("ErrorConnection");
            }


        }

        public async Task StartLobby(StartMessage message)
        {
            Lobby lobby = await dbContext.Lobbys.Include(lobby => lobby.users).
                Include(lobby => lobby.triviaQuestions).
                ThenInclude(question => question.answers).
                FirstAsync(m => m.UrlString == message.UrlString);

            User user = await dbContext.Users.FirstAsync(m => m.Id == message.UserId);
            try
            {
                if (lobby.users.Contains(user))
                {
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {

            }

            if (user.IsLeader)
            {
                //USE THIS SECTION TO GET API INFO
                try
                {
                         await Clients.Group(message.UrlString).SendAsync("StartedLobby", "not ready");
                    
                        try
                        {
                            lobby.isStarted = true;
                            //await dbContext.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    
                    await Clients.Group(message.UrlString).SendAsync("ReadyLobby", "EROR");
                }
                catch
                {
                    await Clients.Group(message.UrlString).SendAsync("ErrorAPI", "EROR");
                }


            }
            else
            {


            }



        }

        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            // Add your own code here.
            // For example: in a chat application, mark the user as offline, 
            // delete the association between the current connection id and user name.
            User user = await dbContext.Users.FirstAsync(m => m.ConnectionID == Context.ConnectionId);
            Lobby lobby = await dbContext.Lobbys.Include(x => x.users).FirstAsync(m => m.Id == user.LobbyId);

            lobby.users.Remove(user);
            if (lobby.users.Count == 0)
            {
                dbContext.Lobbys.Remove(lobby);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                await dbContext.SaveChangesAsync();
                await Clients.Group(lobby.UrlString).SendAsync("JoinedLobby", lobby.users);
            }



        }

    }
}