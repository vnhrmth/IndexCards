﻿using System;
using System.ComponentModel.DataAnnotations;

namespace CardsAPI.Models
{
    public class UserUpsertion
    {
        [Required] public string Name { get; set; }
        [Required] public string Password { get; set; }

        [Required(ErrorMessage = "Mail Id is required")]
        [EmailAddress]
        public string MailId { get; set; }
    }
}

