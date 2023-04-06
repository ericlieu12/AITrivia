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
                    user.score = 0;
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
        public async Task dispenseQuestion(String urlString, TriviaQuestion question)
        {
           
            
            
            //prevent client exposure of correct answer TOD O LATER
            var listOfAnswers = new List<String>();
            var correctAnswer = "";
            foreach (TriviaAnswer answer in question.answers)
            {
                if (answer.isCorrect)
                {
                    correctAnswer = answer.answerString;
                }
                listOfAnswers.Add(answer.answerString);
            }
            var questionToReturn = new 
            { question = question.questionString,
                answers = listOfAnswers,
                correctAnswer = correctAnswer
            };

            await Clients.Group(urlString).SendAsync("ReadyQuestion", questionToReturn);

            //create a service to ping incorrect answers or just send incorrect answers cause why not this is
            //taking too much of my damn time
            //await Task.Delay(5000);


            //Lobby lobby = dbContext.Lobbys.Include(lobby => lobby.triviaQuestions).AsSplitQuery().First(m => m.UrlString == urlString);
            //if (lobby.triviaQuestions[lobby.questionNumber].Id == question.Id)
            //{
            //    Console.WriteLine("TIME LIMIT BREACHED");
            //    await Clients.Group(urlString).SendAsync("DoneQuestion", questionToReturn);
            //}

        }
        public async Task recieveAnswer(SendAnswerClientToServerMessage message)
        {
           
                Lobby lobby = dbContext.Lobbys.Include(lobby => lobby.users).
                Include(lobby => lobby.triviaQuestions).
                ThenInclude(question => question.answers).AsSplitQuery().
                First(m => m.UrlString == message.UrlString);
                User user = dbContext.Users.First(m => m.Id == message.UserId);

           
                try
                {
                    TriviaAnswer answer = lobby.triviaQuestions[lobby.questionNumber].answers.First(m => m.answerString == message.answerString);
                    if (answer.isCorrect)
                    {
                        user.isCorrect = true;
                        user.score += 500;
                    }
                    else
                    {
                        user.isCorrect = false;
                    }
                }
                catch (Exception ex)
                {
                    user.isCorrect = false;
                }




           
           
            lobby.answersComplete++;
            //await Clients.Client(Context.ConnectionId).SendAsync("qUEST", user);
            var saved = false;
            while (!saved)
            {
                try
                {
                    // Attempt to save changes to the database
                    await dbContext.SaveChangesAsync();
                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        if (entry.Entity is Lobby)
                        {
                            var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();

                            foreach (var property in proposedValues.Properties)
                            {
                                if (property.Name == "answersComplete")
                                {
                                    var databaseValue = databaseValues[property];
                                    proposedValues[property] = (int)databaseValue + 1;
                                    break;
                                }
                              
                            
                            }

                            // Refresh original values to bypass next concurrency check
                            entry.OriginalValues.SetValues(databaseValues);
                        }
                        else
                        {
                            throw new NotSupportedException(
                                "Don't know how to handle concurrency conflicts for "
                                + entry.Metadata.Name);
                        }
                    }
                }
            }



            await checkIfEveryoneCompletedAnswersInLobby(message.UrlString);

            //if lobby.questions == DONE {do something}
            //send back if user correct or not
            //foreach user in lobby users
            //Clients.Client(Context.ConnectionId).SendAsync("UserCreated", user.IsCorrect);
            //wait 10 second
            //dispense next question
        }
        public async Task checkIfEveryoneCompletedAnswersInLobby(String urlString)
        {
            Lobby lobby = await dbContext.Lobbys.Include(lobby => lobby.users).
                Include(lobby => lobby.triviaQuestions).
                ThenInclude(question => question.answers).
                FirstAsync(m => m.UrlString == urlString);
            if (lobby.answersComplete == lobby.users.Count)
            {
                foreach (User lobbyUser in lobby.users)
                {
                    //send back if user correct or not
                    await Clients.Client(lobbyUser.ConnectionID).SendAsync("CorrectOrIncorrect", lobbyUser.isCorrect);
                }

                await Task.Delay(5000);

                await Clients.Group(urlString).SendAsync("UpdateScores", lobby.users);

                lobby.questionNumber++;
                lobby.answersComplete = 0;
                await dbContext.SaveChangesAsync();

                await Task.Delay(5000);
                try
                {
                    dispenseQuestion(lobby.UrlString, lobby.triviaQuestions[lobby.questionNumber]);
                }
                catch (Exception ex)
                {
                    await Clients.Group(urlString).SendAsync("LobbyDone", lobby.users);
                }

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

                    dispenseQuestion(message.UrlString, lobby.triviaQuestions[lobby.questionNumber]);
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
    class Question
    {
        string question;
        string[] answers;
        string correctAnswer;
    }
}