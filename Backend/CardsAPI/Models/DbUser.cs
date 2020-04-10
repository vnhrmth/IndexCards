using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CardsAPI.Models
{
    public class DbUser
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string MailId { get; set; }
        public List<DbTopic> Topics { get; set; }
    }
}