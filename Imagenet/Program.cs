﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imagenet
{
    class Program
    {

        static void Main(string[] args)
        {
            Manager m = new Manager();
            //m.BatchLoadWords();
            //m.AddIsAvailable();
            m.AddImgs();
        }
       
    }
}
