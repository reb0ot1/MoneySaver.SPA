﻿using System.ComponentModel.DataAnnotations;

namespace MoneySaver.SPA.Models
{
    public class UserForAuthenticationDto
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
