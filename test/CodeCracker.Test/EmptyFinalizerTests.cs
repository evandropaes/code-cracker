﻿using Microsoft.CodeAnalysis;
using TestHelper;
using Xunit;

namespace CodeCracker.Test
{
    public class EmptyFinalizerTests : CodeFixTest<EmptyFinalizerAnalyzer, EmptyFinalizerCodeFixProvider>
    {
        [Fact]
        public void RemoveEmptyFinalizerWhenIsEmpty()
        {
            var test = @"
                public class MyClass 
                { 
                    ~MyClass() 
                    { 

                    } 
                }";

            var expected = new DiagnosticResult
            {
                Id = EmptyFinalizerAnalyzer.DiagnosticId,
                Message = "Remove Empty Finalizers",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 4, 21) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void RemoveEmptyFinalizerWithSingleLineComment()
        {
            var test = @"
                public class MyClass 
                { 
                    ~MyClass() 
                    { 
                        //comments...
                    }
                }";

            var expected = new DiagnosticResult
            {
                Id = EmptyFinalizerAnalyzer.DiagnosticId,
                Message = "Remove Empty Finalizers",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 4, 21) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void RemoveEmptyFinalizerWithMultiLineComment()
        {
            var test = @"
                public class MyClass 
                { 
                    ~MyClass() 
                    { 
                        /*
                            multiline
                            comments
                        */
                    }
                }";

            var expected = new DiagnosticResult
            {
                Id = EmptyFinalizerAnalyzer.DiagnosticId,
                Message = "Remove Empty Finalizers",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 4, 21) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void MaintainFinalizerWhenUsed()
        {
            var test = @"
                public class MyClass
                {
                    private System.IntPtr pointer;

                    ~MyClass() 
                    { 
                        pointer = System.IntPtr.Zero; 
                    }
                }";

            VerifyCSharpHasNoDiagnostics(test);
        }

        [Fact]
        public void WhenFinalizerIsRemovedFromClass()
        {
            var source = @"
                public class MyClass 
                { 
                    ~MyClass() 
                    { 

                    } 
                }";

            var fixtest = @"
                public class MyClass 
                { 
                }";

            VerifyCSharpFix(source, fixtest, 0);
        }
    }
}