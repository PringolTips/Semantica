using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semantica
{
    public class Variable
    {
        public enum TipoD
        {
            Char, Int, Float
        };
        public enum TipoZ
        {
            Private, Protected, Public
        };
       
        public Variable(string nombre, TipoD tipo, TipoZ zona = TipoZ.Private)
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.zona = zona;
            this.valor = 0;
        }
        public string nombre { get; }
        public TipoD tipo { get; }
        public TipoZ zona { get; }
        public float valor { get; set; }

    }
}