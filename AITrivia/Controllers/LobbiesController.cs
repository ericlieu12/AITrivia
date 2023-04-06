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
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        public async Task<ActionResult<Lobby>> CreateLobby([FromBody] string topic)
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

                //for (int j = 0; j < 10; j++)
                //{
                //    var question = new TriviaQuestion();
                //    question.answers = new List<TriviaAnswer>();
                //    question.questionString = "What is the answer to this really long test question 1 +" + j.ToString() + "?";
                //    for (int k = 0; k < 4; k++)
                //    {
                //        var answer = new TriviaAnswer();
                //        answer.answerString = "This is a really correct answer of a long string of a: " + k.ToString() + j.ToString();
                //        if (k == 1)
                //        {
                //            answer.isCorrect = true;
                //        }
                //        else
                //        {
                //            answer.isCorrect = false;
                //        }
                //        question.answers.Add(answer);
                //    }
                //    lobby.triviaQuestions.Add(question);
                //}
                messageContentToAPI systemMessageToGPT = new messageContentToAPI();
                systemMessageToGPT.role = "system";
                systemMessageToGPT.content = "You are an assitant that gives trivia questions in this JSON format with no line breaks: { question: 'the question', answers: ['first answer', 'second answer', 'third answer', 'fourth answer'],correctAnswer: 'answer'}";

                messageContentToAPI userMessageToGPT = new messageContentToAPI();
                userMessageToGPT.role = "user";
                userMessageToGPT.content = "Give me a trivia question for "+ topic + " with four answers and the correct answer, just the object";
                List<messageContentToAPI> chatMessagesToGPT = new List<messageContentToAPI>();
                chatMessagesToGPT.Add(systemMessageToGPT);
                chatMessagesToGPT.Add(userMessageToGPT);

                //generate questions
                for (int j = 0; j < 10; j++)
                {

                    using (HttpClient client = new HttpClient())
                    {
                        var uri = "https://api.openai.com/v1/chat/completions?model=gtp-4";



                        thingToTurnIntoJSONBody thingToPutIntoBody = new thingToTurnIntoJSONBody
                        {

                            messages = chatMessagesToGPT,
                            model = "gpt-3.5-turbo"
                        };



                        var req = new HttpRequestMessage(HttpMethod.Post, uri);
                        req.Content = new StringContent(JsonConvert.SerializeObject(thingToPutIntoBody), Encoding.UTF8, MediaTypeNames.Application.Json /* or "application/json" in older versions */);
                        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "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");
                        // This is the important part:

                        HttpResponseMessage resp = await client.SendAsync(req);

                        string jsonString = await resp.Content.ReadAsStringAsync();

                        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                        {
                            Converters = new List<JsonConverter> { new MessageConverter() }
                        };

                        ChatCompletion chatCompletion = JsonConvert.DeserializeObject<ChatCompletion>(jsonString);

                        // Parse the content property of the Message object
                        MessageContent messageContent = JsonConvert.DeserializeObject<MessageContent>(chatCompletion.choices[0].message.content);

                        messageContentToAPI assistantMessageThatYouGotBackFromChatGPT = new messageContentToAPI();
                        assistantMessageThatYouGotBackFromChatGPT.role = "assistant";
                        assistantMessageThatYouGotBackFromChatGPT.content = chatCompletion.choices[0].message.content;
                        chatMessagesToGPT.Add(assistantMessageThatYouGotBackFromChatGPT);

                        messageContentToAPI userMessageToGPTAskingForNewQuestion = new messageContentToAPI();
                        userMessageToGPTAskingForNewQuestion.role = "user";
                        userMessageToGPTAskingForNewQuestion.content = "Give me another trivia question for popsicles with four answers and the correct answer, just the object";
                        chatMessagesToGPT.Add(userMessageToGPTAskingForNewQuestion);

                        var question = new TriviaQuestion();
                        question.answers = new List<TriviaAnswer>();
                        question.questionString = messageContent.question;
                        for (int k = 0; k < 4; k++)
                        {
                            var answer = new TriviaAnswer();
                            answer.answerString = messageContent.answers[k];
                            if (messageContent.answers[k] == messageContent.correctAnswer)
                            {
                                answer.isCorrect = true;
                            }
                            else
                            {
                                answer.isCorrect = false;
                            }
                            question.answers.Add(answer);
                        }
                        lobby.triviaQuestions.Add(question);
                    }


                }


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
    public class messageContentToAPI
    {
        public string role;
        public string content;
    }
    public class thingToTurnIntoJSONBody
    {
        public List<messageContentToAPI> messages { get; set; }
        public string model { get; set; }
    }
    public class Usage
    {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
    }

    public class MessageContent
    {
        public string question { get; set; }
        public string[] answers { get; set; }
        public string correctAnswer { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class Choice
    {
        public Message message { get; set; }
        public string finish_reason { get; set; }
        public int index { get; set; }
    }

    public class ChatCompletion
    {
        public string id { get; set; }
        public string @object { get; set; }
        public int created { get; set; }
        public string model { get; set; }
        public Usage usage { get; set; }
        public Choice[] choices { get; set; }
    }
    public class MessageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Message);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            Message message = new Message();
            message.role = (string)jo["role"];
            message.content = jo["content"].ToString();
            return message;
        }


        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

      
    }
}
