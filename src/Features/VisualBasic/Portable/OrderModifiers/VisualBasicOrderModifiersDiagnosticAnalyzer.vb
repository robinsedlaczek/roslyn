﻿' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Collections.Generic
Imports Microsoft.CodeAnalysis.CodeStyle
Imports Microsoft.CodeAnalysis.VisualBasic.CodeStyle
Imports Microsoft.CodeAnalysis.VisualBasic.Extensions
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.Diagnostics
Imports Microsoft.CodeAnalysis.OrderModifiers
Imports Microsoft.CodeAnalysis.Text

Namespace Microsoft.CodeAnalysis.VisualBasic.OrderModifiers
    <DiagnosticAnalyzer(LanguageNames.VisualBasic)>
    Friend Class VisualBasicOrderModifiersDiagnosticAnalyzer
        Inherits AbstractOrderModifiersDiagnosticAnalyzer

        Public Sub New()
            MyBase.New(VisualBasicCodeStyleOptions.PreferredModifierOrder, VisualBasicOrderModifiersHelper.Instance)
        End Sub

        Protected Overrides Function GetModifiers(node As SyntaxNode) As SyntaxTokenList
            Return node.GetModifiers()
        End Function

        Protected Overrides Sub Recurse(
            context As SyntaxTreeAnalysisContext,
            preferredOrder As Dictionary(Of Integer, Integer),
            descriptor As DiagnosticDescriptor,
            root As SyntaxNode)

            For Each child In root.ChildNodesAndTokens()
                If child.IsNode Then
                    Dim declarationStatement = TryCast(child.AsNode(), DeclarationStatementSyntax)
                    If declarationStatement IsNot Nothing Then
                        If ShouldCheck(declarationStatement) Then
                            CheckModifiers(context, preferredOrder, descriptor, declarationStatement)
                        End If

                        Recurse(context, preferredOrder, descriptor, declarationStatement)
                    End If
                End If
            Next
        End Sub

        Private Function ShouldCheck(statement As DeclarationStatementSyntax) As Boolean
            Dim modifiers = statement.GetModifiers()
            If modifiers.Count >= 2 Then
                ' We'll see modifiers twice in some circumstances.  First, on a VB block
                ' construct, and then on the VB begin statement for that block.  In order
                ' to not double report, only check the statement that teh modifier actually
                ' belongs to.
                Return modifiers.First().Parent Is statement
            End If

            Return False
        End Function
    End Class
End Namespace
