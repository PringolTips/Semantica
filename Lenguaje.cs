using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/*
    1. Colocar el numero de linea en errores lexicos y sintaticticos
    2. Cambiar la clase token por atributis publicos (get ,  set)
    3. Cambiar los constructores de la clase lexico usando parametros por default
*/

namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        public Lenguaje()
        {

        }
        public Lenguaje(String nombre) : base(nombre)
        {

        }
        //Programa  -> Librerias? Variables? Main
        public void Program()
        {
            Librerias();
            Variables();
            Main();
        }
        //Librerias -> using ListaLibrerias; Librerias?
        private void Librerias()
        {
            match("using");
            ListaLibrerias();
            match(Token.Tipos.FinSentencia);
            if (getContenido()  == "using" )
            {
                Librerias();
            }
            

        }
        //ListaLibrerias -> identificador (.ListaLibrerias)?

        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (getContenido() == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }

    }


}