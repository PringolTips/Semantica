using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
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
            match(Tipos.Identificador);
            if(getContenido() == ",")
            {
                match(",");
                ListaIdentificadores();
            }
            
        }
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones()
        {
            match("{");
            ListaInstrucciones();
            match("}");

        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            Instruccion();
            
        }
        //Instruccion -> Console | If | While | do | For | Asignacion
        private void Instruccion()
        {
           
            
        }
        //Asignacion -> Identificador = Expresion;
        private void Asignacion()
        {
            match(Tipos.Asignacion);
            match("=");
            Expresion();
        }
        //If -> if (Condicion) bloqueInstrucciones | instruccion
        //     (else bloqueInstrucciones | instruccion)?
        //Condicion -> Expresion operadorRelacional Expresion
        private void If()
        {   
            match("if");
            match("(");
            Condicion();
            match(")");
            if(getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
                Instruccion();
            
        }	
        private void Condicion()
        {

        }
        //While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While()
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if(getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
                Instruccion();


        }
        //Do -> do 
        //        bloqueInstrucciones | intruccion 
        //      while(Condicion);
        private void Do()
        {
            match("do");
            if(getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
                Instruccion();
            match("while");
            match("(");
            Condicion();
            match(")");
        }
        //For -> for(Asignacion Condicion; Incremento) 
        //       BloqueInstrucciones | Intruccion
        private void For()
        {
            match("for");
            match("(");
            Asignacion();
            Condicion();
            match(";");
            Incremento();
            match(")");
            if(getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
                Instruccion();
            
        } 
    
    
        //Incremento -> Identificador ++ | --
        private void Incremento()
        {
            match(Tipos.Identificador);
            if(getContenido() == "++")
            {
                match(Tipos.IncTermino);
            }
            else if(getContenido() == "--")
            {
                match(Tipos.IncTermino);
            }
            
        }
        //Console -> Console.(WriteLine|Write) (cadena); |
        //           Console.(Read | ReadLine) ();
        private void Console()
        {
            
            
        }
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