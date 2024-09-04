using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semantica
{
    public class Sintaxis : Lexico
    {
        public Sintaxis()
        {
            nextToken();
        }
        public Sintaxis(string nombre) : base(nombre)
        {
            nextToken();
        }
        public void match(string espera)
        {
            if (Contenido == espera)
            {
                nextToken();
            }
            else
            {
                throw new Error("Sintaxis: se espera un: " + espera + "( " + Contenido + " )" + "En la linea: " + linea, log);
            }
        }
        public void match(Tipos espera)
        {
            if (Clasificacion == espera)
            {
                nextToken();
            }
            else
            {
                throw new Error("Sintaxis: se espera un: " + espera + "( " + Contenido + " )" + "En la linea: " + linea,log);
            }
        }
    }
}