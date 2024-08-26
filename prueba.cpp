using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

static void Main(string[] args)
{
    char  e;
    float pi;

    Console.Write("Proyecto 6");
    Console.WriteLine(" - ITQ");
    e = Console.ReadLine();    
    pi = (3+5)*8-(10-e)/2; //61;
    pi++;   //62
    e--;    //3
    pi+=e;  //65
    pi-=5;  //60
    e-=3;   //0
    pi*=10; //600
    e+=2;   //2
    pi/=2;  //300

    int a;
    if (1==1)
    {
        if (2==2)
            Console.WriteLine("EntrÃ³ al IF");
        a=100;
    }
    else
    {
        a=200;
        Console.WriteLine("EntrÃ³ al ELSE");
    }
}