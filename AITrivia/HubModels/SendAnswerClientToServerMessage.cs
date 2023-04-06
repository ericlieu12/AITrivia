namespace AITrivia.HubModels
{
    public class SendAnswerClientToServerMessage
    {
        public int UserId { get; set; }

        public string UrlString { get; set; }

        public string answerString { get; set; }
    }
}
