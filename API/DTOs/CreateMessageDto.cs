namespace API.DTOs
{
    public class CreateMessageDto
    {
        public string RecipientUsername { get; set; }   //Recipient
        public string Content { get; set; }
    }
}