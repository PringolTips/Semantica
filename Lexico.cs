using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
namespace Semantica
{
    public class Lexico : Token, IDisposable
    {
        protected StreamReader archivo;
        public StreamWriter log;
        protected int linea, caracter;
        const int F = -1;
        const int E = -2;
        int[,] TRAND =
        {
             //WS, L, D, ., E, +, -, ;, =, *, /, %, &, |, !, <, >, ?, ", {, },EOF,Ld,\n, $
        /* 0*/{ 0, 1, 2,26, 1, 9,10, 8,18,12,27,12,14,15,19,17,17,21,22,24,25,26,26, 0,31},
        /* 1*/{ F, 1, 1, F, 1, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /* 2*/{ F, F, 2, 3, 5, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /* 3*/{ E, E, 4, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, F},
        /* 4*/{ F, F, 4, F, 5, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /* 5*/{ E, E, 7, E, E, 6, 6, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, F},
        /* 6*/{ E, E, 7, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, F},
        /* 7*/{ F, F, 7, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /* 8*/{ F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /* 9*/{ F, F, F, F, F,11, F, F,11, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /*10*/{ F, F, F, F, F, F,11, F,11, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /*11*/{ F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /*12*/{ F, F, F, F, F, F, F, F,13, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /*13*/{ F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /*14*/{ F, F, F, F, F, F, F, F, F, F, F, F,16, F, F, F, F, F, F, F, F, F, F, F, F},
        /*15*/{ F, F, F, F, F, F, F, F, F, F, F, F, F,16, F, F, F, F, F, F, F, F, F, F, F},
        /*16*/{ F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /*17*/{ F, F, F, F, F, F, F, F,20, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /*18*/{ F, F, F, F, F, F, F, F,20, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /*19*/{ F, F, F, F, F, F, F, F,20, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /*20*/{ F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /*21*/{ F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /*22*/{22,22,22,22,22,22,22,22,22,22,22,22,22,22,22,22,22,22,23,22,22, E,22,22, F},
        /*23*/{ F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /*24*/{ F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /*25*/{ F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /*26*/{ F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /*27*/{ F, F, F, F, F, F, F, F,13,29,28, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
        /*28*/{28,28,28,28,28,28,28,28,28,28,28,28,28,28,28,28,28,28,28,28,28, 0,28, 0, F},
        /*29*/{29,29,29,29,29,29,29,29,29,30,29,29,29,29,29,29,29,29,29,29,29, E,29,29, F},
        /*30*/{29,29,29,29,29,29,29,29,29,30, 0,29,29,29,29,29,29,29,29,29,29, E,29,29, F},
        /*31*/{ E, E,32, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E},
        /*32*/{ F, F,32, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
             //WS, L, D, ., E, +, -, ;, =, *, /, %, &, |, !, <, >, ?, ", {, },EOF,Ld,\n, $
        };
        public Lexico(string nombre = "prueba.cpp") // Constructor
        {
            linea = caracter = 1;
            log = new StreamWriter(Path.GetFileNameWithoutExtension(nombre) + ".log");
            log.AutoFlush = true;
            this.linea = 1;
            log.WriteLine("Analizador Lexico");
            log.WriteLine("Autores: \nVega Angeles Christopher");
            log.WriteLine("Moya Arreola Cristian");
            log.WriteLine("Martinez Prieto Angel Josue");
            log.WriteLine("Fecha:" + DateTime.Now);
            if (Path.GetExtension(nombre) != ".cpp")
            {
                throw new Error("El archivo " + nombre + " no tiene extension CPP", log);
            }
            if (!File.Exists(nombre))
            {
                throw new Error("El archivo " + nombre + " no existe", log);
            }
            archivo = new StreamReader(nombre);
        }
        public void Dispose() // Destructor
        {
            archivo.Close();
            log.Close();
        }
        int Columna(char c)
        {
            if (finArchivo())
            {
                return 21;
            }
            else if (c == '\n')
            {
                linea++;
                return 23;
            }
            else if (char.IsWhiteSpace(c))
            {
                return 0;
            }
            else if (char.ToUpper(c) == 'E')
            {
                return 4;
            }
            else if (char.IsLetter(c))
            {
                return 1;
            }
            else if (char.IsDigit(c))
            {
                return 2;
            }
            else if (c == '.')
            {
                return 3;
            }
            else if (c == '+')
            {
                return 5;
            }
            else if (c == '-')
            {
                return 6;
            }
            else if (c == ';')
            {
                return 7;
            }
            else if (c == '=')
            {
                return 8;
            }
            else if (c == '*')
            {
                return 9;
            }
            else if (c == '/')
            {
                return 10;
            }
            else if (c == '%')
            {
                return 11;
            }
            else if (c == '&')
            {
                return 12;
            }
            else if (c == '|')
            {
                return 13;
            }
            else if (c == '!')
            {
                return 14;
            }
            else if (c == '>')
            {
                return 15;
            }
            else if (c == '<')
            {
                return 16;
            }
            else if (c == '?')
            {
                return 17;
            }
            else if (c == '"')
            {
                return 18;
            }
            else if (c == '{')
            {
                return 19;
            }
            else if (c == '}')
            {
                return 20;
            }
            else if (c == '$')
            {
                return 24;
            }
            else
            {
                return 22;
            }
        }
        private void Clasificar(int Estado)
        {
            switch (Estado)
            {
                case 01: Clasificacion = Tipos.Identificador; break;
                case 02: Clasificacion = Tipos.Numero; break;
                case 08: Clasificacion = Tipos.FinSentencia; break;
                case 09:
                case 10: Clasificacion = Tipos.OpTermino; break;
                case 11: Clasificacion = Tipos.IncTermino; break;
                case 12: Clasificacion = Tipos.OpFactor; break;
                case 13: Clasificacion = Tipos.IncFactor; break;
                case 14: Clasificacion = Tipos.Caracter; break;
                case 15: Clasificacion = Tipos.Caracter; break;
                case 16:
                case 19: Clasificacion = Tipos.OpLogico; break;
                case 17:
                case 20: Clasificacion = Tipos.OpRelacional; break;
                case 21: Clasificacion = Tipos.OpTernario; break;
                case 18: Clasificacion = Tipos.Asignacion; break;
                case 22: Clasificacion = Tipos.Cadena; break;
                case 24: Clasificacion = Tipos.Inicio; break;
                case 25: Clasificacion = Tipos.Fin; break;
                case 26: Clasificacion = Tipos.Caracter; break;
                case 27: Clasificacion = Tipos.OpFactor; break;

            }
        }
        public void nextToken()
        {
            char c;
            string buffer = "";
            int Estado = 0;

            while (Estado >= 0)
            {
                c = (char)archivo.Peek();
                Estado = TRAND[Estado, Columna(c)];

                Clasificar(Estado);
                if (Estado >= 0)
                {
                    if (Estado == 0)
                    {
                        buffer = "";
                    }
                    else if (Estado > 0)
                    {
                        buffer += c;
                    }
                    caracter++;
                    archivo.Read();
                }
            }
            if (Estado == E)
            {
                if (Clasificacion == Tipos.Numero)
                {
                    throw new Error(" lexico: se espera un digito en la linea: " + linea + "   " + buffer, log);
                }
                else if (Clasificacion == Tipos.Cadena)
                {
                    throw new Error(" lexico: se espera cierre de cadena en la linea:" + linea + "   " + buffer, log);
                }
                else if (Clasificacion == Tipos.OpFactor)
                {
                    throw new Error(" lexico: se espera cierre de Comentario en la linea: " + linea + "   " + buffer, log);
                }
            }


            if (Clasificacion == Tipos.Identificador)
            {
                if (buffer == "int" || buffer == "char" || buffer == "float" || buffer == "double")
                {
                    Clasificacion = Tipos.TipoDato;
                }
                else if (buffer == "if" || buffer == "else" || buffer == "switch")
                {
                    Clasificacion = Tipos.Condicion;
                }
                else if (buffer == "while" || buffer == "for" || buffer == "do")
                {
                    Clasificacion = Tipos.Ciclo;
                }

            }
            Contenido = buffer;
            //log.WriteLine(Contenido + " = " + Clasificacion);
        }
        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }
    }
}