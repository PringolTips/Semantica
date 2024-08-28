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

        private void Variables()
        {
            
        }
        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores()
        {
            
        }
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones()
        {

        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            
        }
        //Instruccion -> Console | If | While | do | For | Asignacion
        private void Instruccion()
        {
            
        }
        //Asignacion -> Identificador = Expresion;
        private void Asignacion()
        {
            
        }
        //If -> if (Condicion) bloqueInstrucciones | instruccion
        //     (else bloqueInstrucciones | instruccion)?
        //Condicion -> Expresion operadorRelacional Expresion
        private void Condicion()
        {

        }
//While -> while(Condicion) bloqueInstrucciones | instruccion
//Do -> do 
//        bloqueInstrucciones | intruccion 
//      while(Condicion);
//For -> for(Asignacion Condicion; Incremento) 
//       BloqueInstrucciones | Intruccion 
    
    
        //Incremento -> Identificador ++ | --
        private void Incremento()
        {
            
        }
        //Console -> Console.(WriteLine|Write) (cadena); |
        //           Console.(Read | ReadLine) ();
//
        //Main      -> static void Main(string[] args) BloqueInstrucciones 
        private void Main()
        {
            
        }   
        //Expresion -> Termino MasTermino
        private void Expresion()
        {

        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            
        }
        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            
        }

    }


}