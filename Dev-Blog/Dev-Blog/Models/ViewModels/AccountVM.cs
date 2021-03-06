﻿using System.ComponentModel.DataAnnotations;

namespace Dev_Blog.Models.ViewModels
{
    public class AccountVM
    {
        [Required]
        [StringLength(25)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(35)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [StringLength(35)]
        public string ConfirmPassword { get; set; }

        [Required]
        [EmailAddress]
        [RegularExpression("^(?=.{1,64}@)[A-Za-z0-9_-]+(\\.[A-Za-z0-9_-]+)*@[^-][A-Za-z0-9-]+(\\.[A-Za-z0-9-]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Please enter a correct email address")]
        public string Email { get; set; }
    }
}