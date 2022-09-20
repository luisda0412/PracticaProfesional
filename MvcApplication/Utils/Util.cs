using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public class Util
{
    public static void ValidateErrors(Controller pController)
    {

        var listaErrores = pController.ModelState.Select(x => x.Value.Errors)
                     .Where(y => y.Count > 0)
                     .ToList();

        foreach (ModelErrorCollection item in listaErrores)
        {
            if (item.Count > 0)
                pController.ModelState.AddModelError("", item[0].ErrorMessage.ToString());
        }

    }


    public static List<string> GetModelStateErrors(ModelStateDictionary pModelState)
    {

        List<string> lista = new List<string>();

        var listaErrores = pModelState.Select(x => x.Value.Errors)
                     .Where(y => y.Count > 0)
                     .ToList();

        foreach (var item in pModelState)
        {
            lista.Add(item.Value.Errors[0].ErrorMessage);
        }


        return lista;

    }

}
