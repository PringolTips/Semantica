using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/*
    1. Usar find en lugar del for each                                                  Listo
    2. Valiar que no existan varibles duplicadas                                        Listo
    3. Validar que existan las variables en las expressions matematicas
       Asignacion                                                                       Listo
    4. 1.5 + 1.5 = 3 <- float porque float + float = float                              Listo
    5. Meter el valor de la variable al stack                                           Listo
    6. Asignar una expresión matematica a la variable al momento de declararla
       verificando la semantica                                                         Listo
    7. Emular el if                                                                     Listo
    8. Validar que en el ReadLine se capturen solo numeros e implementar una exception  Listo
    12.ListaConcatenacion 30, 40, 50, 12, 0                                             Listo
    10. Quitar comillas y considerar el write                                           Listo
    9. Desarrollar lista de concatenacion                                               Listo
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
        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            if (Contenido == "using")
            {
                Librerias();
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
            if (Contenido == "using")//Caso base de la recursividad
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
            Variable.TipoD tipoDato = getTipo(Contenido);
            match(Tipos.TipoDato);
            ListaIdentificadores(tipoDato);
            match(";");
        }
        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoD t)
        {
            if (listaVariables.Find(v => v.nombre == Contenido) != null)
            {
                throw new Error("Semantico: la variable " + Contenido + " ya fue declarada", log, linea);
            }
            listaVariables.Add(new Variable(Contenido, t));
            var variable = listaVariables.Find(delegate (Variable x) { return x.nombre == Contenido; });
            match(Tipos.Identificador);

            if (Contenido == "=")
            {
                match("=");
                Expresion();
                float valor = S.Pop();

                if (AnalisisSemantico(variable, valor))
                {
                    variable.valor = valor;
                    S.Push(valor);
                    log.WriteLine(variable.nombre + " = " + valor);
                }
                else
                {
                    throw new Error("Semantico, no puedo asignar un " + tipoDatoExpresion + " a un " + variable.tipo, log, linea);
                }
            }
            if (Contenido == ",")
            {
                match(",");
                ListaIdentificadores(t);
            }
        }
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones(bool evaluacion)
        {
            match("{");
            if (Clasificacion != Tipos.Fin)
            {
                ListaInstrucciones(evaluacion);
            }
            match("}");
        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (Contenido != "}")
            {
                ListaInstrucciones(evaluacion);
            }
        }
        //Instruccion -> Console | If | While | do | For | Asignacion
        private void Instruccion(bool evaluacion)
        {
            if (Contenido == "Console")
            {
                console(evaluacion);
            }
            else if (Contenido == "if")
            {
                If(evaluacion);
            }
            else if (Contenido == "while")
            {
                While(evaluacion);
            }
            else if (Contenido == "do")
            {
                Do(evaluacion);
            }
            else if (Contenido == "for")
            {
                For(evaluacion);
            }
            else if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion(evaluacion);
                match(";");
            }
        }
        private bool ExisteVariable(string variableNombre)
        {
            return listaVariables.Exists(v => v.nombre == variableNombre);
        }
        //Identificador = Expresion;
        //            -> Identificador (++ | --); INC o DEC
        //            -> Identificador (+= | -= ) Expresion; PUSH o POP
        //            -> Identificador (*= | /= | %= ) Expresion; PUSH o POP
        private void Asignacion(bool Condicion)
        {
            String variable = Contenido;
            match(Tipos.Identificador);
            if (!ExisteVariable(variable))
            {
                throw new Error("Semantico: La variable " + variable + " no ha sido declarada. ", log, linea);
            }
            var v = listaVariables.Find(delegate (Variable x) { return x.nombre == variable; });
            float nuevoValor = v.valor;
            tipoDatoExpresion = Variable.TipoD.Char;

            if (Contenido == "++")
            {
                match("++");
                if (Condicion)
                {
                    nuevoValor++;
                }
            }
            else if (Contenido == "--")
            {
                match("--");
                if (Condicion)
                {
                    nuevoValor--;

                }
            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
                if (Condicion)
                {
                    nuevoValor += S.Pop();
                }
            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
                if (Condicion)
                {
                    nuevoValor -= S.Pop();
                }
            }
            else if (Contenido == "*=")
            {
                match("*=");
                Expresion();
                if (Condicion)
                {
                    nuevoValor *= S.Pop();
                }
            }
            else if (Contenido == "/=")
            {
                match("/=");
                Expresion();
                if (Condicion)
                {
                    nuevoValor /= S.Pop();
                }
            }
            else if (Contenido == "%=")
            {
                match("%=");
                Expresion();
                if (Condicion)
                {
                    nuevoValor %= S.Pop();
                }
            }
            else if (Contenido == "=")
            {
                match("=");
                if (Contenido == "Console")
                {
                    match("Console");
                    match(".");
                    if (Contenido == "ReadLine")
                    {
                        match("ReadLine");
                        match("(");
                        match(")");
                        if (Condicion)
                        {
                            if (!float.TryParse(Console.ReadLine(), out nuevoValor))
                            {
                                throw new Error("Error Semantico: solo se permiten números.", log, linea);
                            }
                            else
                                v.valor = nuevoValor;
                        }
                    }
                    else if (Contenido == "Read")
                    {
                        match("Read");
                        match("(");
                        match(")");
                        if (Condicion)
                        {
                            nuevoValor = Console.Read();
                        }

                    }
                }
                else
                {
                    Expresion();
                    if (Condicion)
                    {
                        nuevoValor = S.Pop();
                    }
                }
            }
            if (AnalisisSemantico(v, nuevoValor))
            {
                if (Condicion)
                {
                    v.valor = nuevoValor;
                    S.Push(nuevoValor);
                }
            }
            else
            {
                
                throw new Error("Semantico, no puedo asignar un " + tipoDatoExpresion +
                                " a un " + v.tipo, log, linea);
            }
           // log.WriteLine(variable + " = " + nuevoValor);

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
            //Console.WriteLine(tipoDatoExpresion); 
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
                return true;
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
        private void If(bool evaluacion)
        {
            match("if");
            match("(");
            bool resultado = Condicion();
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(resultado && evaluacion);
            }
            else
            {
                Instruccion(resultado && evaluacion);
            }
            if (Contenido == "else")
            {
                match("else");
                if (Contenido == "{")
                {
                    BloqueInstrucciones(!resultado && evaluacion);
                }
                else
                {
                    Instruccion(!resultado && evaluacion);
                }
            }
        }
        //Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion()
        {
            Expresion();
            String operador = Contenido;
            match(Tipos.OpRelacional);
            Expresion();
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
        private void While(bool evaluacion)
        {
            int cTemp = caracter - 6;
            int lTemp = linea;
            bool resultado = false;
            do
            {

                match("while");
                match("(");
                resultado = Condicion() && evaluacion;
                match(")");
                if (resultado)
                {
                    if (Contenido == "{")
                    {
                        BloqueInstrucciones(evaluacion);
                    }
                    else
                    {
                        Instruccion(evaluacion);
                    }
                    caracter = cTemp;
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(cTemp, SeekOrigin.Begin);
                    nextToken();
                }
            } while (resultado);
        }
        //Do -> do 
        //        bloqueInstrucciones | intruccion 
        //      while(Condicion);
        private void Do(bool evaluacion)
        {
            int cTemp = caracter - 3;
            int lTemp = linea;
            bool resultado = false;
            do
            {
                match("do");
                if (Contenido == "{")
                {
                    BloqueInstrucciones(evaluacion);
                }
                else
                {
                    Instruccion(evaluacion);
                }
                match("while");
                match("(");
                resultado = Condicion() && evaluacion;
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
        private void For(bool evaluacion)
        {
            int cTemp = caracter - 4;
            int lTemp = linea;
            bool resultado = false;
            do
            {
                match("for");
                match("(");
                Asignacion(evaluacion);
                match(";");
                resultado = Condicion() && evaluacion;
                match(";");
                Incremento();
                match(")");
                if (resultado)
                {
                    if (Contenido == "{")
                    {
                        BloqueInstrucciones(evaluacion);
                    }
                    else
                    {
                        Instruccion(evaluacion);
                    }
                    caracter = cTemp;
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(cTemp, SeekOrigin.Begin);
                    nextToken();
                }
            } while (resultado);

        }
        //Incremento -> Identificador ++ | --
        private void Incremento()
        {
            match(Tipos.Identificador);
            if (Contenido == "++")
            {
                match("++");
            }
            else
            {
                match("--");
            }

        }
        //Console -> Console.(WriteLine|Write) (cadena); |
        //           Console.(Read | ReadLine) ();
        private void console(bool evaluacion)
        {
            char quitar = '"';
            String cadena = "";
            bool condicion = true;
            match("Console");
            match(".");
            if (Contenido == "WriteLine")
            {
                match("WriteLine");
                condicion = true;
            }
            else
            {
                match("Write");
            }
            match("(");
            if (Clasificacion == Tipos.Cadena)
            {
                if (evaluacion)
                {
                    cadena = Contenido;
                    cadena = cadena.Replace(quitar.ToString(), "");
                    match(Tipos.Cadena);
                    if (condicion)
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
            else
            {
                if (evaluacion)
                {
                    float resultado;
                    var v = listaVariables.Find(variable => variable.nombre == Contenido);
                    resultado = v.valor;
                    match(Tipos.Identificador);
                    if (condicion)
                    {
                        if (Contenido == "+")
                        {
                            String temp = listaConcatenacion();
                            Console.WriteLine(resultado + temp);
                        }
                        else
                        {
                            Console.WriteLine(resultado);
                        }
                    }
                    else
                    {
                        if (Contenido == "+")
                        {
                            String temp = listaConcatenacion();
                            Console.Write(resultado + temp);
                        }
                        else
                        {
                            Console.Write(resultado);
                        }
                    }
                }

            }
            match(")");
            match(";");
        }
        private string listaConcatenacion()
        {
            char quitar = '"';
            String cadena = "";
            string resultado = "";
            match("+");
            if (Clasificacion == Tipos.Identificador)
            {
                if (!ExisteVariable(Contenido))
                {
                    throw new Error("Semantico: la variable no existe: " + Contenido, log, linea);
                }
                var v = listaVariables.Find(variable => variable.nombre == Contenido);
                resultado = v.valor.ToString();
                match(Tipos.Identificador);
            }
            if (Clasificacion == Tipos.Cadena)
            {
                cadena = Contenido;
                cadena = cadena.Replace(quitar.ToString(), "");
                resultado += cadena;
                match(Tipos.Cadena);
            }
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
        private void Expresion()
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
                    case "+":
                        tipoDatoExpresion = valorToTipo(R1 + R2);
                        S.Push(R1 + R2); break;
                    case "-":
                        tipoDatoExpresion = valorToTipo(R1 - R2);
                        S.Push(R1 - R2); break;
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
                    case "*":
                        tipoDatoExpresion = valorToTipo(R1 * R2);
                        S.Push(R1 * R2); break;
                    case "/":
                        tipoDatoExpresion = valorToTipo(R1 / R2);
                        S.Push(R1 / R2); break;
                    case "%":
                        tipoDatoExpresion = valorToTipo(R1 % R2);
                        S.Push(R1 % R2); break;
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