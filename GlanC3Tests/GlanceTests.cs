using Microsoft.VisualStudio.TestTools.UnitTesting;
using GC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GC.Tests
{
    [TestClass()]
    public class GlanceTests
    {
        [TestMethod()]
        public void GenerateCodeTest()
        {
            try
            {
                GC.Glance.GenerateCode("D:/GC/out");
            }
            catch(Exception e)
            {
                Console.WriteLine("Assertion failed, ", e.ToString());
            }
        }
    }
    public class M
    {
        public void Main()
        {
            (new GlanceTests()).GenerateCodeTest();
        }
    }
}