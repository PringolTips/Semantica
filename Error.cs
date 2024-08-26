using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semantica
{
    public class Error : Exception
    {
        
        public Error(string message, StreamWriter log) : base("Error "+message){
            log.WriteLine(message); 
           // log.WriteLine(message);
        }
        
    }
}