using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

// General Information
[assembly: AssemblyProduct   ("Projector")]
[assembly: AssemblyCompany   ("Jeffrey Sharp")]
[assembly: AssemblyCopyright ("Copyright © 2013 Jeffrey Sharp")]
[assembly: AssemblyVersion   ("1.0.*")]

// Compliance
#if CLS_NONCOMPLIANT
    [assembly: CLSCompliant(false)]
#else
    [assembly: CLSCompliant(true)]
#endif
[assembly: ComVisible(false)]

// Security
[assembly: SecurityRules(SecurityRuleSet.Level2)]
[assembly: InternalsVisibleTo("Projector.Tests")]

// Configuration
#if DEBUG
    [assembly: AssemblyConfiguration("Debug")]
#else
    [assembly: AssemblyConfiguration("Release")]
#endif
