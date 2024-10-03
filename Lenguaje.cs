using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
/*
    1. Usar find en lugar del for each                                                  Listo
    2. Valiar que no existan varibles duplicadas                                        Listo
    3. Validar que existan las variables en las expressions matematicas
       Asignacion *
    4. 1.5 + 1.5 = 3 <- float porque float + float = float                              Listo
    5. Meter el valor de la variable al stack
    6. Asignar una expresión matematica a la variable al momento de declararla
       verificando la semantica
    7. Emular el if                                                                     Listo
    8. Validar que en el ReadLine se capturen solo numeros e implementar una exception  Listo
    12.ListaConcatenacion 30, 40, 50, 12, 0
    10. Quitar comillas y considerar el write                                           Listo
    9. Desarrollar lista de contcatenacion
    9. Emular el do                                                                     Listo
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
        private bool ImprimeVariables()
        {
            if (listaVariables.Count > 0)
            {
                listaVariables.Find(v =>
                {
                    log.WriteLine(v.nombre + " (" + v.tipo + ")" + " = " + v.valor);
                    return false;
                });
                return true;
            }
            else
            {
                return false;
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
        private bool ExisteVariable(string variableNombre)
        {
            return listaVariables.Exists(v => v.nombre == variableNombre);
        }
        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoD t)
        {
            if (listaVariables.Find(v => v.nombre == Contenido) != null)
            {
                throw new Error("Semantico: la variable " + Contenido + " ya fue declarada", log, linea);
            }
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
            match("}");

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
        //Asignacion -> Identificador = Expresion;
        private void Asignacion(bool ejecutar)
        {
            string variable = Contenido;
            match(Tipos.Identificador);
            if (!ExisteVariable(variable))
            {
                throw new Error("Semantico: La variable " + variable + " no ha sido declarada. ", log, linea);
            }
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
                            nuevoValor = Console.Read();
                        }
                    }
                    else
                    {
                        match("ReadLine");
                        if (!float.TryParse(Console.ReadLine(), out nuevoValor))
                        {
                            throw new Error("Error Semantico: solo se permiten números.", log, linea);
                        }
                        else
                            v.valor = nuevoValor;
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
            int cTemp = caracter - 3;
            int lTemp = linea;
            bool resultado = false;
            match("while");
            match("(");
            resultado = Condicion() && ejecutar;
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
            int cTemp = caracter - 3;
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
            int condicion = 0;
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
                    match(Tipos.Cadena);
                    if (condicion == 1)
                    {
                        if (Contenido == "+")
                        {
                            String temp = listaConcatenacion();
                            Console.WriteLine(cadena + temp);
                        }
                        else
                        {
                            Console.WriteLine(cadena);
                        }

                    }

                    else
                    {
                        if (Contenido == "+")
                        {
                            String temp = listaConcatenacion();
                            Console.Write(cadena + temp);
                        }
                        else
                        {
                            Console.Write(cadena);
                        }

                    }
                        
                }

            }
            match(")");
            match(";");
        }

        string listaConcatenacion()
        {
            match("+");
            if (!ExisteVariable(Contenido))
            {
                throw new Error("Semantico: la variable no existe: " + Contenido, log, linea);
            }
            var v = listaVariables.Find(variable => variable.nombre == Contenido);
            string resultado = v.valor.ToString();
            match(Tipos.Identificador);
            if (Contenido == "+")
            {
                resultado += listaConcatenacion();
            }
            return resultado;
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
                if (!ExisteVariable(Contenido))
                {
                    throw new Error("Semantico: La variable " + Contenido + " no ha sido declarada. ", log, linea);
                }
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