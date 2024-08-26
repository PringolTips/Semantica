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
            if(getContenido() == espera){
                nextToken();
            }
            else
            {
                throw new Error("Sintaxis: se espera un "+espera,log);
            }
        }

        public void match(Token.Tipos espera){
            if(getClasificacion() == espera){ 
                nextToken();
            }
            else
            {
                throw new Error("Sintaxis: se espera un "+ espera , log);
            }
        }
    }
}