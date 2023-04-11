﻿using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public interface IServiceCajaChica
    {
        IEnumerable<Arqueos_Caja> GetArqueos();
        IEnumerable<Caja_Chica> GetCajaChica();
        Caja_Chica GetCajaChicaLast();
        IEnumerable<Caja_Chica> GetCajaByFecha(DateTime fecha);
        void Save(Caja_Chica caja);

        void AbrirArqueo(Arqueos_Caja caja);
        void SaveArqueo(Arqueos_Caja arq);

        Arqueos_Caja GetArqueoLast();


        void Eliminar(int id);
    }
}
