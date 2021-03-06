﻿using Microsoft.CodeAnalysis;
using TestHelper;
using Xunit;

namespace CodeCracker.Test
{
    public class SealedAttributeTests : CodeFixTest<SealedAttributeAnalyzer, SealedAttributeCodeFixProvider>
    {
        [Fact]
        public void ApplySealedWhenClassInheritsFromSystemAttributeClass()
        {
            var test = @"
                public class MyAttribute : System.Attribute 
                { 

                }";

            var expected = new DiagnosticResult
            {
                Id = SealedAttributeAnalyzer.DiagnosticId,
                Message = "Mark 'MyAttribute' as sealed.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 2, 30) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void ApplySealedWhenClassInheritsIndirectlyFromSystemAttributeClass()
        {
            var test = @"
                public abstract class MyAttribute : System.Attribute 
                { 

                }

                public class OtherAttribute : MyAttribute 
                { 

                }";

            var expected = new DiagnosticResult
            {
                Id = SealedAttributeAnalyzer.DiagnosticId,
                Message = "Mark 'OtherAttribute' as sealed.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 7, 30) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void NotApplySealedWhenClassThatInheritsFromSystemAttributeClassIsAbstract()
        {
            var test = @"
                public abstract class MyAttribute : System.Attribute 
                { 

                }";

            VerifyCSharpHasNoDiagnostics(test);
        }

        [Fact]
        public void NotApplySealedWhenClassThatInheritsFromSystemAttributeClassIsSealed()
        {
            var test = @"
                public sealed class MyAttribute : System.Attribute 
                { 

                }";

            VerifyCSharpHasNoDiagnostics(test);
        }

        [Fact]
        public void NotApplySealedWhenIsStruct()
        {
            var test = @"
                public struct MyStruct 
                { 

                }";

            VerifyCSharpHasNoDiagnostics(test);
        }

        [Fact]
        public void NotApplySealedWhenIsInterface()
        {
            var test = @"
                public interface ITest 
                { 

                    }";

            VerifyCSharpHasNoDiagnostics(test);
        }

        [Fact]
        public void WhenSealedModifierIsAppliedOnClass()
        {
            var source = @"
                public class MyAttribute : System.Attribute 
                { 
                }";

            var fixtest = @"
                public sealed class MyAttribute : System.Attribute 
                { 
                }";

            VerifyCSharpFix(source, fixtest, 0);
        }
    }
}