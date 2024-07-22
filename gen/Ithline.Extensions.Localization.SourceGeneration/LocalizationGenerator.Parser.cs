using System.Collections.Immutable;
using System.Diagnostics;
using Ithline.Extensions.Localization.SourceGeneration.Specs;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ithline.Extensions.Localization.SourceGeneration;

public sealed partial class LocalizationGenerator
{
    private sealed class Parser
    {
        private readonly Dictionary<TypeRef, TypeSpecBuilder> _typeBuilders = [];
        private readonly KnownTypeSymbols _typeSymbols;
        private readonly bool _langVersionIsSupported;

        public Parser(KnownTypeSymbols typeSymbols, bool langVersionIsSupported)
        {
            _typeSymbols = typeSymbols;
            _langVersionIsSupported = langVersionIsSupported;
        }

        public List<DiagnosticInfo>? Diagnostics { get; private set; }

        public SourceGenerationSpec? GetGeneratorSpec(ImmutableArray<MethodCandidate?> candidates, CancellationToken cancellationToken)
        {
            if (!_langVersionIsSupported)
            {
                this.RecordDiagnostic(Descriptors.LanguageVersionIsNotSupported, Location.None);
                return null;
            }

            if (_typeSymbols.IStringLocalizer is null || _typeSymbols.LocalizedString is null)
            {
                return null;
            }

            if (candidates.IsDefaultOrEmpty)
            {
                return null;
            }

            foreach (var candidate in candidates)
            {
                if (candidate is null)
                {
                    continue;
                }

                this.ParseMethod(candidate, cancellationToken);
            }

            var typeSpecs = BuildSpecsOf(null);
            if (typeSpecs is { Count: > 0 } ts)
            {
                return new SourceGenerationSpec
                {
                    Types = ts,
                };
            }

            return null;

            EquatableArray<TypeSpec>? BuildSpecsOf(TypeRef? parentRef)
            {
                List<TypeSpec>? list = null;
                foreach (var builder in _typeBuilders.Values)
                {
                    if (builder.ParentRef != parentRef)
                    {
                        continue;
                    }

                    var types = BuildSpecsOf(builder.TypeRef);
                    var methods = builder.GetMethods();

                    // record only types with members
                    if (types is { Count: > 0 } || methods is { Count: > 0 })
                    {
                        list ??= [];
                        list.Add(new TypeSpec
                        {
                            TypeName = builder.TypeName,
                            Namespace = builder.Namespace,
                            Keyword = builder.Keyword,
                            Types = types,
                            Methods = methods,
                            LocalizerField = builder.LocalizerField,
                        });
                    }
                }

                return list is null ? null : [.. list];
            }
        }

        private void ParseMethod(MethodCandidate candidate, CancellationToken cancellationToken)
        {
            var methodSyntax = candidate.MethodSyntax;
            var methodSymbol = candidate.MethodSymbol;

            var methodName = methodSymbol.Name;

            // method cannot start with _ since that can lead to conflicting symbol names, because the generated symbols start with _
            if (methodName[0] is '_')
            {
                this.RecordDiagnostic(Descriptors.NameStartsWithUnderscore, methodSyntax.Identifier.GetLocation());
                return;
            }

            // do not support generic methods
            if (methodSyntax.Arity > 0)
            {
                this.RecordDiagnostic(Descriptors.MethodIsGeneric, methodSyntax.Identifier.GetLocation());
                return;
            }

            // method must return LocalizedString
            if (!methodSymbol.ReturnType.Equals(_typeSymbols.LocalizedString, SymbolEqualityComparer.Default))
            {
                this.RecordDiagnostic(Descriptors.MustReturnLocalizedString, methodSyntax.ReturnType.GetLocation());
                return;
            }

            // method cannot have body
            var methodBody = methodSyntax.Body as CSharpSyntaxNode ?? methodSyntax.ExpressionBody;
            if (methodBody is not null)
            {
                this.RecordDiagnostic(Descriptors.MethodHasBody, methodBody.GetLocation());
                return;
            }

            ExtractMethodModifiers(methodSyntax, out var isStatic, out var isPartial);

            // method must be partial
            if (!isPartial)
            {
                this.RecordDiagnostic(Descriptors.MustBePartial, methodSyntax.GetLocation());
                return;
            }

            var foundLocalizer = false;
            var parameterSpecs = new List<MethodParameterSpec>();
            foreach (var parameter in methodSymbol.Parameters)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // semantic problem, just bail
                if (string.IsNullOrWhiteSpace(parameter.Name))
                {
                    return;
                }

                // if parameter is error type, just bail
                var parameterType = parameter.Type;
                if (parameterType is IErrorTypeSymbol)
                {
                    return;
                }

                // do not support any parameter modifiers
                if (parameter.RefKind is not RefKind.None)
                {
                    this.RecordDiagnostic(Descriptors.ParameterHasRefModifier, parameter.Locations[0], parameter.Name);
                    return;
                }

                var isLocalizer = parameterType.ImplementsInterface(_typeSymbols.IStringLocalizer);
                parameterSpecs.Add(new MethodParameterSpec
                {
                    Name = parameter.Name,
                    Type = new TypeRef(parameterType),
                    IsLocalizer = isLocalizer,
                });

                // if parameter is not localizer, we continue
                if (!isLocalizer)
                {
                    continue;
                }

                // instance method cannot have localizer argument
                if (!isStatic)
                {
                    this.RecordDiagnostic(Descriptors.InstanceMethodHasLocalizerArgument, methodSyntax.GetLocation());
                    return;
                }

                // static method has more than 1 localizer
                if (foundLocalizer)
                {
                    this.RecordDiagnostic(Descriptors.MultipleLocalizerArguments, methodSyntax.GetLocation());
                    return;
                }

                foundLocalizer = true;
            }

