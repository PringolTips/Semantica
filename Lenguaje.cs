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
    12.ListaConcatenacion 30, 40, 50, 12, 0
    10. Quitar comillas y considerar el write
    9. Desarrollar lista de contcatenacion
    9. Emular el do
    10. Emular el for  -- 15 puntos
    11. Emular el while -- 15 puntos
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
            Variable.TipoD tipo = getTipo(Contenido);
            match(Tipos.TipoDato);
            ListaIdentificadores(tipo);
            match(";");
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
                match(";");
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
            float nuevoValor = v.valor;

            tipoDatoExpresion = Variable.TipoD.Char;

            if (Contenido == "=")
            {
                match("=");
                if (Contenido == "Console")
                {
                    match("Console");
                    match(".");
                    if (Contenido == "Read")
                    {
                        match("Read");
                        if (ejecutar)
                        {
                            float valor = Console.Read();
                        }
                        // 8
                    }
                    else
                    {
                        match("ReadLine");
                        nuevoValor = float.Parse("" + Console.ReadLine());
                        // 8
                    }
                    match("(");
                    match(")");
                }
                else
                {
                    Expresion();
                    nuevoValor = S.Pop();
                }
            }
            else if (Contenido == "++")
            {
                match("++");
                nuevoValor++;
            }
            else if (Contenido == "--")
            {
                match("--");
                nuevoValor--;
            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
                nuevoValor += S.Pop();
            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
                nuevoValor -= S.Pop();
            }
            else if (Contenido == "*=")
            {
                match("*=");
                Expresion();
                nuevoValor *= S.Pop();
            }
            else if (Contenido == "/=")
            {
                match("/=");
                Expresion();
                nuevoValor /= S.Pop();
            }
            else
            {
                match("%=");
                Expresion();
                nuevoValor %= S.Pop();
            }
            // match(";");
            if (AnalisisSemantico(v, nuevoValor))
            {
                if (ejecutar)
                    v.valor = nuevoValor;
            }
            else
            {
                // tipoDatoExpresion = 
                throw new Error("Semantico, no puedo asignar un " + tipoDatoExpresion +
                                " a un " + v.tipo, log, linea);
            }
            log.WriteLine(variable + " = " + nuevoValor);
        }
        private Variable.TipoD valorToTipo(float valor)
        {
            if (valor % 1 != 0)
            {
                return Variable.TipoD.Float;
            }
            else if (valor <= 255)
            {
                return Variable.TipoD.Char;
            }
            else if (valor <= 65535)
            {
                return Variable.TipoD.Int;
            }
            return Variable.TipoD.Float;
        }
        bool AnalisisSemantico(Variable v, float valor)
        {
            if (tipoDatoExpresion > v.tipo)
            {
                return false;
            }
            else if (valor % 1 == 0)
            {
                if (v.tipo == Variable.TipoD.Char)
                {
                    if (valor <= 255)
                        return true;
                }
                else if (v.tipo == Variable.TipoD.Int)
                {
                    if (valor <= 65535)
                        return true;
                }
                return false;
            }
            else
            {
                if (v.tipo == Variable.TipoD.Char ||
                    v.tipo == Variable.TipoD.Int)
                    return false;
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
            Expresion(); // E1
            string operador = Contenido;
            match(Tipos.OpRelacional);
            Expresion(); // E2
            float R2 = S.Pop();
            float R1 = S.Pop();
            switch (operador)
            {
                case ">": return R1 > R2;
                case ">=": return R1 >= R2;
                case "<": return R1 < R2;
                case "<=": return R1 <= R2;
                case "==": return R1 == R2;
                default: return R1 != R2;
            }
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
            {
                Instruccion(ejecutar);
            }
        }
        //Do -> do 
        //        bloqueInstrucciones | intruccion 
        //      while(Condicion);
        private void Do(bool ejecutar)
        {
            int cTemp = caracter-3;
            int lTemp = linea;
            bool resultado = false;
            do
            {
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
                resultado = Condicion() && ejecutar;
                match(")");
                match(";");
                if (resultado)
                {
                    caracter = cTemp;
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(cTemp, SeekOrigin.Begin);
                    nextToken();
                }
            } while (resultado);
        }
        //For -> for(Asignacion Condicion; Incremento) 
        //       BloqueInstrucciones | Intruccion
        private void For(bool ejecutar)
        {
            match("for");
            match("(");
            Asignacion(ejecutar);
            match(";");
            Condicion();
            match(";");
            Asignacion(ejecutar);
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
            char quitar = '"';
            String cadena = "";
            int condicion  = 0;
            match("Console");
            match(".");
            if (Contenido == "WriteLine")
            {
                match("WriteLine");
                condicion = 1;
            }
            else
            {
                match("Write");
            }
            match("(");
            if (Clasificacion == Tipos.Cadena)
            {
                if (ejecutar)
                {
                    cadena = Contenido;
                    cadena = cadena.Replace(quitar.ToString(), "");
                    if(condicion == 1)
                    Console.WriteLine(cadena);
                    else
                    Console.Write(cadena);
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
            match(Tipos.Identificador); // Validar que exista la variable
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
                    aCastear = getTipo(Contenido);
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");
                if (huboCast && aCastear != Variable.TipoD.Float)
                {
                    tipoDatoExpresion = aCastear;
                    float valor = S.Pop();
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