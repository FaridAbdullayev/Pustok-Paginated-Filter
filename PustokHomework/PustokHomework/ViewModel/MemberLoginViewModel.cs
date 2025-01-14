﻿using System.ComponentModel.DataAnnotations;

namespace PustokHomework.ViewModel
{
    public class MemberLoginViewModel
    {
        [MaxLength(25)]
        [MinLength(5)]
        [Required]
        public string UserName { get; set; }

        [MaxLength(25)]
        [MinLength(8)]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
