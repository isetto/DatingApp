using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }       
        public string PublicId { get; set; }
        public AppUser AppUser { get; set; } //by this line and the one below we tell to migration that Photo must have userId and it is not possible to create Photo without user
        public int AppUserId { get; set; }
    }
}