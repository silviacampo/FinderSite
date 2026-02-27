/*
 * https://www.codeproject.com/Articles/1331918/Parallelization-in-Unit-Tests-with-MSTest-v2
 The variables Workers and Scope can be changed depending on your own customization:
Workers=0 (use as many threads as possible based on CPU and core count)
Workers=X (X is number of threads to execute tests)
Scope =ClassLevel (each thread executes a TestClass you have in your project)
Scope =MethodLevel (each thread executes a TestMethod)
 */
 /*
  Think how to test map and map2 in petcontroller
  Algo que abra la pagina y detecte que no nay error de consola, o que hay un mapa?
  */
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.ClassLevel)]
namespace MSTestUnitTest
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
		}
	}
}
