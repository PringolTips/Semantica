using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/*
    1.  Asignacion  -> Identificador = Expresion;
                    -> Identificador (++ | --); INC o DEC                               -listo
                    -> Identificador (+= | -= ) Expresion; PUSH o POP                   -listo
                    -> Identificador (*= | /= | %= ) Expresion; PUSH o POP              -listo
    2.  Validar que las variables fueron declaradas                                     -listo
    3.  Evaluar variables en la expresion matematica                                    -listo
    4.  Emular el Write, WriteLine, Read y ReadLine en el metodo console                -listo
    5.  En Asignacion incluir el ReadLine y Read                                        -listo
    6.  Agregar Evaluacion en el resto de los metodos (true)                            -listo
*/

namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> variables;
        Stack<float> s;
        public Lenguaje()
        {
            variables = new List<Variable>();
            s = new Stack<float>();


        }
        public Lenguaje(string name) : base(name)
        {
            variables = new List<Variable>();
            s = new Stack<float>();

        }
        private void displayVariables()
        {
            log.WriteLine("Variables");
            log.WriteLine("---------");
            asm.WriteLine(";Variables");
            asm.WriteLine(";---------");
            foreach (Variable v in variables)
            {
                log.WriteLine(v.getNombre() + " " + v.getTipo() + " " + v.getValor());
                asm.WriteLine(v.getNombre() + " dw 0");
            }
        }

        private bool ExisteVariable(string nombre)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre() == nombre)
                {
                    return true;
                }
            }
            return false;
        }

        public float Salida_variable(string nombre)
        {

            foreach (Variable v in variables)
            {
                if (v.getNombre() == nombre)
                {
                    return v.getValor();
                }
            }
            return 0;

        }


        private void modificaVariable(string nombre, float valor)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre() == nombre)
                {
                    v.setValor(valor);
                }
            }
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            if (getContenido() == "using")
            {
                Librerias();
            }
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            asm.WriteLine("include 'emu8086.inc'");
            asm.WriteLine("org 100h");
            Main();
            displayVariables();
            asm.WriteLine("ret");
            asm.WriteLine("define_scan_num");
            asm.WriteLine("define_print_num_uns ");
            asm.WriteLine("define_print_num");
            asm.WriteLine("end");

        }

        //Librerias -> using ListaLibrerias; Librerias?
        private void Librerias()
        {
            match("using");
            ListaLibrerias();
            match(";");
            if (getContenido() == "using")//Caso base de la recursividad
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
        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            Variable.TipoD tipoDato = Variable.TipoD.Char;
            switch (getContenido())
            {
                case "int": tipoDato = Variable.TipoD.Int; break;
                case "float": tipoDato = Variable.TipoD.Float; break;
            }
            match(Tipos.TipoDato);
            ListaIdentificadores(tipoDato);
            match(";");
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
        }

        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoD tipoDato)
        {
            if (!ExisteVariable(getContenido()))
            {
                variables.Add(new Variable(getContenido(), tipoDato));
            }
            else
            {
                throw new Error("Sintaxis: variable diplicada " + getContenido(), log);
            }
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                ListaIdentificadores(tipoDato);
            }

        }
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones(bool evaluacion)
        {
            match("{");
            if (getClasificacion() != Tipos.Fin)
            {
                ListaInstrucciones(evaluacion);
            }

            match("}");

        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion)
        {
            Instruccion(evaluacion);

            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }


        }

        //Instruccion -> Console | If | While | do | For | Asignacion
        private void Instruccion(bool evaluacion)
        {
            if (getContenido() == "Console")
            {
                console(evaluacion);
            }
            else if (getContenido() == "if")
            {
                If(evaluacion);
            }
            else if (getContenido() == "while")
            {
                While(evaluacion);
            }
            else if (getContenido() == "do")
            {
                Do(evaluacion);
            }
            else if (getContenido() == "for")
            {
                For(evaluacion);
            }
            else if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion(evaluacion);
            }

        }
        //Identificador = Expresion;
        //            -> Identificador (++ | --); INC o DEC
        //            -> Identificador (+= | -= ) Expresion; PUSH o POP
        //            -> Identificador (*= | /= | %= ) Expresion; PUSH o POP
        private void Asignacion(bool Condicion)
        {

            String nombre = getContenido();
            if (ExisteVariable(getContenido()))
            {
                match(Tipos.Identificador);
                if (getContenido() == "++")
                {
                    match("++");
                    match(";");
                    if (Condicion)
                    {
                        float temp = Salida_variable(nombre) + 1;
                        modificaVariable(nombre, temp);
                        asm.WriteLine("INC " + nombre);
                    }

                }
                else if (getContenido() == "--")
                {

                    match("--");
                    match(";");
                    if (Condicion)
                    {
                        float temp = Salida_variable(nombre) - 1;
                        modificaVariable(nombre, temp);
                        asm.WriteLine("DEC " + nombre);
                    }
                }
                else if (getContenido() == "+=")
                {
                    match("+=");
                    if (getClasificacion() == Tipos.Identificador)
                    {


                        if (Condicion)
                        {
                            String nombre2 = getContenido();
                            modificaVariable(nombre, Salida_variable(nombre) + Salida_variable(nombre2));
                            asm.WriteLine("MOV  AX, " + Salida_variable(nombre2));
                            asm.WriteLine("ADD " + nombre + " , AX");
                        }
                        match(Tipos.Identificador);



                    }
                    else if (getClasificacion() == Tipos.Numero)
                    {


                        if (Condicion)
                        {
                            float temp = float.Parse(getContenido());
                            modificaVariable(nombre, Salida_variable(nombre) + temp);
                            asm.WriteLine("ADD " + nombre + " , " + temp);

                        }
                        match(Tipos.Numero);



                    }
                    match(";");

                }
                else if (getContenido() == "-=")
                {
                    match("-=");
                    if (getClasificacion() == Tipos.Identificador)
                    {


                        if (Condicion)
                        {
                            String nombre2 = getContenido();
                            modificaVariable(nombre, Salida_variable(nombre) - Salida_variable(nombre2));
                            asm.WriteLine("MOV  AX, " + Salida_variable(nombre2));
                            asm.WriteLine("SUB " + nombre + " , AX");

                        }
                        match(Tipos.Identificador);

                    }
                    else if (getClasificacion() == Tipos.Numero)
                    {
                        if (Condicion)
                        {
                            float temp = float.Parse(getContenido());
                            modificaVariable(nombre, Salida_variable(nombre) - temp);
                            asm.WriteLine("SUB " + nombre + " , " + temp);

                        }

                        match(Tipos.Numero);



                    }
                    match(";");

                }
                else if (getContenido() == "*=")
                {
                    match("*=");
                    if (getClasificacion() == Tipos.Identificador)
                    {


                        if (Condicion)
                        {
                            String nombre2 = getContenido();
                            asm.WriteLine("MOV  AX, " + Salida_variable(nombre));
                            asm.WriteLine("MUL " + nombre2);
                            asm.WriteLine("MOV " + nombre + " , AX");
                            modificaVariable(nombre, Salida_variable(nombre) * Salida_variable(nombre2));
                        }
                        match(Tipos.Identificador);



                    }
                    else if (getClasificacion() == Tipos.Numero)
                    {


                        if (Condicion)
                        {
                            float temp = float.Parse(getContenido());
                            asm.WriteLine("MOV  AX, " + temp);
                            asm.WriteLine("MUL " + nombre);
                            asm.WriteLine("MOV " + nombre + " , AX");
                            modificaVariable(nombre, Salida_variable(nombre) * temp);
                        }
                        match(Tipos.Numero);



                    }
                    match(";");
                }
                else if (getContenido() == "/=")
                {
                    match("/=");
                    if (getClasificacion() == Tipos.Identificador)
                    {


                        if (Condicion)
                        {
                            String nombre2 = getContenido();
                            asm.WriteLine("MOV  BX, " + Salida_variable(nombre2));
                            asm.WriteLine("MOV  AX, " + Salida_variable(nombre));
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("MOV " + nombre + " , AX");
                            modificaVariable(nombre, Salida_variable(nombre) / Salida_variable(nombre2));
                        }
                        match(Tipos.Identificador);


                    }
                    else if (getClasificacion() == Tipos.Numero)
                    {


                        if (Condicion)
                        {
                            float temp = float.Parse(getContenido());
                            asm.WriteLine("MOV  BX, " + temp);
                            asm.WriteLine("MOV  AX, " + Salida_variable(nombre));
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("MOV " + nombre + " , AX");
                            modificaVariable(nombre, Salida_variable(nombre) / temp);
                            match(Tipos.Numero);
                        }

                    }

                    match(";");
                }
                else if (getContenido() == "%=")
                {
                    match("%=");
                    if (getClasificacion() == Tipos.Identificador)
                    {
                        if (Condicion)
                        {
                            String nombre2 = getContenido();
                            asm.WriteLine("MOV  BX, " + Salida_variable(nombre2));
                            asm.WriteLine("MOV  AX, " + Salida_variable(nombre));
                            asm.WriteLine("ADD BX");
                            asm.WriteLine("MOV " + nombre + " , DX");
                            modificaVariable(nombre, Salida_variable(nombre) % Salida_variable(nombre2));

                        }

                        match(Tipos.Identificador);
                    }
                    else if (getClasificacion() == Tipos.Numero)
                    {
                        if (Condicion)
                        {
                            float temp = float.Parse(getContenido());

                            asm.WriteLine("MOV  BX, " + temp);
                            asm.WriteLine("MOV  AX, " + Salida_variable(nombre));
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("ADD " + nombre + " , DX");
                            modificaVariable(nombre, Salida_variable(nombre) % temp);
                        }
                        match(Tipos.Numero);
                    }
                    match(";");

                }
                else if (getContenido() == "=")
                {
                    match("=");
                    if (getContenido() == "Console")
                    {
                        match("Console");
                        match(".");
                        if (getContenido() == "ReadLine")
                        {
                            match("ReadLine");
                            match("(");
                            match(")");
                            match(";");
                            if (Condicion)
                            {
                                String? temp = Console.ReadLine();
                                if (temp != null)
                                {
                                    modificaVariable(nombre, float.Parse(temp));
                                    asm.WriteLine("printn ' ' ");
                                    asm.WriteLine("call scan_num");
                                    asm.WriteLine("MOV " + nombre + " , CX");
                                }
                                else
                                {
                                    modificaVariable(nombre, 0);
                                    asm.WriteLine("printn ' ' ");
                                    asm.WriteLine("call scan_num");
                                    asm.WriteLine("MOV " + nombre + " , 0");
                                }

                            }

                        }
                        else if (getContenido() == "Read")
                        {
                            match("Read");
                            match("(");
                            match(")");
                            match(";");
                            if (Condicion)
                            {
                                char? temp = (char)Console.Read();
                                if (temp != null)
                                {
                                    modificaVariable(nombre, (float)temp);
                                    asm.WriteLine("printn ' ' ");
                                    asm.WriteLine("call scan_num");
                                    asm.WriteLine("MOV " + nombre + " , CX");
                                }
                                else
                                {
                                    modificaVariable(nombre, 0);
                                    asm.WriteLine("printn ' ' ");
                                    asm.WriteLine("call scan_num");
                                    asm.WriteLine("MOV " + nombre + " , 0");
                                }

                            }

                        }


                    }
                    else
                    {
                        Expresion();
                        match(";");
                        if (Condicion)
                        {
                            modificaVariable(nombre, s.Pop());
                            asm.WriteLine("POP AX");
                            asm.WriteLine("MOV " + nombre + " , AX");

                        }


                    }


                }

            }
            else
            {
                throw new Error("Sintaxis: variable no declarada " + getContenido(), log);
            }



        }
        //If -> if (Condicion) bloqueInstrucciones | instruccion
        //     (else bloqueInstrucciones | instruccion)?
        private void If(bool evaluacion)
        {
            match("if");
            match("(");
            bool EvaluaIF = Condicion() && evaluacion;
            //Console.WriteLine(EvaluaIF + " " + !EvaluaIF);

            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(EvaluaIF);
            }
            else
            {
                Instruccion(EvaluaIF);
            }
            if (getContenido() == "else")
            {
                match("else");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(!EvaluaIF);
                }
                else
                {
                    Instruccion(!EvaluaIF);
                }
            }

        }

        //Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion()
        {
            Expresion();
            String operador = getContenido();
            match(Tipos.OpRelacional);
            Expresion();
            float R2 = s.Pop();
            float R1 = s.Pop();
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
            match("while");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(evaluacion);
            }
            else
            {
                Instruccion(evaluacion);
            }


        }
        //Do -> do 
        //        bloqueInstrucciones | intruccion 
        //      while(Condicion);
        private void Do(bool evaluacion)
        {
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(evaluacion);
            }
            else
            {
                Instruccion(evaluacion);
            }
            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");

        }
        //For -> for(Asignacion Condicion; Incremento) 
        //       BloqueInstrucciones | Intruccion 
        private void For(bool evaluacion)
        {
            match("for");
            match("(");
            Asignacion(evaluacion);
            Condicion();
            match(";");
            Incremento();
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(evaluacion);
            }
            else
            {
                Instruccion(evaluacion);
            }

        }
        //Incremento -> Identificador ++ | --
        private void Incremento()
        {
            match(Tipos.Identificador);
            if (getContenido() == "++")
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
            match("Console");
            match(".");
            if (getContenido() == "WriteLine")
            {
                match("WriteLine");
                match("(");
                if (evaluacion)
                {
                    String cadena = getContenido().TrimStart('"');
                    cadena = cadena.TrimEnd('"');
                    Console.WriteLine(cadena);
                    asm.WriteLine("printn " + '"' + cadena + '"');
                }

                match(Tipos.Cadena);
                match(")");
                match(";");
            }
            else if (getContenido() == "Write")
            {
                match("Write");
                match("(");
                if (evaluacion)
                {
                    String cadena = getContenido().TrimStart('"');
                    cadena = cadena.TrimEnd('"');
                    Console.Write(cadena);
                    asm.WriteLine("print " + '"' + cadena + '"');
                }

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
                Console.Read();
            }
            else if (getContenido() == "ReadLine")
            {
                match("ReadLine");
                match("(");
                match(")");
                match(";");
                Console.ReadLine();
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
            if (getClasificacion() == Tipos.OpTermino)
            {
                string operador = getContenido();
                match(Tipos.OpTermino);
                Termino();
                float R2 = s.Pop();
                asm.WriteLine("POP BX");
                float R1 = s.Pop();
                asm.WriteLine("POP AX");
                switch (operador)
                {
                    case "+":
                        asm.WriteLine("ADD AX, BX");
                        asm.WriteLine("PUSH AX");
                        s.Push(R1 + R2); break;
                    case "-":
                        asm.WriteLine("SUB AX, BX");
                        asm.WriteLine("PUSH AX");
                        s.Push(R1 - R2); break;
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
            if (getClasificacion() == Tipos.OpFcator)
            {
                string operador = getContenido();
                match(Tipos.OpFcator);
                Factor();
                float R2 = s.Pop();
                asm.WriteLine("POP BX");
                float R1 = s.Pop();
                asm.WriteLine("POP AX");
                switch (operador)
                {
                    case "*":
                        s.Push(R1 * R2);
                        asm.WriteLine("MUL BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "/":
                        s.Push(R1 / R2);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH AX"); break;
                    case "%":
                        s.Push(R1 % R2);
                        asm.WriteLine("ADD BX");
                        asm.WriteLine("PUSH DX");
                        break;
                }
            }

        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                asm.WriteLine("MOV AX , " + getContenido());
                asm.WriteLine("PUSH AX");
                //Console.Write(getContenido() + " ");
                s.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                s.Push(Salida_variable(getContenido()));
                asm.WriteLine("MOV AX , " + getContenido());
                asm.WriteLine("PUSH AX");
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                Expresion();
                match(")");
            }


        }
    }
}