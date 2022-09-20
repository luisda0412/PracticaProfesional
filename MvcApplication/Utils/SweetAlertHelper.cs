using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication.Util
{
    public class SweetAlertHelper
    {
        public static string Mensaje(string titulo, string mensaje, SweetAlertMessageType tipoMensaje)
        {
            return "swal({title: '" + titulo + "',text: '" + mensaje + "',type: '" + tipoMensaje + "'});";
        }
    }

    public enum SweetAlertMessageType
    {
        warning, error, success, info
    }
}