            // static method is missing localizer
            if (isStatic && !foundLocalizer)
            {
                this.RecordDiagnostic(Descriptors.MissingLocalizerArgument, methodSyntax.GetLocation());
                return;
            }

            var typeBuilder = this.ResolveTypeBuilder(candidate.ClassSymbol, candidate.ClassSyntax);

            // check that instance method has access to localizer field
            if (!isStatic && typeBuilder.LocalizerField is null)
            {
                this.RecordDiagnostic(Descriptors.MissingLocalizerField, methodSyntax.GetLocation(), candidate.TypeRef.Name);
                return;
            }

            var methodSpec = new MethodSpec
            {
                Name = methodName,
                Modifiers = methodSyntax.Modifiers.ToString(),
                ReturnType = new TypeRef(methodSymbol.ReturnType),
                IsExtensionMethod = methodSymbol.IsExtensionMethod,
                IsStatic = isStatic,
                IsPartial = isPartial,
                ResourceId = candidate.ResourceId ?? methodName,
                Parameters = [.. parameterSpecs]
            };
            typeBuilder.AddMethod(methodSpec);
        }

        private void RecordDiagnostic(DiagnosticDescriptor descriptor, Location location, params object?[]? messageArgs)
        {
            Diagnostics ??= [];
            Diagnostics.Add(DiagnosticInfo.Create(descriptor, location, messageArgs));
        }

        private TypeSpecBuilder ResolveTypeBuilder(INamedTypeSymbol classSymbol, TypeDeclarationSyntax classSyntax)
        {
            var typeRef = new TypeRef(classSymbol);
            if (!_typeBuilders.TryGetValue(typeRef, out var typeBuilder))
            {
                var localizerField = this.FindLocalizerField(classSymbol);
                var namespaceName = ResolveNamespace(classSyntax);

                TypeSpecBuilder? parentBuilder = null;
                if (classSymbol.ContainingType is INamedTypeSymbol parentSymbol
                    && classSyntax.Parent is TypeDeclarationSyntax parentSyntax)
                {
                    parentBuilder = this.ResolveTypeBuilder(parentSymbol, parentSyntax);
                }

                _typeBuilders.Add(typeRef, typeBuilder = new TypeSpecBuilder
                {
                    TypeRef = typeRef,
                    ParentRef = parentBuilder?.TypeRef,
                    TypeName = GenerateTypeName(classSyntax),
                    Keyword = classSyntax.Keyword.ValueText,
                    Namespace = namespaceName,
                    LocalizerField = localizerField
                });
            }

            return typeBuilder;

            static string GenerateTypeName(TypeDeclarationSyntax typeDeclaration)
            {
                var parameterList = typeDeclaration.TypeParameterList;
                if (parameterList is not null && parameterList.Parameters.Count != 0)
                {
                    // The source generator produces a partial class that the compiler merges with the original
                    // class definition in the user code. If the user applies attributes to the generic types
                    // of the class, it is necessary to remove these attribute annotations from the generated
                    // code. Failure to do so may result in a compilation error (CS0579: Duplicate attribute).
                    for (var i = 0; i < parameterList.Parameters.Count; i++)
                    {
                        var parameter = parameterList.Parameters[i];

                        if (parameter.AttributeLists.Count > 0)
                        {
                            typeDeclaration = typeDeclaration.ReplaceNode(parameter, parameter.WithAttributeLists([]));
                        }
                    }
                }

                return typeDeclaration.Identifier.ToString() + typeDeclaration.TypeParameterList;
            }
        }

