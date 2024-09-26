using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
/*
    1. Usar find en lugar del for each
    2. Valiar que no existan varibles duplicadas
    3. Validar que existan las variables en las expressions matematicas
       Asignacion 
    4. 1.5 + 1.5 = 3 <- float porque float + float = float *
    5. Meter el valor de la variable al stack
    6. Asignar una expresión matematica a la variable al momento de declararla
       verificando la semantica
    7. Emular el if  *
    8. VAlidar que en el ReadLine se capturen solo numeros e implementar una exception
    10. Quitar comillas y considerar el write
    9. Desarrollar lista de contcatenacion
    9. Emular el do
    10. Emular el for
    11. Emular el while
*/
namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        private List<Variable> listaVariables;
        private Stack<float> S;
        private Variable.TipoD tipoDatoExpresion;
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
        private void BloqueInstrucciones(bool ejecutar)
        {
            match("{");
            if (Contenido != "}")
            {
                ListaInstrucciones(ejecutar);
            }
            match("}");
        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool ejecutar)
        {
            Instruccion(ejecutar);
            if (Contenido != "}")
            {
                ListaInstrucciones(ejecutar);
            }

        }
        //Instruccion -> Console | If | While | do | For | Variables | Asignacion
        private void Instruccion(bool ejecutar)
        {
            if (Contenido == "Console")
            {
                console(ejecutar);
            }
            else if (Contenido == "if")
            {
                If(ejecutar);
            }
            else if (Contenido == "while")
            {
                While(ejecutar);
            }
            else if (Contenido == "do")
            {
                Do(ejecutar);
            }
            else if (Contenido == "for")
            {
                For(ejecutar);
            }
            if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion(ejecutar);
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
        private void Asignacion(bool ejecutar)
        {
            string variable = Contenido;
            match(Tipos.Identificador);

            var v = listaVariables.Find(delegate (Variable x) { return x.nombre == variable; });
            float nuevovalor = v.valor;

            tipoDatoExpresion = Variable.TipoD.Char;
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
                if (Contenido == "Consola")
                {
                    match("Read");
                    float valor = Console.Read();
                }
                else
                {
                    match("ReadLine");
                    nuevovalor = float.Parse("" + Console.ReadLine());
                    //8
                }
                match("(");
                match(")");

            }
            match(";");
            if (AnalisisSemnatico(v, nuevovalor))
            {
                if (ejecutar)
                    v.valor = nuevovalor;
            }
            else
            {

                throw new Error("Semantico: No puedo asignar un " + tipoDatoExpresion
                + "a un " + v.tipo, log, linea);
            }
            log.WriteLine(variable + " = " + nuevovalor);
        }
        private String valorToTipo(float valor)
        {
            if (valor % 1 != 0)
            {
                return "float";
            }
            else if (valor <= 255)
            {
                return "char";
            }
            else if (valor <= 65535)
            {
                return "int";
            }
            return "float";
        }
        bool AnalisisSemnatico(Variable v, float valor)
        {
            if (tipoDatoExpresion > v.tipo)
            {
                return false;
            }
            else if (valor % 1 != 0)
            {
                if (v.tipo == Variable.TipoD.Char)
                {
                    if (valor <= 255)
                    {
                        return true;
                    }

                }
                else if (v.tipo == Variable.TipoD.Int)
                {
                    if (valor <= 65535)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                if (v.tipo == Variable.TipoD.Char || v.tipo == Variable.TipoD.Char)
                {
                    return true;
                }
            }
            return true;
        }
        //If -> if (Condicion) bloqueInstrucciones | instruccion
        //     (else bloqueInstrucciones | instruccion)?
        private void If(bool ejecutar)
        {
            match("if");
            match("(");
            bool resultado = Condicion();
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(resultado && ejecutar);
            }
            else
            {
                Instruccion(resultado && ejecutar);
            }
            if (Contenido == "else")
            {
                match("else");
                if (Contenido == "{")
                {
                    BloqueInstrucciones(!resultado && ejecutar);
                }
                else
                {
                    Instruccion(!resultado && ejecutar);
                }
            }
        }
        //Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion()
        {
            Expresion(); //Expresion1
            string operador = Contenido;
            match(Tipos.OpRelacional);
            Expresion(); //Expresion2
            float R2 = S.Pop();
            float R1 = S.Pop();
            switch (operador)
            {
                case ">": return R1 > R2;
                case ">=": return R1 >= R2;
                case "<": return R1 < R2;
                case "<=": return R1 <= R2;
                case "==": return R1 == R2;
                case "!=": return R1 != R2;
            }
            return false;
        }
        //While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(bool ejecutar)
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(ejecutar);
            }
            else
                Instruccion(ejecutar);
        }
        //Do -> do 
        //        bloqueInstrucciones | intruccion 
        //      while(Condicion);
        private void Do(bool ejecutar)
        {
            int cTemp = caracter+2;
            int lTemp = linea;
            match("do");
            if (Contenido == "{")
            {
                BloqueInstrucciones(ejecutar);
            }
            else
            {
                Instruccion(ejecutar);
            }
            match("while");
            match("(");
            bool resultado = Condicion();
            if(resultado)
            {
                caracter = cTemp;
                linea = lTemp;
                archivo.Seek(cTemp, SeekOrigin.Begin);
            }
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) 
        //       BloqueInstrucciones | Intruccion
        private void For(bool ejecutar)
        {
            match("for");
            match("(");
            Asignacion(ejecutar);
            Condicion();
            match(";");
            Incremento();
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(ejecutar);
            }
            else
                Instruccion(ejecutar);
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
        private void console(bool ejecutar)
        {
            match("Console");
            match(".");
            if (Contenido == "WriteLine")
            {
                match("WriteLine");
            }
            else if (Contenido == "Write")
            {
                match("Write");
            }
            match("(");
            if (Clasificacion == Tipos.Cadena)
            {
                if (ejecutar)
                {
                    Console.WriteLine(Contenido);
                }
                match(Tipos.Cadena);
                if (Contenido == "+")
                {
                    listaConcatenacion();
                }
            }
            match(")");
            match(";");
        }

        string listaConcatenacion()
        {
            match("+");
            match(Tipos.Identificador); //Validar que existe la variable 
            if (Contenido == "+")
            {
                listaConcatenacion();
            }
            return "";
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
            BloqueInstrucciones(true);
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
                if (tipoDatoExpresion < valorToTipo(float.Parse(Contenido)))
                {
                    tipoDatoExpresion = valorToTipo(float.Parse(Contenido));
                }
                match(Tipos.Numero);
            }
            else if (Clasificacion == Tipos.Identificador)
            {
                //5.
                var v = listaVariables.Find(delegate (Variable x) { return x.nombre == Contenido; });
                S.Push(v.valor);
                if (tipoDatoExpresion < v.tipo)
                {
                    tipoDatoExpresion = v.tipo;
                }
                match(Tipos.Identificador);
            }
            else
            {
                bool huboCast = false;
                Variable.TipoD aCastear = Variable.TipoD.Char;
                match("(");
                if (Clasificacion == Tipos.TipoDato)
                {
                    huboCast = true;
                    aCastear = Tipos.TipoDato;
                    //aCastear = Tipo(Contenido);
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                    //12
                    // Sacar un elemnto del stack
                    //Castearlo
                    //Meterlo casteado
                }
                Expresion();
                match(")");
                if (huboCast && aCastear != Variable.TipoD.Float)
                {
                    tipoDatoExpresion = aCastear;
                    float valor = S.Pop();
                    // Castearlo
                    if (aCastear == Variable.TipoD.Char)
                    {
                        valor %= 256;
                    }
                    else
                    {
                        valor %= 65536;
                    }

                    S.Push(valor);
                }
            }
        }
    }
}