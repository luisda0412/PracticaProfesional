using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.ViewModel
{
    public class LoginViewModel
    {
        [Display(Name = "Correo Electrónico")]
        [EmailAddress(ErrorMessage = "Digite un {0} válido")]
        public string Email { get; set; }
    }
}