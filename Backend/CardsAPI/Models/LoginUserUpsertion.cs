using System;
using System.ComponentModel.DataAnnotations;

namespace CardsAPI.Models
{
    public class LoginUserUpsertion
    {
        [EmailAddress]
        [Required(ErrorMessage = "Mail Id is required")]
        public string MailId { get; set; }
        [Required] public string Password { get; set; }
    }
}
