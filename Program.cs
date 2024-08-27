using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semantica
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (Sintaxis L = new Sintaxis("prueba.cpp"))
                {
                    /*while(!L.finArchivo())
                    {
                        L.nextToken();
                    }*/
                    L.match(Token.Tipos.Numero);
                    L.match(Token.Tipos.OpTermino);
                    L.match(Token.Tipos.Identificador);
                    L.match(Token.Tipos.FinSentencia);


                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
