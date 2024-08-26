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
        TipoD tipo;
        string nombre;
        float valor;
        TipoZ zona;

        public Variable(string nombre, TipoD tipo, TipoZ zona = TipoZ.Private)
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.zona = zona;
            this.valor = 0;
        }
        public string getNombre()
        {
            return nombre;
        }
        public TipoD getTipo()
        {
            return tipo;
        }
        public float getValor()
        {
            return valor;
        }
        public void setValor(float valor)
        {
            this.valor = valor;
        }

    }
}