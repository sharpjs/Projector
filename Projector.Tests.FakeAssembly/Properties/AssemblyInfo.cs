using System.Reflection;
using System.Security;

// Description
[assembly: AssemblyTitle       ("Projector.Tests.FakeAssembly")]
[assembly: AssemblyDescription ("Projector Test Suite Fake Assembly")]

// Security
//
// All code is transparent; the entire assembly will not do anything privileged or unsafe.
// http://msdn.microsoft.com/en-us/library/dd233102.aspx
//
[assembly: SecurityTransparent]
