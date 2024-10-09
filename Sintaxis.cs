using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semantica
{
    public class Sintaxis: Lexico
    {
        public Sintaxis(){
            nextToken();
        }
        public Sintaxis(string name) : base(name){
            nextToken();
        }
        public void match(String espera){
            if(Contenido == espera){
                nextToken();
            }
            else
            {
                throw new Error("Sintaxis: se espera un "+ espera + " : " + Contenido + " ",log,linea);
            }
        }

        public void match(Tipos espera){
            if(Clasificacion == espera){ 
                nextToken();
            }
            else
            {
                throw new Error("Sintaxis: se espera un "+ espera + " : " + Contenido + " " , log, linea);
            }
        }
    }
}