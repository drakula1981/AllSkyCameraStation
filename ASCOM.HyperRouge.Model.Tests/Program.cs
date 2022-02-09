using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.HyperRouge.Model.Tests {
   internal class Program {
      static void Main(string[] args) {
         Console.WriteLine($"{CurrentConditions.GetInstance("http://allskymini.local/datas/current.json")}");
         Console.ReadLine(); 
      }
   }
}
