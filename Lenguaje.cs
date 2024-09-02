using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        List<Variable> listaVariables;
        public Lenguaje()
        {
            listaVariables = new List<Variable>();
        }
        public Lenguaje(String nombre) : base(nombre)
        {
            listaVariables = new List<Variable>();
        }
        //Programa  -> Librerias? Variables? Main
        public void Program()
        {
            if (getContenido() == "using")
            {
                Librerias();
            }
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            Main();
            ImprimeVariables(); 
        }
        //Librerias -> using ListaLibrerias; Librerias?
        private void Librerias()
        {
            match("using");
            ListaLibrerias();
            match(Token.Tipos.FinSentencia);
            if (getContenido() == "using")
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
        Variable.TipoD getTipo(String TipoDato)
        {
            Variable.TipoD Tipo = Variable.TipoD.Char;
            switch (TipoDato)
            {
                case "int": Tipo = Variable.TipoD.Int; break;
                case "float": Tipo = Variable.TipoD.Float; break;
            }
            return Tipo;
        }
        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            Variable.TipoD Tipo = getTipo(getContenido());
            match(Tipos.TipoDato);
            ListaIdentificadores(Tipo);
            match(";");
        }
        private void ImprimeVariables()
        {
            foreach (Variable v in listaVariables)
            {
                log.WriteLine(v.getNombre()+ " (" + v.getTipo() +")"+ " = " + v.getValor());
            }
        }
        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoD t)
        {
            listaVariables.Add(new Variable(getContenido(), t));
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                ListaIdentificadores(t);
            }

        }
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones()
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }
            match("}");

        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            Instruccion();
            if (getContenido() != "}")
            {
                ListaInstrucciones();
            }

        }
        //Instruccion -> Console | If | While | do | For | Variables | Asignacion
        private void Instruccion()
        {
            if (getContenido() == "Console")
            {
                console();
            }
            else if (getContenido() == "if")
            {
                If();
            }
            else if (getContenido() == "while")
            {
                While();
            }
            else if (getContenido() == "do")
            {
                Do();
            }
            else if (getContenido() == "for")
            {
                For();
            }
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion();
            }

        }
        //Asignacion -> Identificador = Expresion;
        private void Asignacion()
        {
            match(Tipos.Identificador);
            match("=");
            Expresion();
            match(Tipos.FinSentencia);
        }
        //If -> if (Condicion) bloqueInstrucciones | instruccion
        //     (else bloqueInstrucciones | instruccion)?

        private void If()
        {
            match("if");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            if (getContenido() == "else")
            {
                match("else");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones();
                }
                else
                {
                    Instruccion();
                }
            }


        }
        //Condicion -> Expresion operadorRelacional Expresion
        private void Condicion()
        {
            Expresion();
            match(Tipos.OpRelacional);
            Expresion();

        }
        //While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While()
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
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
            if (getContenido() == "{")
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
            if (getContenido() == "{")
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
            if (getContenido() == "++")
            {
                match(Tipos.IncTermino);
            }
            else if (getContenido() == "--")
            {
                match(Tipos.IncTermino);
            }

        }
        //Console -> Console.(WriteLine|Write) (cadena); |
        //           Console.(Read | ReadLine) ();
        private void console()
        {
            match("Console");
            match(".");
            if (getContenido() == "WriteLine")
            {
                match("WriteLine");
                match("(");
                match(Tipos.Cadena);
                match(")");
                match(";");
            }
            else if (getContenido() == "Write")
            {
                match("Write");
                match("(");
                match(Tipos.Cadena);
                match(")");
                match(";");
            }
            else if (getContenido() == "Read")
            {
                match("Read");
                match("(");
                match(")");
                match(";");
            }
            else if (getContenido() == "ReadLine")
            {
                match("ReadLine");
                match("(");
                match(")");
                match(";");
            }
        }
        //
        //Main      -> static void Main(string[] args) BloqueInstrucciones 
        private void Main()
        {
            match("static");
            match("void");
            match("Main");
            match("(");
            match("string");
            match("[");
            match("]");
            match("args");
            match(")");
            BloqueInstrucciones();


        }
        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OpTermino)
            {
                match(Tipos.OpTermino);
                Termino();
            }

        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();

        }
        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OpFactor)
            {
                match(Tipos.OpFactor);
                Factor();
            }

        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                if(getClasificacion() == Tipos.TipoDato)
                {
                    match(Tipos.TipoDato);
                    match(")");
                    match("("); 
                }
                Expresion();
                match(")");
            }
        }

    }
}