        private string? FindLocalizerField(INamedTypeSymbol classSymbol)
        {
            string? localizerField = null;

            var currentClass = classSymbol;
            var onMostDerivedType = true;

            // We keep track of the names of all non-localizer fields, since they prevent referring to localizer
            // primary constructor parameters with the same name. Example:
            // partial class C(IStringLocalizer localizer)
            // {
            //     private readonly object localizer = localizer;
            //
            //     [LocalizedString("M1")]
            //     public partial void M1(); // The IStringLocalizer primary constructor parameter cannot be used here.
            // }
            var shadowedNames = new HashSet<string>(StringComparer.Ordinal);

            while (currentClass is { SpecialType: not SpecialType.System_Object })
            {
                foreach (var fs in currentClass.GetMembers().OfType<IFieldSymbol>())
                {
                    // private field on parent type
                    if (!onMostDerivedType && fs.DeclaredAccessibility == Accessibility.Private)
                    {
                        continue;
                    }

                    if (!fs.CanBeReferencedByName)
                    {
                        continue;
                    }

                    if (!fs.Type.ImplementsInterface(_typeSymbols.IStringLocalizer))
                    {
                        shadowedNames.Add(fs.Name);
                        continue;
                    }

                    if (localizerField is not null)
                    {
                        this.RecordDiagnostic(Descriptors.MultipleLocalizerFields, fs.Locations[0], classSymbol.Name);
                        return null;
                    }

                    localizerField = fs.Name;
                }

                onMostDerivedType = false;
                currentClass = currentClass.BaseType;
            }

            // We prioritize fields over primary constructor parameters and avoid warnings if both exist.
            if (localizerField is not null)
            {
                return localizerField;
            }

            Debug.Assert(classSymbol is not null, "class is not null");
            foreach (var primaryConstructor in classSymbol!.InstanceConstructors)
            {
                if (!primaryConstructor.DeclaringSyntaxReferences.Any(ds => ds.GetSyntax() is ClassDeclarationSyntax))
                {
                    continue;
                }

                foreach (var parameter in primaryConstructor.Parameters)
                {
                    if (!parameter.Type.ImplementsInterface(_typeSymbols.IStringLocalizer))
                    {
                        continue;
                    }

                    if (shadowedNames.Contains(parameter.Name))
                    {
                        // Accessible fields always shadow primary constructor parameters,
                        // so we can't use the primary constructor parameter,
                        // even if the field is not a valid logger.
                        this.RecordDiagnostic(Descriptors.PrimaryConstructorParameterHidden, parameter.Locations[0], classSymbol.Name);
                        return null;
                    }

                    if (localizerField is not null)
                    {
                        this.RecordDiagnostic(Descriptors.MultipleLocalizerFields, parameter.Locations[0], classSymbol.Name);
                        return null;
                    }

                    localizerField = parameter.Name;
                }
            }

            return localizerField;
        }

        private static void ExtractMethodModifiers(MethodDeclarationSyntax methodDeclaration, out bool isStatic, out bool isPartial)
        {
            isStatic = false;
            isPartial = false;

            foreach (var mod in methodDeclaration.Modifiers)
            {
                if (mod.IsKind(SyntaxKind.PartialKeyword))
                {
                    isPartial = true;
                }
                else if (mod.IsKind(SyntaxKind.StaticKeyword))
                {
                    isStatic = true;
                }
            }
        }

        private static string? ResolveNamespace(TypeDeclarationSyntax classDeclaration)
        {
            var potentialNamespaceParent = classDeclaration.Parent;
            while (potentialNamespaceParent is not null
                and not NamespaceDeclarationSyntax
                and not FileScopedNamespaceDeclarationSyntax)
            {
                potentialNamespaceParent = potentialNamespaceParent.Parent;
            }

            if (potentialNamespaceParent is not BaseNamespaceDeclarationSyntax namespaceParent)
            {
                return null;
            }

            var name = namespaceParent.Name.ToString();
            while (namespaceParent.Parent is NamespaceDeclarationSyntax parent)
            {
                name = $"{parent.Name}.{name}";
                namespaceParent = parent;
            }
            return name;
        }
    }

    private sealed class TypeSpecBuilder
    {
        private List<MethodSpec>? _methods;

        public TypeSpecBuilder()
        {
        }

        public required TypeRef TypeRef { get; init; }
        public required TypeRef? ParentRef { get; init; }
        public required string TypeName { get; init; }
        public required string Keyword { get; init; }
        public required string? Namespace { get; init; }
        public required string? LocalizerField { get; init; }

        public void AddMethod(MethodSpec methodSpec)
        {
            _methods ??= [];
            _methods.Add(methodSpec);
        }

        public EquatableArray<MethodSpec>? GetMethods()
        {
            return _methods is null ? null : [.. _methods];
        }
    }
}
