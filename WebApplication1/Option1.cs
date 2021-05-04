using AutoConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    [AutoConfig("Option1")]
    public class Option1
    {
        public int Test1 { get; set; }
    }
}
