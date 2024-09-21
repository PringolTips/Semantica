using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
/*
    1. Usar find en lugar del for each
    2. Valiar que no existan varibles duplicadas
    3. Validar que existan las variables en las expressions matematicas
       Asignacion 
*/
namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        private List<Variable> listaVariables;
        private Stack<float> S;
        public Lenguaje(String nombre = "prueba.cpp")
        {
            log.WriteLine("Analisis Sintactico");
            listaVariables = new List<Variable>();
            S = new Stack<float>();
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
        private void ImprimeVariables()
        {
            foreach (Variable v in listaVariables)
            {
                log.WriteLine(v.nombre + " (" + v.tipo + ")" + " = " + v.valor);
            }
        }
        //Programa Librerias? Variables? Main
        public void Program()
        {
            if (Contenido == "using")
            {
                Librerias();
            }
            if (Clasificacion == Tipos.TipoDato)
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
            match(";");
            if (Contenido == "using")
            {
                Librerias();
            }
        }
        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (Contenido == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }
        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            Variable.TipoD Tipo = getTipo(Contenido);
            match(Tipos.TipoDato);
            ListaIdentificadores(Tipo);
            match(";");
            /*if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }*/
        }
        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoD t)
        {
            listaVariables.Add(new Variable(Contenido, t));
            match(Tipos.Identificador);
            if (Contenido == ",")
            {
                match(",");
                ListaIdentificadores(t);
            }
        }
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones()
        {
            match("{");
            if (Contenido != "}")
            {
                ListaInstrucciones();
            }
            match("}");
        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            Instruccion();
            if (Contenido != "}")
            {
                ListaInstrucciones();
            }

        }
        //Instruccion -> Console | If | While | do | For | Variables | Asignacion
        private void Instruccion()
        {
            if (Contenido == "Console")
            {
                console();
            }
            else if (Contenido == "if")
            {
                If();
            }
            else if (Contenido == "while")
            {
                While();
            }
            else if (Contenido == "do")
            {
                Do();
            }
            else if (Contenido == "for")
            {
                For();
            }
            if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion();
            }
        }
        private void Dimensionvariable(string nombre, float valor)
        {
            if (getTipo(nombre) == Variable.TipoD.Char && valor > 255)
            {
                throw new Error(" Semantico: La variable   (" + nombre + ") esta fuera de rango", log);
            }
            else if (getTipo(nombre) == Variable.TipoD.Int && valor > 65535)
            {
                throw new Error(" Semantico: La variable   (" + nombre + ") esta fuera de rango", log);
            }
        }
        //Asignacion -> Identificador = Expresion;
        private void Asignacion()
        {
            string variable = Contenido;
            match(Tipos.Identificador);

            var v = listaVariables.Find(delegate (Variable x) { return x.nombre == variable; });
            float nuevovalor = v.valor;
            if (Contenido == "++")
            {
                match("++");
                //Dimensionvariable(variable, ObtenerValor(variable) + 1);
                nuevovalor++;
            }
            else if (Contenido == "--")
            {
                match("--");
                // Dimensionvariable(variable, ObtenerValor(variable) - 1);
                nuevovalor--;

            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
                nuevovalor += S.Pop();

            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
                nuevovalor -= S.Pop();

            }
            else if (Contenido == "*=")
            {
                match("*=");
                Expresion();
                nuevovalor *= S.Pop();
            }
            else if (Contenido == "/=")
            {
                match("/=");
                Expresion();
                nuevovalor /= S.Pop();
            }
            else if (Contenido == "%=")
            {
                match("%=");
                Expresion();
                nuevovalor %= S.Pop();
            }
            else
            {
                match("=");
                Expresion();
                nuevovalor = S.Pop();
            }
            match(";");
            if (AnalisisSemnatico(variable, nuevovalor))
            {
                v.valor = nuevovalor;
            }
            log.WriteLine(variable + " = " + nuevovalor);
        }
        bool AnalisisSemnatico(string variable, float valor)
        {
            return true;

        }
        //If -> if (Condicion) bloqueInstrucciones | instruccion
        //     (else bloqueInstrucciones | instruccion)?
        private void If()
        {
            match("if");
            match("(");
            Condicion();
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            if (Contenido == "else")
            {
                match("else");
                if (Contenido == "{")
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
            if (Contenido == "{")
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
            if (Contenido == "{")
            {
                BloqueInstrucciones();
            }
            else
                Instruccion();
            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");
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
            if (Contenido == "{")
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
            if (Contenido == "++")
            {
                match(Tipos.IncTermino);
            }
            else if (Contenido == "--")
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
            if (Contenido == "WriteLine")
            {
                match("WriteLine");
                match("(");
                match(Tipos.Cadena);
                match(")");
                match(";");
            }
            else if (Contenido == "Write")
            {
                match("Write");
                match("(");
                match(Tipos.Cadena);
                match(")");
                match(";");
            }
            else if (Contenido == "Read")
            {
                match("Read");
                match("(");
                match(")");
                match(";");
            }
            else if (Contenido == "ReadLine")
            {
                match("ReadLine");
                match("(");
                match(")");
                match(";");
            }
        }
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
            ImprimeStack();
        }
        //Expresion -> Termino MasTermino
        private void
        Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (Clasificacion == Tipos.OpTermino)
            {
                string operador = Contenido;
                match(Tipos.OpTermino);
                Termino();
                float R2 = S.Pop();
                float R1 = S.Pop();
                switch (operador)
                {
                    case "+": S.Push(R1 + R2); break;
                    case "-": S.Push(R1 - R2); break;
                }
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
            if (Clasificacion == Tipos.OpFactor)
            {
                string operador = Contenido;
                match(Tipos.OpFactor);
                Factor();
                float R2 = S.Pop();
                float R1 = S.Pop();
                switch (operador)
                {
                    case "*": S.Push(R2 * R1); break;
                    case "/": S.Push(R1 / R2); break;
                    case "%": S.Push(R1 % R2); break;
                }
            }
        }
        private void ImprimeStack()
        {
            log.WriteLine("Stack: ");
            foreach (float e in S)
            {
                log.Write(e + " ");
            }
            log.WriteLine();
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (Clasificacion == Tipos.Numero)
            {
                S.Push(float.Parse(Contenido));
                match(Tipos.Numero);
            }
            else if (Clasificacion == Tipos.Identificador)
            {
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                if (Clasificacion == Tipos.TipoDato)
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