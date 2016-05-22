// <copyright file="MibAnalyzer.cs" company="None">
//    <para>
//    This work is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published
//    by the Free Software Foundation; either version 2 of the License,
//    or (at your option) any later version.</para>
//    <para>
//    This work is distributed in the hope that it will be useful, but
//    WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//    General Public License for more details.</para>
//    <para>
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307
//    USA</para>
//    Original Java code Copyright (c) 2004-2016 Per Cederberg. All
//    rights reserved.
//    C# conversion Copyright (c) 2016 Jeremy Gibbons. All rights reserved
// </copyright>

namespace MibbleSharp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using MibbleSharp.Asn1;
    using MibbleSharp.Snmp;
    using MibbleSharp.Type;
    using MibbleSharp.Value;
    using PerCederberg.Grammatica.Runtime;

    /// <summary>
    /// A MIB file analyzer. This class analyzes the MIB file parse tree,
    /// and creates appropriate MIB modules with the right symbols. This
    /// analyzer handles imports by adding them to the MIB loader queue.
    /// As the imported MIB symbols aren't available during the analysis,
    /// type and value references will be created whenever an identifier
    /// is encountered.
    /// </summary>
    internal class MibAnalyzer : Asn1Analyzer
    {
        /// <summary>
        /// The list of MIB modules found.
        /// </summary>
        private IList<Mib> mibs = new List<Mib>();

        /// <summary>
        /// The MIB file being analyzed.
        /// </summary>
        private string file;

        /// <summary>
        /// The MIB loader using this analyzer.
        /// </summary>
        private MibLoader loader;

        /// <summary>
        /// The MIB loader log.
        /// </summary>
        private MibLoaderLog log;

        /// <summary>
        /// The current MIB module being analyzed.
        /// </summary>
        private Mib currentMib = null;

        /// <summary>
        /// The base MIB symbol context. This context will be extended
        /// when parsing the import list.
        /// </summary>
        private IMibContext baseContext = null;

        /// <summary>
        /// The MIB context stack. This stack is modified during the
        /// parsing to add type or import contexts as necessary. The top
        /// context on the stack is returned by the getContext() method.
        /// </summary>
        /// <see cref="getContext"/>
        private IList<IMibContext> contextStack = new List<IMibContext>();

        /// <summary>
        /// The implicit tags flag.
        /// </summary>
        private bool implicitTags = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="MibAnalyzer"/> class.
        /// </summary>
        /// <param name="file">The MIB file being analyzed</param>
        /// <param name="loader">The MIB loader using this analyzer</param>
        /// <param name="log">The MIB loader log to be used</param>
        public MibAnalyzer(string file, MibLoader loader, MibLoaderLog log)
        {
            this.file = file;
            this.loader = loader;
            this.log = log;
        }

        /// <summary>
        /// Gets the list of MIB modules found during analysis.
        /// </summary>
        public IEnumerable<Mib> Mibs
        {
            get
            {
                return this.mibs;
            }
        }

        /// <summary>
        /// Gets the top context on the context stack.
        /// </summary>
        private IMibContext TopContext
        {
            get
            {
                return this.contextStack[this.contextStack.Count - 1];
            }
        }

        /// <summary>
        /// Resets this analyzer. This method is mostly used to release
        /// all references to parsed data.
        /// </summary>
        public override void Reset()
        {
            this.mibs = new List<Mib>();
            this.currentMib = null;
            this.baseContext = null;
            this.contextStack.Clear();
            this.implicitTags = true;
        }

        /// <summary>
        /// Adds the binary number as a node value. This method will
        /// convert the binary string to a BigInteger.
        /// </summary>
        /// <param name="node">The node being exited</param>
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitBinaryString(Token node)
        {
            string str = node.GetImage();

            str = str.Substring(1, str.Length - 2);
            long value = Convert.ToInt64(str, 2);

            node.Values.Add((BigInteger)value);
            node.Values.Add(str);
            return node;
        }

        /// <summary>
        /// Adds the hexadecimal number as a node value. This method will
        /// convert the hexadecimal string to a BigInteger.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitHexadecimalString(Token node)
        {
            string str = node.GetImage();

            str = str.Substring(1, str.Length - 2);
            long value = Convert.ToInt64(str, 16);
            node.AddValue((BigInteger)value);
            node.AddValue(str);
            return node;
        }

        /// <summary>
        /// Adds the quoted string as a node value. This method will
        /// remove the quotation marks and replace any double marks inside
        /// the string with a single mark.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitQuotedString(Token node)
        {
            string str = node.GetImage();
            int pos;

            str = str.Substring(1, str.Length - 1);
            do
            {
                pos = str.IndexOf("\"\"");
                if (pos >= 0)
                {
                    str = str.Substring(0, pos) + '"' + str.Substring(pos + 2);
                }
            }
            while (pos >= 0);

            node.AddValue(str);
            return node;
        }

        /// <summary>
        /// Adds the identifier string as a node value.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitIdentifierString(Token node)
        {
            node.AddValue(node.GetImage());
            return node;
        }

        /// <summary>
        /// Adds the number as a node value. This method will convert the
        /// number string to a BigInteger.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitNumberString(Token node)
        {
            string str = node.GetImage();
            BigInteger value = BigInteger.Parse(str);
            node.AddValue(value);
            return node;
        }

        /// <summary>
        /// Stores any MIB tail comments if available.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitStart(Production node)
        {
            string comment = MibAnalyzerUtil.GetCommentsFooter(node);

            if (this.currentMib != null)
            {
                this.currentMib.FooterComment = comment;
            }

            return null;
        }

        /// <summary>
        /// Creates the current MIB module container and the base context.
        /// </summary>
        /// <param name="node">The node being entered</param>  
        public override void EnterModuleDefinition(Production node)
        {
            this.currentMib = new Mib(this.file, this.loader, this.log);
            this.baseContext = this.loader.getDefaultContext();
            this.baseContext = new CompoundContext(this.currentMib, this.baseContext);
            this.PushContext(this.baseContext);
        }

        /// <summary>
        /// Sets the MIB name to the module identifier string value. Also
        /// removes this node from the parse tree.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException">
        /// If the node analysis discovered errors
        /// </exception>
        public override Node ExitModuleDefinition(Production node)
        {
            this.currentMib.Name = this.GetStringValue(this.GetChildAt(node, 0), 0);
            this.currentMib.HeaderComment = MibAnalyzerUtil.GetComments(node);
            this.mibs.Add(this.currentMib);
            return node;
        }

        /// <summary>
        /// Adds the module identifier string as a node value.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException">
        /// If the node analysis discovered errors
        /// </exception> 
        public override Node ExitModuleIdentifier(Production node)
        {
            node.AddValue(this.GetStringValue(this.GetChildAt(node, 0), 0));
            return node;
        }

        /// <summary>
        /// Adds the module identifier string as a node value.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException">
        /// If the node analysis discovered errors
        /// </exception> 
        public override Node ExitModuleReference(Production node)
        {
            node.AddValue(this.GetStringValue(this.GetChildAt(node, 0), 0));
            return node;
        }

        /// <summary>Sets the implicit tags flag.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>null to remove the node from the parse tree.</returns>
        /// <exception cref="ParseException">
        /// If the node analysis discovered errors
        /// </exception> 
        public override Node ExitTagDefault(Production node)
        {
            Node child;

            child = this.GetChildAt(node, 0);
            if (child.GetId() == (int)Asn1Constants.EXPLICIT)
            {
                this.implicitTags = false;
            }
            else
            {
                this.implicitTags = true;
            }

            return null;
        }

        /// <summary>
        /// Adds all imported MIB files to the MIB context. Also removes
        /// this node from the parse tree.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>null so the node is removed</returns>
        public override Node ExitImportList(Production node)
        {
            ArrayList imports = this.GetChildValues(node);
            MibImport imp;
            IMibContext current = this.loader.getDefaultContext();
            bool addMissingSmi = true;

            foreach (var import in imports)
            {
                imp = import as MibImport;
                if (imp != null &&
                    (imp.Name.StartsWith("RFC1065-SMI") ||
                    imp.Name.StartsWith("RFC1155-SMI") ||
                    imp.Name.StartsWith("SNMPv2-SMI")))
                {
                    addMissingSmi = false;
                }
            }

            if (addMissingSmi)
            {
                // TODO: Ugly hack that adds a "hidden" SNMPv1 SMI as the last
                //       import, but without any named symbols (triggering
                //       warnings for each symbol used).
                imp = new MibImport(this.loader, this.GetLocation(node), "RFC1155-SMI", new List<MibSymbol>());
                this.loader.scheduleLoad(imp.Name);
                this.currentMib.AddImport(imp);
                imports.Add(imp);
            }

            for (int i = imports.Count - 1; i >= 0; i--)
            {
                imp = (MibImport)imports[i];
                current = new CompoundContext(imp, current);
            }

            this.baseContext = new CompoundContext(this.currentMib, current);
            this.PopContext();
            this.PushContext(this.baseContext);
            return null;
        }

        /// <summary>
        /// Schedules the imported MIB file for loading. Also adds a MIB
        /// reference as a node value.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException">
        /// If the node analysis discovered errors
        /// </exception> 
        public override Node ExitSymbolsFromModule(Production node)
        {
            MibImport imp;
            string module;
            System.Collections.ArrayList symbols;
            Node child;

            // Create MIB reference
            child = this.GetChildAt(node, 0);
            symbols = child.Values;
            if (symbols == null)
            {
                symbols = new ArrayList();
            }

            child = this.GetChildAt(node, 2);
            module = this.GetStringValue(child, 0);
            imp = new MibImport(this.loader, this.GetLocation(child), module, new List<MibSymbol>());

            // Schedule MIB loading
            this.loader.scheduleLoad(module);

            // Add reference to MIB and node
            this.currentMib.AddImport(imp);
            node.AddValue(imp);
            return node;
        }

        /// <summary>
        /// Adds all symbol identifiers as node values.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSymbolList(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>
        /// Adds the identifier string as a node value. If the symbol name
        /// is not an identifier, no node value will be added.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSymbol(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>
        /// Creates a macro symbol and adds it to the MIB. Also removes
        /// this node from the parse tree.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitMacroDefinition(Production node)
        {
            string name;
            MibMacroSymbol symbol;

            // Check macro name
            name = this.GetStringValue(this.GetChildAt(node, 0), 0);
            if (this.currentMib.GetSymbol(name) != null)
            {
                throw new ParseException(
                    ParseException.ErrorType.ANALYSIS,
                    "a symbol '" + name + "' already present in the MIB",
                    node.GetStartLine(),
                    node.GetStartColumn());
            }

            // Create macro symbol
            symbol = new MibMacroSymbol(
                this.GetLocation(node),
                this.currentMib,
                name);
            symbol.Comment = MibAnalyzerUtil.GetComments(node);

            return null;
        }

        /// <summary>Adds the macro name as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitMacroReference(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>
        /// Creates a type symbol and adds it to the MIB. Also removes
        /// this node from the parse tree.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitTypeAssignment(Production node)
        {
            string name;
            MibType type;
            MibTypeSymbol symbol;

            // Check type name
            name = this.GetStringValue(this.GetChildAt(node, 0), 0);
            if (this.currentMib.GetSymbol(name) != null)
            {
                throw new ParseException(
                    ParseException.ErrorType.ANALYSIS,
                    "a symbol '" + name + "' already present in the MIB",
                    node.GetStartLine(),
                    node.GetStartColumn());
            }

            if (!char.IsUpper(name[0]))
            {
                string warning = "type identifier '" + name + "' doesn't " +
                    "start with an uppercase character";
                this.log.AddWarning(this.GetLocation(node), warning);
            }

            // Create type symbol
            type = (MibType)this.GetValue(this.GetChildAt(node, 2), 0);
            symbol = new MibTypeSymbol(
                this.GetLocation(node),
                this.currentMib,
                name,
                type);
            symbol.Comment = MibAnalyzerUtil.GetComments(node);

            return null;
        }

        /// <summary>Adds a MIB type as the node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitType(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>Adds a type reference as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitDefinedType(Production node)
        {
            IMibContext local = this.TopContext;
            string name = null;
            object value = null;
            FileLocation loc = this.GetLocation(node);
            Node child;

            for (int i = 0; i < node.GetChildCount(); i++)
            {
                child = node.GetChildAt(i);
                switch ((Asn1Constants)child.GetId())
                {
                    case Asn1Constants.MODULE_REFERENCE:
                        name = this.GetStringValue(child, 0);
                        local = this.currentMib.GetImport(name);
                        if (local == null)
                        {
                            throw new ParseException(
                                ParseException.ErrorType.ANALYSIS,
                                "referenced module not imported '" + name + "'",
                                child.GetStartLine(),
                                child.GetStartColumn());
                        }

                        break;
                    case Asn1Constants.IDENTIFIER_STRING:
                        name = this.GetStringValue(child, 0);
                        break;
                    case Asn1Constants.VALUE_OR_CONSTRAINT_LIST:
                        value = this.GetValue(child, 0);
                        break;
                }
            }

            if (value is Constraint)
            {
                value = new TypeReference(loc, local, name, (Constraint)value);
            }
            else if (value is ArrayList)
            {
                ArrayList al = value as ArrayList;
                value = new TypeReference(loc, local, name, al.OfType<MibValueSymbol>().ToList());
            }
            else
            {
                value = new TypeReference(loc, local, name);
            }

            node.AddValue(value);
            return node;
        }

        /// <summary>Adds a MIB type as the node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitBuiltinType(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>Adds a null type as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitNullType(Production node)
        {
            node.AddValue(new NullType());
            return node;
        }

        /// <summary>Adds a boolean type as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitBooleanType(Production node)
        {
            node.AddValue(new BooleanType());
            return node;
        }

        /// <summary>Adds a real type as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitRealType(Production node)
        {
            node.AddValue(new RealType());
            return node;
        }

        /// <summary>Adds an integer type as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitIntegerType(Production node)
        {
            IntegerType type;
            ArrayList values;
            object obj;

            values = this.GetChildValues(node);
            if (values.Count == 0)
            {
                type = new IntegerType();
            }
            else
            {
                obj = values[0];
                if (obj is System.Collections.ArrayList)
                {
                    ArrayList al = obj as ArrayList;
                    type = new IntegerType(al.OfType<MibValueSymbol>().ToList());
                }
                else
                {
                    type = new IntegerType((Constraint)obj);
                }
            }

            node.AddValue(type);
            return node;
        }

        /// <summary>Adds an object identifier type as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitObjectIdentifierType(Production node)
        {
            node.AddValue(new ObjectIdentifierType());
            return node;
        }

        /// <summary>Adds a string type as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitStringType(Production node)
        {
            StringType type;
            System.Collections.ArrayList values;

            values = this.GetChildValues(node);
            if (values.Count == 0)
            {
                type = new StringType();
            }
            else
            {
                type = new StringType((Constraint)values[0]);
            }

            node.AddValue(type);
            return node;
        }

        /// <summary>Adds a bit set type as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitBitStringType(Production node)
        {
            BitSetType type;
            System.Collections.ArrayList values;
            object obj;

            values = this.GetChildValues(node);
            if (values.Count == 0)
            {
                type = new BitSetType();
            }
            else
            {
                obj = values[0];
                if (obj is System.Collections.ArrayList)
                {
                    ArrayList al = obj as ArrayList;
                    type = new BitSetType(al.OfType<MibValueSymbol>().ToList());
                }
                else
                {
                    type = new BitSetType((Constraint)obj);
                }
            }

            node.AddValue(type);
            return node;
        }

        /// <summary>Adds a bit set type as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitBitsType(Production node)
        {
            return this.ExitBitStringType(node);
        }

        /// <summary>Adds a MIB sequence type as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSequenceType(Production node)
        {
            System.Collections.ArrayList elements = this.GetChildValues(node);

            node.AddValue(new SequenceType(elements.OfType<ElementType>().ToList()));
            return node;
        }

        /// <summary>Adds a sequence of MIB type as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSequenceOfType(Production node)
        {
            MibType type;
            Constraint c = null;
            Node child;

            child = this.GetChildAt(node, node.GetChildCount() - 1);
            type = (MibType)this.GetValue(child, 0);
            if (node.GetChildCount() == 4)
            {
                child = this.GetChildAt(node, 1);
                c = (Constraint)this.GetValue(child, 0);
            }

            node.AddValue(new SequenceOfType(type, c));
            return node;
        }

        /// <summary>
        /// Adds a null type as a node value. This method also prints an
        /// error about this construct being unsupported.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSetType(Production node)
        {
            // TODO: implement set type support
            this.log.AddError(
                this.GetLocation(node),
                "SET type currently unsupported");
            node.AddValue(new NullType());
            return node;
        }

        /// <summary>
        /// Adds a null type as a node value. This method also prints an
        /// error about this construct being unsupported.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSetOfType(Production node)
        {
            // TODO: implement set of type support
            this.log.AddError(
                this.GetLocation(node),
                "SET OF type currently unsupported");
            node.AddValue(new NullType());
            return node;
        }

        /// <summary>Adds a MIB choice type as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitChoiceType(Production node)
        {
            ArrayList al = this.GetChildValues(node);
            node.AddValue(new ChoiceType(al.OfType<ElementType>().ToList()));
            return node;
        }

        /// <summary>
        /// Adds a null type as a node value. This method also prints an
        /// error about this construct being unsupported.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitEnumeratedType(Production node)
        {
            // TODO: implement enumerated type support
            this.log.AddError(
                this.GetLocation(node),
                "ENUMERATED type currently unsupported");
            node.AddValue(new NullType());
            return node;
        }

        /// <summary>
        /// Adds a null type as a node value. This method also prints an
        /// error about this construct being unsupported.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSelectionType(Production node)
        {
            // TODO: implement selection type support
            this.log.AddError(
                this.GetLocation(node),
                "selection type currently unsupported");
            node.AddValue(new NullType());
            return node;
        }

        /// <summary>Adds the tagged type as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitTaggedType(Production node)
        {
            MibType type;
            MibTypeTag tag;
            bool implicitly = this.implicitTags;

            Node child;

            child = this.GetChildAt(node, 0);

            tag = (MibTypeTag)this.GetValue(child, 0);

            child = this.GetChildAt(node, 1);
            if (child.GetId() == (int)Asn1Constants.EXPLICIT_OR_IMPLICIT_TAG)
            {
                implicitly = (bool)this.GetValue(child, 0);
            }

            child = this.GetChildAt(node, node.GetChildCount() - 1);
            type = (MibType)this.GetValue(child, 0);
            type.SetTag(implicitly, tag);
            node.AddValue(type);
            return node;
        }

        /// <summary>Called when exiting a parse tree node.</summary>
        /// <param name="node">The node being exited</param>
        /// <returns>
        /// The node to add to the parse tree, or null if no parse
        /// tree should be created
        /// </returns>
        public override Node ExitTag(Production node)
        {
            System.Collections.ArrayList values = this.GetChildValues(node);
            int category = MibTypeTag.ContextSpecificCategory;
            int value;

            if (values.Count == 1)
            {
                value = (int)((BigInteger)values[0]);
            }
            else
            {
                category = (int)values[0];
                value = (int)((BigInteger)values[1]);
            }

            node.AddValue(new MibTypeTag(category, value));
            return node;
        }

        /// <summary>Adds the type tag category value as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitClass(Production node)
        {
            Node child = GetChildAt(node, 0);
            int category;

            if (child.GetId() == (int)Asn1Constants.UNIVERSAL)
            {
                category = MibTypeTag.UniversalCategory;
            }
            else if (child.GetId() == (int)Asn1Constants.APPLICATION)
            {
                category = MibTypeTag.ApplicationCategory;
            }
            else if (child.GetId() == (int)Asn1Constants.PRIVATE)
            {
                category = MibTypeTag.PrivateCategory;
            }
            else
            {
                category = MibTypeTag.ContextSpecificCategory;
            }

            node.AddValue(category);
            return node;
        }

        /// <summary>Adds the implicit boolean flag as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitExplicitOrImplicitTag(Production node)
        {
            Node child = GetChildAt(node, 0);

            if (child.GetId() == (int)Asn1Constants.EXPLICIT)
            {
                node.AddValue(false);
            }
            else
            {
                node.AddValue(true);
            }

            return node;
        }

        /// <summary>
        /// Adds a null type as a node value. This method also prints an
        /// error about this construct being unsupported.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitAnyType(Production node)
        {
            // TODO: implement any type support
            this.log.AddError(
                this.GetLocation(node),
                "ANY type currently unsupported");
            node.AddValue(new NullType());
            return node;
        }

        /// <summary>Adds all element types as a node values.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitElementTypeList(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>Adds an element type as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitElementType(Production node)
        {
            string name = null;
            MibType type;
            Node child;

            child = this.GetChildAt(node, 0);
            if (child.GetId() == (int)Asn1Constants.IDENTIFIER_STRING)
            {
                name = this.GetStringValue(child, 0);
                child = this.GetChildAt(node, 1);
            }

            if (child.GetId() != (int)Asn1Constants.TYPE)
            {
                throw new ParseException(
                    ParseException.ErrorType.ANALYSIS,
                    "referencing components is currently unsupported",
                    child.GetStartLine(),
                    child.GetStartColumn());
            }

            type = new ElementType(name, (MibType)this.GetValue(child, 0));
            type.Comment = MibAnalyzerUtil.GetComments(node);
            node.AddValue(type);
            return node;
        }

        /// <summary>Prints an error about this construct being unsupported.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitOptionalOrDefaultElement(Production node)
        {
            // TODO: implement this method?
            this.log.AddError(
                this.GetLocation(node),
                "optional and default elements are currently unsupported");
            return null;
        }

        /// <summary>
        /// Adds an array list with symbols or a constraint as the node
        /// value.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitValueOrConstraintList(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>
        /// Adds an array list with symbols as the node value.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitNamedNumberList(Production node)
        {
            MibValueSymbol symbol;
            Node child;

            for (int i = 0; i < node.GetChildCount(); i++)
            {
                child = node.GetChildAt(i);
                if (child.GetId() == (int)Asn1Constants.NAMED_NUMBER)
                {
                    symbol = (MibValueSymbol)child.GetValue(0);
                    symbol.Comment = MibAnalyzerUtil.GetComments(child);
                }
            }

            node.AddValue(this.GetChildValues(node));
            return node;
        }

        /// <summary>Adds a value symbol as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitNamedNumber(Production node)
        {
            MibValueSymbol symbol;
            string name;
            MibValue value;

            name = this.GetStringValue(this.GetChildAt(node, 0), 0);
            value = (MibValue)this.GetValue(this.GetChildAt(node, 2), 0);
            symbol = new MibValueSymbol(
                this.GetLocation(node),
                null,
                name,
                null,
                value);
            node.AddValue(symbol);
            return node;
        }

        /// <summary>Adds a MIB value as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitNumber(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>Adds a MIB type constraint as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitConstraintList(Production node)
        {
            Constraint result = null;
            System.Collections.ArrayList values;
            Constraint c;

            values = this.GetChildValues(node);
            for (int i = values.Count - 1; i >= 0; i--)
            {
                c = (Constraint)values[i];
                if (result == null)
                {
                    result = c;
                }
                else
                {
                    result = new CompoundConstraint(c, result);
                }
            }

            node.AddValue(result);
            return node;
        }

        /// <summary>Adds a MIB type constraint as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitConstraint(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>Adds a MIB type constraint as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitValueConstraintList(Production node)
        {
            return this.ExitConstraintList(node);
        }

        /// <summary>
        /// Adds a MIB type value or value range constraint as a node
        /// value.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitValueConstraint(Production node)
        {
            System.Collections.ArrayList list = this.GetChildValues(node);
            NumberValue lower = null;
            NumberValue upper = null;
            bool strictLower = false;
            bool strictUpper = false;
            object obj;

            if (list.Count == 0)
            {
                throw new ParseException(
                    ParseException.ErrorType.ANALYSIS,
                    "no value specified in constraint",
                    node.GetStartLine(),
                    node.GetStartColumn());
            }
            else if (list.Count == 1)
            {
                lower = (NumberValue)list[0];
                obj = new ValueConstraint(this.GetLocation(node), lower);
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    obj = list[i];
                    if (obj is bool && strictLower == false)
                    {
                        strictLower = (bool)obj;
                    }
                    else if (obj is bool)
                    {
                        strictUpper = (bool)obj;
                    }
                    else if (strictLower == false)
                    {
                        lower = (NumberValue)obj;
                    }
                    else
                    {
                        upper = (NumberValue)obj;
                    }
                }

                obj = new ValueRangeConstraint(
                    this.GetLocation(node),
                    lower,
                    strictLower,
                    upper,
                    strictUpper);
            }

            node.AddValue(obj);
            return node;
        }

        /// <summary>
        /// Adds the upper end point and strict inequality flags as node
        /// values.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitValueRange(Production node)
        {
            Node child;

            // Check for strict lower end point
            child = this.GetChildAt(node, 0);
            if (child.GetId() == (int)Asn1Constants.LESS_THAN)
            {
                node.AddValue(true);
            }
            else
            {
                node.AddValue(false);
            }

            // Add upper end point (or null)
            child = this.GetChildAt(node, node.GetChildCount() - 1);
            node.AddValue(child.GetValue(0));

            // Check for strict upper end point
            child = this.GetChildAt(node, node.GetChildCount() - 2);
            if (child.GetId() == (int)Asn1Constants.LESS_THAN)
            {
                node.AddValue(true);
            }
            else
            {
                node.AddValue(false);
            }

            return node;
        }

        /// <summary>
        /// Adds a MIB value or null as a node value. The null value is
        /// used to represent a minimum value.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitLowerEndPoint(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>
        /// Adds a MIB value or null as a node value. The null value is
        /// used to represent a maximum value.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitUpperEndPoint(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>
        /// Adds a MIB type size constraint as a node value.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSizeConstraint(Production node)
        {
            Constraint c;

            c = (Constraint)this.GetValue(this.GetChildAt(node, 1), 0);
            node.AddValue(new SizeConstraint(this.GetLocation(node), c));
            return node;
        }

        /// <summary>
        /// Removes this node from the parse tree, and prints an error
        /// about this construct being unsupported.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitAlphabetConstraint(Production node)
        {
            // TODO: implement alphabet constraints
            this.log.AddError(
                this.GetLocation(node),
                "FROM constraints are currently unsupported");
            return null;
        }

        /// <summary>
        /// Removes this node from the parse tree, and prints an error
        /// about this construct being unsupported.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitContainedTypeConstraint(Production node)
        {
            // TODO: implement contained type constraints
            this.log.AddError(
                this.GetLocation(node),
                "INCLUDES constraints are currently unsupported");
            return null;
        }

        /// <summary>
        /// Removes this node from the parse tree, and prints an error
        /// about this construct being unsupported.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitInnerTypeConstraint(Production node)
        {
            // TODO: implement inner type constraints
            this.log.AddError(
                this.GetLocation(node),
                "WITH COMPONENT(S) constraints are currently unsupported");
            return null;
        }

        /// <summary>
        /// Creates a value symbol and adds it to the MIB. Also removes
        /// this node from the parse tree.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitValueAssignment(Production node)
        {
            string name;
            MibType type;
            MibValue value;
            MibValueSymbol symbol;

            // Check value name
            name = this.GetStringValue(this.GetChildAt(node, 0), 0);
            if (this.currentMib.GetSymbol(name) != null)
            {
                throw new ParseException(
                    ParseException.ErrorType.ANALYSIS,
                    "a symbol '" + name + "' already present in the MIB",
                    node.GetStartLine(),
                    node.GetStartColumn());
            }

            if (!char.IsLower(name[0]))
            {
                string warning = "value identifier '" + name + "' doesn't " +
                    "start with a lowercase character";
                this.log.AddWarning(
                    this.GetLocation(node),
                    warning);
            }

            // Create value symbol
            type = (MibType)this.GetValue(this.GetChildAt(node, 1), 0);
            value = (MibValue)this.GetValue(this.GetChildAt(node, 3), 0);
            symbol = new MibValueSymbol(
                this.GetLocation(node),
                this.currentMib,
                name,
                type,
                value);
            symbol.Comment = MibAnalyzerUtil.GetComments(node);

            return null;
        }

        /// <summary>Adds a MIB value as the node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitValue(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }
        
        /// <summary>Adds a value reference as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitDefinedValue(Production node)
        {
            ValueReference vref;
            IMibContext local = this.TopContext;
            string name;
            Node child;

            // Check for module reference
            child = this.GetChildAt(node, 0);
            if (child.GetId() == (int)Asn1Constants.MODULE_REFERENCE)
            {
                name = this.GetStringValue(child, 0);
                local = this.currentMib.GetImport(name);
                if (local == null)
                {
                    throw new ParseException(
                        ParseException.ErrorType.ANALYSIS,
                        "referenced module not imported '" + name + "'",
                        child.GetStartLine(),
                        child.GetStartColumn());
                }

                child = this.GetChildAt(node, 1);
            }

            // Create value reference
            name = this.GetStringValue(child, 0);
            vref = new ValueReference(this.GetLocation(node), local, name);
            node.AddValue(vref);
            return node;
        }

        /// <summary>Adds a MIB value as the node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitBuiltinValue(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>Adds a MIB null value as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitNullValue(Production node)
        {
            node.AddValue(NullValue.NULL);
            return node;
        }

        /// <summary>Adds a MIB boolean value as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitBooleanValue(Production node)
        {
            Node child = GetChildAt(node, 0);

            if (child.GetId() == (int)Asn1Constants.TRUE)
            {
                node.AddValue(BooleanValue.TRUE);
            }
            else
            {
                node.AddValue(BooleanValue.FALSE);
            }

            return node;
        }

        /// <summary>Adds a MIB number value as the node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSpecialRealValue(Production node)
        {
            double d;

            if (this.GetChildAt(node, 0).GetId() == (int)Asn1Constants.PLUS_INFINITY)
            {
                d = double.PositiveInfinity;
            }
            else
            {
                d = double.NegativeInfinity;
            }

            node.AddValue(new RealValue(d));
            return node;
        }

        /// <summary>Adds a MIB number value as the node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitNumberValue(Production node)
        {
            BigInteger number;

            if (this.GetChildAt(node, 0).GetId() == (int)Asn1Constants.MINUS)
            {
                number = (BigInteger)this.GetValue(this.GetChildAt(node, 1), 0);
                number = -number;
            }
            else
            {
                number = (BigInteger)this.GetValue(this.GetChildAt(node, 0), 0);
            }

            node.AddValue(new NumberValue(number));
            return node;
        }

        /// <summary>Adds a MIB number value as the node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitBinaryValue(Production node)
        {
            Node child;
            BigInteger number;
            string text;

            child = this.GetChildAt(node, 0);
            number = (BigInteger)child.GetValue(0);
            text = (string)child.GetValue(1);
            node.AddValue(new BinaryNumberValue(number, text.Length));
            return node;
        }

        /// <summary>Adds a MIB number value as the node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitHexadecimalValue(Production node)
        {
            Node child;
            BigInteger number;
            string text;

            child = this.GetChildAt(node, 0);
            number = (BigInteger)child.GetValue(0);
            text = (string)child.GetValue(1);
            node.AddValue(new HexNumberValue(number, text.Length));
            return node;
        }

        /// <summary>Adds a MIB string value as the node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitStringValue(Production node)
        {
            string str;

            str = this.GetStringValue(this.GetChildAt(node, 0), 0);
            node.AddValue(new StringValue(str));
            return node;
        }

        /// <summary>Adds a MIB value as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitBitOrObjectIdentifierValue(Production node)
        {
            if (MibAnalyzerUtil.IsBitValue(node))
            {
                return this.ExitBitValue(node);
            }
            else
            {
                return this.ExitObjectIdentifierValue(node);
            }
        }

        /// <summary>Adds a MIB bit set value as the node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitBitValue(Production node)
        {
            System.Collections.ArrayList components = this.GetChildValues(node);
            System.Collections.BitArray bits = new System.Collections.BitArray(64);
            IList<ValueReference> values = new List<ValueReference>();
            NamedNumber number;

            for (int i = 0; i < components.Count; i++)
            {
                number = (NamedNumber)components[i];
                if (number.hasNumber())
                {
                    bits.Set((int)number.Number, true);
                }
                else
                {
                    values.Add(number.Reference);
                }
            }

            node.AddValue(new BitSetValue(bits, values));
            return node;
        }

        /// <summary>Adds a MIB object identifier value as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitObjectIdentifierValue(Production node)
        {
            System.Collections.ArrayList components = this.GetChildValues(node);
            MibValue parent = null;
            NamedNumber number;
            int value;

            // Check for minimum number of components
            if (components.Count < 1)
            {
                throw new ParseException(
                    ParseException.ErrorType.ANALYSIS,
                    "object identifier must contain at least one component",
                    node.GetStartLine(),
                    node.GetStartColumn());
            }

            // Analyze components
            for (int i = 0; i < components.Count; i++)
            {
                number = (NamedNumber)components[i];
                if (number.hasNumber())
                {
                    value = number.IntValue;
                    if (parent == null && value == 0)
                    {
                        parent = new ValueReference(
                            this.GetLocation(node),
                            this.TopContext,
                            DefaultContext.CCITT);
                    }
                    else if (parent == null && value == 1)
                    {
                        parent = new ValueReference(
                            this.GetLocation(node),
                            this.TopContext,
                            DefaultContext.ISO);
                    }
                    else if (parent == null && value == 2)
                    {
                        parent = new ValueReference(
                            this.GetLocation(node),
                            this.TopContext,
                            DefaultContext.JOINTISOCCITT);
                    }
                    else if (parent is ObjectIdentifierValue)
                    {
                        try
                        {
                            parent = new ObjectIdentifierValue(
                                this.GetLocation(node),
                                (ObjectIdentifierValue)parent,
                                number.Name,
                                value);
                        }
                        catch (MibException e)
                        {
                            this.log.AddError(e.Location, e.Message);
                            parent = null;
                        }
                    }
                    else
                    {
                        parent = new ObjectIdentifierValue(
                                            this.GetLocation(node),
                                            (ValueReference)parent,
                                            number.Name,
                                            value);
                    }
                }
                else if (parent != null)
                {
                    string error = "object identifier component '" + number.Name +
                        "' has been previously defined, remove any " +
                        "components to the left";
                    throw new ParseException(
                        ParseException.ErrorType.ANALYSIS,
                        error,
                        node.GetStartLine(),
                        node.GetStartColumn());
                }
                else
                {
                    parent = number.Reference;
                }
            }

            // Set node value
            node.AddValue(parent);
            return node;
        }

        /// <summary>Adds all the named numbers as the node values.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitNameValueList(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>Adds a named number as the node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitNameValueComponent(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>Adds a named number as the node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitNameOrNumber(Production node)
        {
            NamedNumber value;
            object obj;
            ValueReference vref;

            obj = this.GetValue(this.GetChildAt(node, 0), 0);
            if (obj is BigInteger)
            {
                value = new NamedNumber((BigInteger)obj);
            }
            else if (obj is string)
            {
                vref = new ValueReference(
                    this.GetLocation(node),
                    this.TopContext,
                    (string)obj);
                value = new NamedNumber((string)obj, vref);
            }
            else
            {
                value = (NamedNumber)obj;
            }

            node.AddValue(value);
            return node;
        }

        /// <summary>Adds a named number as the node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitNameAndNumber(Production node)
        {
            NamedNumber value;
            string name;
            object obj;

            name = this.GetStringValue(this.GetChildAt(node, 0), 0);
            obj = this.GetValue(this.GetChildAt(node, 2), 0);
            if (obj is BigInteger)
            {
                value = new NamedNumber(name, (BigInteger)obj);
            }
            else
            {
                value = new NamedNumber(name, (ValueReference)obj);
            }

            node.AddValue(value);
            return node;
        }

        /// <summary>Adds an SNMP type as the node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitDefinedMacroType(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>Adds the defined macro name as the node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitDefinedMacroName(Production node)
        {
            node.AddValue(((Token)node.GetChildAt(0)).GetImage());
            return node;
        }

        /// <summary>Adds an SNMP module identity as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSnmpModuleIdentityMacroType(Production node)
        {
            string update;
            string org;
            string contact;
            string desc;
            System.Collections.ArrayList revisions = new System.Collections.ArrayList();

            this.currentMib.SmiVersion = 2;
            update = this.GetStringValue(this.GetChildAt(node, 1), 0);
            org = this.GetStringValue(this.GetChildAt(node, 2), 0);
            contact = this.GetStringValue(this.GetChildAt(node, 3), 0);
            desc = this.GetStringValue(this.GetChildAt(node, 4), 0);
            for (int i = 5; i < node.GetChildCount(); i++)
            {
                revisions.Add(this.GetValue(this.GetChildAt(node, i), 0));
            }

            node.AddValue(
                new SnmpModuleIdentity(
                    update,
                    org,
                    contact,
                    desc,
                    revisions.OfType<SnmpRevision>().ToList()));
            return node;
        }
        
        /// <summary>Called when exiting a parse tree node.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSnmpObjectIdentityMacroType(Production node)
        {
            SnmpStatus status;
            string desc;
            string sref;

            this.currentMib.SmiVersion = 2;
            status = (SnmpStatus)this.GetValue(this.GetChildAt(node, 1), 0);
            desc = this.GetStringValue(this.GetChildAt(node, 2), 0);
            if (node.GetChildCount() <= 3)
            {
                sref = null;
            }
            else
            {
                sref = this.GetStringValue(this.GetChildAt(node, 3), 0);
            }

            node.AddValue(new SnmpObjectIdentity(status, desc, sref));
            return node;
        }

        /// <summary>Adds the syntax type to the MIB context stack if possible.</summary>
        /// <param name="node">The parent node</param>
        /// <param name="child">The child node or null</param>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override void ChildSnmpObjectTypeMacroType(Production node, Node child)
        {
            MibType type;

            if (child.GetId() == (int)Asn1Constants.SNMP_SYNTAX_PART)
            {
                type = (MibType)this.GetValue(child, 0);
                if (type is IMibContext)
                {
                    this.PushContextExtension((IMibContext)type);
                }
            }

            node.AddChild(child);
        }

        /// <summary>
        /// Adds an SNMP object type as a node value. This method also
        /// removes any syntax type from the MIB context stack if needed.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSnmpObjectTypeMacroType(Production node)
        {
            SnmpObjectType type;
            MibType syntax = null;
            string units = null;
            SnmpAccess access = null;
            SnmpStatus status = null;
            string desc = null;
            string sref = null;
            object index = null;
            MibValue defVal = null;
            Node child;

            for (int i = 0; i < node.GetChildCount(); i++)
            {
                child = node.GetChildAt(i);
                switch ((Asn1Constants)child.GetId())
                {
                    case Asn1Constants.SNMP_SYNTAX_PART:
                        syntax = (MibType)this.GetValue(child, 0);
                        if (syntax is IMibContext)
                        {
                            this.PopContext();
                        }

                        syntax.Comment = MibAnalyzerUtil.GetComments(child);
                        break;
                    case Asn1Constants.SNMP_UNITS_PART:
                        units = this.GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_ACCESS_PART:
                        access = (SnmpAccess)this.GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_STATUS_PART:
                        status = (SnmpStatus)this.GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_DESCR_PART:
                        desc = this.GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_REFER_PART:
                        sref = this.GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_INDEX_PART:
                        index = this.GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_DEF_VAL_PART:
                        defVal = (MibValue)this.GetValue(child, 0);
                        break;
                }
            }

            if (index is System.Collections.ArrayList)
            {
                type = new SnmpObjectType(
                    syntax,
                    units,
                    access,
                    status,
                    desc,
                    sref,
                    (index as ArrayList).OfType<SnmpIndex>().ToList(),
                    defVal);
            }
            else
            {
                type = new SnmpObjectType(
                    syntax,
                    units,
                    access,
                    status,
                    desc,
                    sref,
                    (MibValue)index,
                    defVal);
            }

            node.AddValue(type);
            return node;
        }

        /// <summary>Adds an SNMP notification type as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSnmpNotificationTypeMacroType(Production node)
        {
            System.Collections.ArrayList objects = new System.Collections.ArrayList();
            SnmpStatus status = null;
            string desc = null;
            string sref = null;
            Node child;

            this.currentMib.SmiVersion = 2;
            for (int i = 0; i < node.GetChildCount(); i++)
            {
                child = node.GetChildAt(i);
                switch ((Asn1Constants)child.GetId())
                {
                    case Asn1Constants.SNMP_OBJECTS_PART:
                        objects = (System.Collections.ArrayList)this.GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_STATUS_PART:
                        status = (SnmpStatus)this.GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_DESCR_PART:
                        desc = this.GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_REFER_PART:
                        sref = this.GetStringValue(child, 0);
                        break;
                }
            }

            node.AddValue(
                new SnmpNotificationType(
                    objects.OfType<MibValue>().ToList(),
                    status,
                    desc,
                    sref));
            return node;
        }
        
        /// <summary>Adds an SNMP trap type as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSnmpTrapTypeMacroType(Production node)
        {
            MibValue enterprise = null;
            System.Collections.ArrayList vars = new System.Collections.ArrayList();
            string desc = null;
            string sref = null;
            Node child;

            for (int i = 0; i < node.GetChildCount(); i++)
            {
                child = node.GetChildAt(i);
                switch ((Asn1Constants)child.GetId())
                {
                    case Asn1Constants.SNMP_ENTERPRISE_PART:
                        enterprise = (MibValue)this.GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_VAR_PART:
                        vars = child.Values;
                        break;
                    case Asn1Constants.SNMP_DESCR_PART:
                        desc = this.GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_REFER_PART:
                        sref = this.GetStringValue(child, 0);
                        break;
                }
            }

            node.AddValue(new SnmpTrapType(enterprise, vars.OfType<MibValue>().ToList(), desc, sref));
            return node;
        }
        
        /// <summary>Adds an SNMP textual convention as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSnmpTextualConventionMacroType(Production node)
        {
            string display = null;
            SnmpStatus status = null;
            string desc = null;
            string sref = null;
            MibType syntax = null;
            Node child;

            this.currentMib.SmiVersion = 2;
            for (int i = 0; i < node.GetChildCount(); i++)
            {
                child = node.GetChildAt(i);
                switch ((Asn1Constants)child.GetId())
                {
                    case Asn1Constants.SNMP_DISPLAY_PART:
                        display = this.GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_STATUS_PART:
                        status = (SnmpStatus)this.GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_DESCR_PART:
                        desc = this.GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_REFER_PART:
                        sref = this.GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_SYNTAX_PART:
                        syntax = (MibType)this.GetValue(child, 0);
                        syntax.Comment = MibAnalyzerUtil.GetComments(child);
                        break;
                }
            }

            node.AddValue(
                new SnmpTextualConvention(
                    display,
                    status,
                    desc,
                    sref,
                    syntax));
            return node;
        }

        /// <summary>Adds an SNMP object group as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSnmpObjectGroupMacroType(Production node)
        {
            System.Collections.ArrayList objects;
            SnmpStatus status;
            string desc;
            string sref;

            this.currentMib.SmiVersion = 2;
            objects = (System.Collections.ArrayList)this.GetValue(this.GetChildAt(node, 1), 0);
            status = (SnmpStatus)this.GetValue(this.GetChildAt(node, 2), 0);
            desc = this.GetStringValue(this.GetChildAt(node, 3), 0);

            if (node.GetChildCount() <= 4)
            {
                sref = null;
            }
            else
            {
                sref = this.GetStringValue(this.GetChildAt(node, 4), 0);
            }

            node.AddValue(
                new SnmpObjectGroup(
                    objects.OfType<MibValue>().ToList(),
                    status,
                    desc,
                    sref));
            return node;
        }
        
        /// <summary>Adds an SNMP notification group as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSnmpNotificationGroupMacroType(Production node)
        {
            System.Collections.ArrayList notifications;
            SnmpStatus status;
            string desc;
            string sref;

            this.currentMib.SmiVersion = 2;
            notifications = this.GetChildAt(node, 1).Values;
            status = (SnmpStatus)this.GetValue(this.GetChildAt(node, 2), 0);
            desc = this.GetStringValue(this.GetChildAt(node, 3), 0);
            if (node.GetChildCount() <= 4)
            {
                sref = null;
            }
            else
            {
                sref = this.GetStringValue(this.GetChildAt(node, 4), 0);
            }

            node.AddValue(
                new SnmpNotificationGroup(
                    notifications.OfType<MibValue>().ToList(),
                    status,
                    desc,
                    sref));
            return node;
        }

        /// <summary>Adds an SNMP module compliance type as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSnmpModuleComplianceMacroType(Production node)
        {
            SnmpStatus status = null;
            string desc = null;
            string sref = null;
            System.Collections.ArrayList modules = new System.Collections.ArrayList();
            Node child;

            this.currentMib.SmiVersion = 2;
            for (int i = 0; i < node.GetChildCount(); i++)
            {
                child = node.GetChildAt(i);
                switch ((Asn1Constants)child.GetId())
                {
                    case Asn1Constants.SNMP_STATUS_PART:
                        status = (SnmpStatus)this.GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_DESCR_PART:
                        desc = this.GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_REFER_PART:
                        sref = this.GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_MODULE_PART:
                        modules.Add(this.GetValue(child, 0));
                        break;
                }
            }

            node.AddValue(
                new SnmpModuleCompliance(
                    status,
                    desc,
                    sref,
                    modules.OfType<SnmpModule>().ToList()));
            return node;
        }

        /// <summary>Adds an SNMP agent capabilities as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSnmpAgentCapabilitiesMacroType(Production node)
        {
            string prod = null;
            SnmpStatus status = null;
            string desc = null;
            string sref = null;
            System.Collections.ArrayList modules = new System.Collections.ArrayList();
            Node child;

            this.currentMib.SmiVersion = 2;
            for (int i = 0; i < node.GetChildCount(); i++)
            {
                child = node.GetChildAt(i);
                switch ((Asn1Constants)child.GetId())
                {
                    case Asn1Constants.SNMP_PRODUCT_RELEASE_PART:
                        prod = this.GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_STATUS_PART:
                        status = (SnmpStatus)this.GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_DESCR_PART:
                        desc = this.GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_REFER_PART:
                        sref = this.GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_MODULE_SUPPORT_PART:
                        modules.Add(this.GetValue(child, 0));
                        break;
                }
            }

            node.AddValue(
                new SnmpAgentCapabilities(
                    prod,
                    status,
                    desc,
                    sref,
                    modules.OfType<SnmpModuleSupport>().ToList()));
            return node;
        }
        
        /// <summary>Adds the last update string as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSnmpUpdatePart(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }
        
        /// <summary>Adds the organization name as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSnmpOrganizationPart(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }
        
        /// <summary>Adds the organization contact info as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSnmpContactPart(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }
        
        /// <summary>Adds the description string as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSnmpDescrPart(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }
        
        /// <summary>Adds an SNMP revision as the node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSnmpRevisionPart(Production node)
        {
            SnmpRevision rev;
            MibValue value;
            string desc;

            value = (MibValue)this.GetValue(this.GetChildAt(node, 1), 0);
            desc = this.GetStringValue(this.GetChildAt(node, 3), 0);
            rev = new SnmpRevision(value, desc);
            rev.setComment(MibAnalyzerUtil.GetComments(node));
            node.AddValue(rev);
            return node;
        }
        
        /// <summary>Adds an SNMP status as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSnmpStatusPart(Production node)
        {
            Node child;
            string name;

            child = this.GetChildAt(node, 1);
            name = this.GetStringValue(child, 0);
            if (name.Equals("mandatory"))
            {
                node.AddValue(SnmpStatus.MANDATORY);
            }
            else if (name.Equals("optional"))
            {
                node.AddValue(SnmpStatus.OPTIONAL);
            }
            else if (name.Equals("current"))
            {
                node.AddValue(SnmpStatus.CURRENT);
            }
            else if (name.Equals("deprecated"))
            {
                node.AddValue(SnmpStatus.DEPRECATED);
            }
            else if (name.Equals("obsolete"))
            {
                node.AddValue(SnmpStatus.OBSOLETE);
            }
            else
            {
                node.AddValue(SnmpStatus.CURRENT);
                throw new ParseException(
                    ParseException.ErrorType.ANALYSIS,
                    "unrecognized status value: '" + name + "'",
                    child.GetStartLine(),
                    child.GetStartColumn());
            }

            return node;
        }
        
        /// <summary>Adds the reference string as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSnmpReferPart(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>Adds a MIB type as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSnmpSyntaxPart(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }
        
        /// <summary>Adds the units string as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSnmpUnitsPart(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>Adds the SNMP access as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSnmpAccessPart(Production node)
        {
            Node child;
            string name;

            child = this.GetChildAt(node, 0);
            if (child.GetId() != (int)Asn1Constants.ACCESS)
            {
                this.currentMib.SmiVersion = 2;
            }

            child = this.GetChildAt(node, 1);
            name = this.GetStringValue(child, 0);
            if (name.Equals("read-only"))
            {
                node.AddValue(SnmpAccess.READ_ONLY);
            }
            else if (name.Equals("read-write"))
            {
                node.AddValue(SnmpAccess.READ_WRITE);
            }
            else if (name.Equals("read-create"))
            {
                node.AddValue(SnmpAccess.READ_CREATE);
            }
            else if (name.Equals("write-only"))
            {
                node.AddValue(SnmpAccess.WRITE_ONLY);
            }
            else if (name.Equals("not-implemented"))
            {
                node.AddValue(SnmpAccess.NOT_IMPLEMENTED);
            }
            else if (name.Equals("not-accessible"))
            {
                node.AddValue(SnmpAccess.NOT_ACCESSIBLE);
            }
            else if (name.Equals("accessible-for-notify"))
            {
                node.AddValue(SnmpAccess.ACCESSIBLE_FOR_NOTIFY);
            }
            else
            {
                node.AddValue(SnmpAccess.READ_WRITE);
                throw new ParseException(
                    ParseException.ErrorType.ANALYSIS,
                    "unrecognized access value: '" + name + "'",
                    child.GetStartLine(),
                    child.GetStartColumn());
            }

            return node;
        }
        
        /// <summary>
        /// Adds either a list of value or a single value as a node value.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSnmpIndexPart(Production node)
        {
            if (this.GetChildAt(node, 0).GetId() == (int)Asn1Constants.INDEX)
            {
                node.AddValue(this.GetChildValues(node));
            }
            else
            {
                node.AddValues(this.GetChildValues(node));
            }

            return node;
        }
        
        /// <summary>Adds the index MIB values as node values.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitIndexValueList(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>Adds the index MIB value as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitIndexValue(Production node)
        {
            SnmpIndex index;
            object obj = this.GetChildValues(node)[0];

            switch ((Asn1Constants)node.GetChildAt(0).GetId())
            {
                case Asn1Constants.VALUE:
                    index = new SnmpIndex(false, (MibValue)obj, null);
                    break;
                case Asn1Constants.IMPLIED:
                    index = new SnmpIndex(true, (MibValue)obj, null);
                    break;
                default:
                    index = new SnmpIndex(false, null, (MibType)obj);
                    break;
            }

            node.AddValue(index);
            return node;
        }

        /// <summary>Adds the index MIB type as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitIndexType(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>Adds the default MIB value as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSnmpDefValPart(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }
        
        /// <summary>Adds a list of MIB values as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSnmpObjectsPart(Production node)
        {
            node.AddValue(this.GetChildValues(node));
            return node;
        }
        
        /// <summary>Adds the MIB values as node values.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitValueList(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>Adds the enterprise MIB value as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSnmpEnterprisePart(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }
        
        /// <summary>Adds the variable MIB values as node values.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSnmpVarPart(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }
        
        /// <summary>Adds the display hint as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSnmpDisplayPart(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }
        
        /// <summary>Adds the MIB values as node values.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSnmpNotificationsPart(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }
        
        /// <summary>Adds an SNMP module as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSnmpModulePart(Production node)
        {
            SnmpModule module;
            string name = null;
            System.Collections.ArrayList groups = new System.Collections.ArrayList();
            System.Collections.ArrayList modules = new System.Collections.ArrayList();
            string comment = null;
            Node child;

            for (int i = 0; i < node.GetChildCount(); i++)
            {
                child = node.GetChildAt(i);
                switch ((Asn1Constants)child.GetId())
                {
                    case Asn1Constants.MODULE:
                        comment = MibAnalyzerUtil.GetComments(child);
                        break;
                    case Asn1Constants.SNMP_MODULE_IMPORT:
                        name = this.GetStringValue(child, 0);
                        this.PopContext();
                        break;
                    case Asn1Constants.SNMP_MANDATORY_PART:
                        groups = (System.Collections.ArrayList)this.GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_COMPLIANCE_PART:
                        modules.Add(this.GetValue(child, 0));
                        break;
                }
            }

            module = new SnmpModule(name, groups.OfType<MibValue>().ToList(), modules.OfType<SnmpCompliance>().ToList());
            module.Comment = comment;
            node.AddValue(module);
            return node;
        }
        
        /// <summary>
        /// Adds the module name as a node value. This method also sets
        /// current MIB context to the referenced module. The imports are
        /// implicit, meaning that symbol names do not have to be listed
        /// in order to be imported.
        /// </summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSnmpModuleImport(Production node)
        {
            MibImport imp;
            string module;

            // Load referenced module
            module = this.GetStringValue(this.GetChildAt(node, 0), 0);
            this.loader.scheduleLoad(module);

            // Create module reference and context
            imp = new MibImport(this.loader, this.GetLocation(node), module, null);
            this.currentMib.AddImport(imp);
            this.PushContextExtension(imp);

            // Return results
            node.AddValue(module);
            return node;
        }
        
        /// <summary>Adds the list of group values as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSnmpMandatoryPart(Production node)
        {
            node.AddValue(this.GetChildValues(node));
            return node;
        }
        
        /// <summary>Adds an SNMP compliance object as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSnmpCompliancePart(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }
        
        /// <summary>Adds an SNMP compliance object as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitComplianceGroup(Production node)
        {
            SnmpCompliance comp;
            MibValue value;
            string desc;

            value = (MibValue)this.GetValue(this.GetChildAt(node, 1), 0);
            desc = this.GetStringValue(this.GetChildAt(node, 2), 0);
            comp = new SnmpCompliance(true, value, null, null, null, desc);
            comp.setComment(MibAnalyzerUtil.GetComments(node));
            node.AddValue(comp);
            return node;
        }
        
        /// <summary>Adds an SNMP compliance object as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitComplianceObject(Production node)
        {
            SnmpCompliance comp;
            MibValue value = null;
            MibType syntax = null;
            MibType write = null;
            SnmpAccess access = null;
            string desc = null;
            Node child;

            for (int i = 0; i < node.GetChildCount(); i++)
            {
                child = node.GetChildAt(i);
                switch ((Asn1Constants)child.GetId())
                {
                    case Asn1Constants.VALUE:
                        value = (MibValue)this.GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_SYNTAX_PART:
                        syntax = (MibType)this.GetValue(child, 0);
                        syntax.Comment = MibAnalyzerUtil.GetComments(child);
                        break;
                    case Asn1Constants.SNMP_WRITE_SYNTAX_PART:
                        write = (MibType)this.GetValue(child, 0);
                        write.Comment = MibAnalyzerUtil.GetComments(child);
                        break;
                    case Asn1Constants.SNMP_ACCESS_PART:
                        access = (SnmpAccess)this.GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_DESCR_PART:
                        desc = this.GetStringValue(child, 0);
                        break;
                }
            }

            comp = new SnmpCompliance(false, value, syntax, write, access, desc);
            comp.setComment(MibAnalyzerUtil.GetComments(node));
            node.AddValue(comp);
            return node;
        }
        
        /// <summary>Adds the MIB type as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSnmpWriteSyntaxPart(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }
        
        /// <summary>Adds the product release string as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSnmpProductReleasePart(Production node)
        {
            node.AddValues(this.GetChildValues(node));
            return node;
        }

        /// <summary>Adds an SNMP module support as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSnmpModuleSupportPart(Production node)
        {
            string module = null;
            System.Collections.ArrayList groups = null;
            System.Collections.ArrayList vars = new System.Collections.ArrayList();
            Node child;

            for (int i = 0; i < node.GetChildCount(); i++)
            {
                child = node.GetChildAt(i);
                switch ((Asn1Constants)child.GetId())
                {
                    case Asn1Constants.SNMP_MODULE_IMPORT:
                        module = this.GetStringValue(child, 0);
                        this.PopContext();
                        break;
                    case Asn1Constants.VALUE_LIST:
                        groups = child.Values;
                        break;
                    case Asn1Constants.SNMP_VARIATION_PART:
                        vars.Add(this.GetValue(child, 0));
                        break;
                }
            }

            node.Values.Add(new SnmpModuleSupport(module, groups.OfType<MibValue>().ToList(), vars.OfType<SnmpVariation>().ToList()));
            return node;
        }

        /// <summary>
        /// Modifies the MIB context stack to make sure all references are
        /// interpreted in the context of the symbol being modified.
        /// </summary>
        /// <param name="node">The parent node</param>
        /// <param name="child">The child node or null</param>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override void ChildSnmpVariationPart(Production node, Node child)
        {
            MibType type;
            IMibContext context;

            if (child.GetId() == (int)Asn1Constants.VALUE)
            {
                context = new MibTypeContext(this.GetValue(child, 0));
                this.PushContextExtension(context);
            }
            else if (child.GetId() == (int)Asn1Constants.SNMP_SYNTAX_PART)
            {
                type = (MibType)this.GetValue(child, 0);
                if (type is IMibContext)
                {
                    this.PushContextExtension((IMibContext)type);
                }
            }

            node.AddChild(child);
        }
        
        /// <summary>Adds an SNMP variation as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        /// <exception cref="ParseException"> if the node analysis discovered errors</exception>
        public override Node ExitSnmpVariationPart(Production node)
        {
            MibValue value = null;
            MibType syntax = null;
            MibType write = null;
            SnmpAccess access = null;
            ArrayList reqs = new ArrayList();
            MibValue defVal = null;
            string desc = null;
            Node child;

            for (int i = 0; i < node.GetChildCount(); i++)
            {
                child = node.GetChildAt(i);
                switch ((Asn1Constants)child.GetId())
                {
                    case Asn1Constants.VALUE:
                        value = (MibValue)this.GetValue(child, 0);
                        this.PopContext();
                        break;
                    case Asn1Constants.SNMP_SYNTAX_PART:
                        syntax = (MibType)this.GetValue(child, 0);
                        if (syntax is IMibContext)
                        {
                            this.PopContext();
                        }

                        syntax.Comment = MibAnalyzerUtil.GetComments(child);
                        break;
                    case Asn1Constants.SNMP_WRITE_SYNTAX_PART:
                        write = (MibType)this.GetValue(child, 0);
                        write.Comment = MibAnalyzerUtil.GetComments(child);
                        break;
                    case Asn1Constants.SNMP_ACCESS_PART:
                        access = (SnmpAccess)this.GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_CREATION_PART:
                        reqs = (ArrayList)this.GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_DEF_VAL_PART:
                        defVal = (MibValue)this.GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_DESCR_PART:
                        desc = this.GetStringValue(child, 0);
                        break;
                }
            }

            node.Values.Add(
                new SnmpVariation(
                    value,
                    syntax,
                    write,
                    access,
                    reqs.OfType<MibValue>().ToList(),
                    defVal,
                    desc));
            return node;
        }
        
        /// <summary>Adds a list of the MIB values as a node value.</summary>
        /// <param name="node">The node being exited</param>  
        /// <returns>The node to add to the parse tree</returns>
        public override Node ExitSnmpCreationPart(Production node)
        {
            node.Values.Add(this.GetChildValues(node));
            return node;
        }

        /// <summary>Returns the location of a specified node.</summary>
        /// <param name="node">The parse tree node</param>
        /// <returns>The file location of the node</returns>
        private FileLocation GetLocation(Node node)
        {
            return new FileLocation(
                this.file,
                node.GetStartLine(),
                node.GetStartColumn());
        }

        /// <summary>Adds a new context to the top of the context stack.
        /// </summary>
        /// <param name="context">The context to add</param>
        private void PushContext(IMibContext context)
        {
            this.contextStack.Add(context);
        }

        /// <summary>Adds an extension to the current context to the top of the
        /// context stack. A new compound context will be created by
        /// appending the top context to the specified one.
        /// </summary>
        /// <param name="context">The context extension to add</param>
        private void PushContextExtension(IMibContext context)
        {
            this.PushContext(new CompoundContext(context, this.TopContext));
        }

        /// <summary>
        /// Removes the top context on the context stack.
        /// </summary>
        private void PopContext()
        {
            this.contextStack.RemoveAt(this.contextStack.Count - 1);
        }
    }
}
