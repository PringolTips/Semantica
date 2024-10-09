using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semantica
{
    public class Token
    {
        public enum Tipos
        {
            Identificador, Numero, FinSentencia, OpTermino, OpFactor,
            OpLogico, OpRelacional, OpTernario, Asignacion, IncTermino,
            IncFactor, Cadena, Inicio, Fin, Caracter, TipoDato, Ciclo, 
            Condicion
            
        };
        public Token()
        {
            Contenido = "";
        }
         public string Contenido
        {get;set;}
        public Tipos Clasificacion
        {get;set;}

    }
}