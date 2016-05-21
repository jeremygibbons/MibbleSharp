//
// MibAnalyzer.cs
// 
// This work is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published
// by the Free Software Foundation; either version 2 of the License,
// or (at your option) any later version.
//
// This work is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307
// USA
// 
// Original Java code Copyright (c) 2004-2016 Per Cederberg. All
// rights reserved.
// C# conversion Copyright (c) 2016 Jeremy Gibbons. All rights reserved
//

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Numerics;
using PerCederberg.Grammatica.Runtime;
using MibbleSharp.Asn1;
using MibbleSharp.Type;
using MibbleSharp.Value;
using MibbleSharp.Snmp;

namespace MibbleSharp
{

    /**
     * A MIB file analyzer. This class analyzes the MIB file parse tree,
     * and creates appropriate MIB modules with the right symbols. This
     * analyzer handles imports by adding them to the MIB loader queue.
     * As the imported MIB symbols aren't available during the analysis,
     * type and value references will be created whenever an identifier
     * is encountered.
     *
     * @author   Per Cederberg, <per at percederberg dot net>
     * @version  2.9
     * @since    2.0
     */
    class MibAnalyzer : Asn1Analyzer
    {

        /**
         * The list of MIB modules found.
         */
        private IList<Mib> mibs = new List<Mib>();

        /**
         * The MIB file being analyzed.
         */
        private string file;

        /**
         * The MIB loader using this analyzer.
         */
        private MibLoader loader;

        /**
         * The MIB loader log.
         */
        private MibLoaderLog log;

        /**
         * The current MIB module being analyzed.
         */
        private Mib currentMib = null;

        /**
         * The base MIB symbol context. This context will be extended
         * when parsing the import list.
         */
        private IMibContext baseContext = null;

        /**
         * The MIB context stack. This stack is modified during the
         * parsing to add type or import contexts as necessary. The top
         * context on the stack is returned by the getContext() method.
         *
         * @see #getContext()
         */
        private IList<IMibContext> contextStack = new List<IMibContext>();

        /**
         * The implicit tags flag.
         */
        private bool implicitTags = true;

        /**
         * Creates a new MIB file analyzer.
         *
         * @param file           the MIB file being analyzed
         * @param loader         the MIB loader using this analyzer
         * @param log            the MIB loader log to use
         */
        public MibAnalyzer(string file, MibLoader loader, MibLoaderLog log)
        {
            this.file = file;
            this.loader = loader;
            this.log = log;
        }

        /**
         * Resets this analyzer. This method is mostly used to release
         * all references to parsed data.
         */
        public override void Reset()
        {
            mibs = new List<Mib>();
            currentMib = null;
            baseContext = null;
            contextStack.Clear();
            implicitTags = true;
        }

        /**
         * Returns the list of MIB modules found during analysis.
         *  
         * @return a list of MIB modules
         */
        public IEnumerable<Mib> getMibs()
        {
            return mibs;
        }

        /**
         * Adds the binary number as a node value. This method will
         * convert the binary string to either an Integer, a Long, or a
         * BigInteger.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitBinaryString(Token node)
        {
            string str = node.GetImage();

            str = str.Substring(1, str.Length - 2);
            long value = Convert.ToInt64(str, 2);

            node.Values.Add((BigInteger) value);
            node.Values.Add(str);
            return node;
        }

        /**
         * Adds the hexadecimal number as a node value. This method will
         * convert the hexadecimal string to either an Integer, a Long,
         * or a BigInteger.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitHexadecimalString(Token node)
        {
            string str = node.GetImage();

            str = str.Substring(1, str.Length - 2);
            long value = Convert.ToInt64(str, 16);
            node.AddValue((BigInteger) value);
            node.AddValue(str);
            return node;
        }

        /**
         * Adds the quoted string as a node value. This method will
         * remove the quotation marks and replace any double marks inside
         * the string with a single mark.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
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
            } while (pos >= 0);
            node.AddValue(str);
            return node;
        }

        /**
         * Adds the identifier string as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitIdentifierString(Token node)
        {
            node.AddValue(node.GetImage());
            return node;
        }

        /**
         * Adds the number as a node value. This method will convert the
         * number string to either an Integer, a Long, or a BigInteger.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitNumberString(Token node)
        {
            string str = node.GetImage();
            BigInteger value = BigInteger.Parse(str);
            node.AddValue(value);
            return node;
        }

        /**
         * Stores any MIB tail comments if available.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitStart(Production node)
        {
            string comment = MibAnalyzerUtil.GetCommentsFooter(node);

            if (currentMib != null)
            {
                currentMib.FooterComment = comment;
            }
            return null;
        }

        /**
         * Creates the current MIB module container and the base context.
         *
         * @param node           the node being entered
         */
        public override void EnterModuleDefinition(Production node)
        {
            currentMib = new Mib(file, loader, log);
            baseContext = loader.getDefaultContext();
            baseContext = new CompoundContext(currentMib, baseContext);
            pushContext(baseContext);
        }

        /**
         * Sets the MIB name to the module identifier string value. Also
         * removes this node from the parse tree.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitModuleDefinition(Production node)
        {

            currentMib.Name = GetStringValue(GetChildAt(node, 0), 0);
            currentMib.HeaderComment = MibAnalyzerUtil.GetComments(node);
            mibs.Add(currentMib);
            return node;
        }

        /**
         * Adds the module identifier string as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitModuleIdentifier(Production node)
        {

            node.AddValue(GetStringValue(GetChildAt(node, 0), 0));
            return node;
        }

        /**
         * Adds the module identifier string as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitModuleReference(Production node)
        {

            node.AddValue(GetStringValue(GetChildAt(node, 0), 0));
            return node;
        }

        /**
         * Sets the implicit tags flag.
         *
         * @param node           the node being exited
         *
         * @return null to remove the node from the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitTagDefault(Production node)
        {

            Node child;

            child = GetChildAt(node, 0);
            if (child.GetId() == (int)Asn1Constants.EXPLICIT)
            {
                implicitTags = false;
            }
            else
            {
                implicitTags = true;
            }
            return null;
        }

        /**
         * Adds all imported MIB files to the MIB context. Also removes
         * this node from the parse tree.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitImportList(Production node)
        {
            ArrayList imports = GetChildValues(node);
            MibImport imp;
            IMibContext current = loader.getDefaultContext();
            bool addMissingSmi = true;

            for (int i = 0; i < imports.Count; i++)
            {
                imp = (MibImport)imports[i];
                if (imp.Name.StartsWith("RFC1065-SMI") ||
                    imp.Name.StartsWith("RFC1155-SMI") ||
                    imp.Name.StartsWith("SNMPv2-SMI"))
                {

                    addMissingSmi = false;
                }
            }
            if (addMissingSmi)
            {
                // TODO: Ugly hack that adds a "hidden" SNMPv1 SMI as the last
                //       import, but without any named symbols (triggering
                //       warnings for each symbol used).
                imp = new MibImport(loader, getLocation(node), "RFC1155-SMI", new List<MibSymbol>());
                loader.scheduleLoad(imp.Name);
                currentMib.AddImport(imp);
                imports.Add(imp);
            }
            for (int i = imports.Count - 1; i >= 0; i--)
            {
                imp = (MibImport)imports[i];
                current = new CompoundContext(imp, current);
            }
            baseContext = new CompoundContext(currentMib, current);
            popContext();
            pushContext(baseContext);
            return null;
        }

        /**
         * Schedules the imported MIB file for loading. Also adds a MIB
         * reference as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitSymbolsFromModule(Production node)
        {

            MibImport imp;
            string module;
            System.Collections.ArrayList symbols;
            Node child;

            // Create MIB reference
            child = GetChildAt(node, 0);
            symbols = child.Values;
            if (symbols == null)
            {
                symbols = new ArrayList();
            }
            child = GetChildAt(node, 2);
            module = GetStringValue(child, 0);
            imp = new MibImport(loader, getLocation(child), module, new List<MibSymbol>());

            // Schedule MIB loading
            loader.scheduleLoad(module);

            // Add reference to MIB and node
            currentMib.AddImport(imp);
            node.AddValue(imp);
            return node;
        }

        /**
         * Adds all symbol identifiers as node values.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSymbolList(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds the identifier string as a node value. If the symbol name
         * is not an identifier, no node value will be added.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSymbol(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Creates a macro symbol and adds it to the MIB. Also removes
         * this node from the parse tree.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitMacroDefinition(Production node)
        {

            string name;
            MibMacroSymbol symbol;

            // Check macro name
            name = GetStringValue(GetChildAt(node, 0), 0);
            if (currentMib.GetSymbol(name) != null)
            {
                throw new ParseException(
                    ParseException.ErrorType.ANALYSIS,
                    "a symbol '" + name + "' already present in the MIB",
                    node.GetStartLine(),
                    node.GetStartColumn());
            }

            // Create macro symbol
            symbol = new MibMacroSymbol(getLocation(node),
                                            currentMib,
                                            name);
            symbol.Comment = MibAnalyzerUtil.GetComments(node);

            return null;
        }

        /**
         * Adds the macro name as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitMacroReference(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Creates a type symbol and adds it to the MIB. Also removes
         * this node from the parse tree.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitTypeAssignment(Production node)
        {

            string name;
            MibType type;
            MibTypeSymbol symbol;

            // Check type name
            name = GetStringValue(GetChildAt(node, 0), 0);
            if (currentMib.GetSymbol(name) != null)
            {
                throw new ParseException(
                    ParseException.ErrorType.ANALYSIS,
                    "a symbol '" + name + "' already present in the MIB",
                    node.GetStartLine(),
                    node.GetStartColumn());
            }
            if (!Char.IsUpper(name[0]))
            {
                log.AddWarning(getLocation(node),
                               "type identifier '" + name + "' doesn't " +
                               "start with an uppercase character");
            }

            // Create type symbol
            type = (MibType)GetValue(GetChildAt(node, 2), 0);
            symbol = new MibTypeSymbol(getLocation(node),
                                           currentMib,
                                           name,
                                           type);
            symbol.Comment = MibAnalyzerUtil.GetComments(node);

            return null;
        }

        /**
         * Adds a MIB type as the node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitType(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds a type reference as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitDefinedType(Production node)
        {

            IMibContext local = getContext();
            string name = null;
            Object value = null;
            FileLocation loc = getLocation(node);
            Node child;

            for (int i = 0; i < node.GetChildCount(); i++)
            {
                child = node.GetChildAt(i);
                switch ((Asn1Constants)child.GetId())
                {
                    case Asn1Constants.MODULE_REFERENCE:
                        name = GetStringValue(child, 0);
                        local = currentMib.GetImport(name);
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
                        name = GetStringValue(child, 0);
                        break;
                    case Asn1Constants.VALUE_OR_CONSTRAINT_LIST:
                        value = GetValue(child, 0);
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

        /**
         * Adds a MIB type as the node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitBuiltinType(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds a null type as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitNullType(Production node)
        {
            node.AddValue(new NullType());
            return node;
        }

        /**
         * Adds a boolean type as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitBooleanType(Production node)
        {
            node.AddValue(new BooleanType());
            return node;
        }

        /**
         * Adds a real type as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitRealType(Production node)
        {
            node.AddValue(new RealType());
            return node;
        }

        /**
         * Adds an integer type as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitIntegerType(Production node)
        {
            IntegerType type;
            ArrayList values;
            Object obj;

            values = GetChildValues(node);
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

        /**
         * Adds an object identifier type as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitObjectIdentifierType(Production node)
        {
            node.AddValue(new ObjectIdentifierType());
            return node;
        }

        /**
         * Adds a string type as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitStringType(Production node)
        {
            StringType type;
            System.Collections.ArrayList values;

            values = GetChildValues(node);
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

        /**
         * Adds a bit set type as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitBitStringType(Production node)
        {
            BitSetType type;
            System.Collections.ArrayList values;
            Object obj;

            values = GetChildValues(node);
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

        /**
         * Adds a bit set type as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitBitsType(Production node)
        {
            return ExitBitStringType(node);
        }

        /**
         * Adds a MIB sequence type as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSequenceType(Production node)
        {
            System.Collections.ArrayList elements = GetChildValues(node);

            node.AddValue(new SequenceType(elements.OfType<ElementType>().ToList()));
            return node;
        }

        /**
         * Adds a sequence of MIB type as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitSequenceOfType(Production node)
        {

            MibType type;
            Constraint c = null;
            Node child;

            child = GetChildAt(node, node.GetChildCount() - 1);
            type = (MibType)GetValue(child, 0);
            if (node.GetChildCount() == 4)
            {
                child = GetChildAt(node, 1);
                c = (Constraint)GetValue(child, 0);
            }
            node.AddValue(new SequenceOfType(type, c));
            return node;
        }

        /**
         * Adds a null type as a node value. This method also prints an
         * error about this construct being unsupported.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSetType(Production node)
        {
            // TODO: implement set type support
            log.AddError(getLocation(node),
                         "SET type currently unsupported");
            node.AddValue(new NullType());
            return node;
        }

        /**
         * Adds a null type as a node value. This method also prints an
         * error about this construct being unsupported.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSetOfType(Production node)
        {
            // TODO: implement set of type support
            log.AddError(getLocation(node),
                         "SET OF type currently unsupported");
            node.AddValue(new NullType());
            return node;
        }

        /**
         * Adds a MIB choice type as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitChoiceType(Production node)
        {
            ArrayList al = GetChildValues(node);
            node.AddValue(new ChoiceType(al.OfType<ElementType>().ToList()));
            return node;
        }

        /**
         * Adds a null type as a node value. This method also prints an
         * error about this construct being unsupported.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitEnumeratedType(Production node)
        {
            // TODO: implement enumerated type support
            log.AddError(getLocation(node),
                         "ENUMERATED type currently unsupported");
            node.AddValue(new NullType());
            return node;
        }

        /**
         * Adds a null type as a node value. This method also prints an
         * error about this construct being unsupported.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSelectionType(Production node)
        {
            // TODO: implement selection type support
            log.AddError(getLocation(node),
                         "selection type currently unsupported");
            node.AddValue(new NullType());
            return node;
        }

        /**
         * Adds the tagged type as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitTaggedType(Production node)
        {

            MibType type;
            MibTypeTag tag;
            bool implicitly = implicitTags;

            Node child;


            child = GetChildAt(node, 0);

            tag = (MibTypeTag)GetValue(child, 0);

            child = GetChildAt(node, 1);
            if (child.GetId() == (int)Asn1Constants.EXPLICIT_OR_IMPLICIT_TAG)
            {
                implicitly = ((Boolean)GetValue(child, 0));
            }
            child = GetChildAt(node, node.GetChildCount() - 1);
            type = (MibType)GetValue(child, 0);
            type.SetTag(implicitly, tag);
            node.AddValue(type);
            return node;
        }

        /**
         * Called when exiting a parse tree node.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree, or
         *         null if no parse tree should be created
         */
        public override Node ExitTag(Production node)
        {
            System.Collections.ArrayList values = GetChildValues(node);
            int category = MibTypeTag.ContextSpecificCategory;
            int value;

            if (values.Count == 1)
            {
                value = (int)((BigInteger)values[0]);
            }
            else
            {
                category = ((int)values[0]);
                value = (int)((BigInteger)values[1]);
            }
            node.AddValue(new MibTypeTag(category, value));
            return node;
        }

        /**
         * Adds the type tag category value as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
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

        /**
         * Adds the implicit boolean flag as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
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

        /**
         * Adds a null type as a node value. This method also prints an
         * error about this construct being unsupported.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitAnyType(Production node)
        {
            // TODO: implement any type support
            log.AddError(getLocation(node),
                         "ANY type currently unsupported");
            node.AddValue(new NullType());
            return node;
        }

        /**
         * Adds all element types as a node values.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitElementTypeList(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds an element type as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitElementType(Production node)
        {

            string name = null;
            MibType type;
            Node child;

            child = GetChildAt(node, 0);
            if (child.GetId() == (int)Asn1Constants.IDENTIFIER_STRING)
            {
                name = GetStringValue(child, 0);
                child = GetChildAt(node, 1);
            }
            if (child.GetId() != (int)Asn1Constants.TYPE)
            {
                throw new ParseException(
                    ParseException.ErrorType.ANALYSIS,
                    "referencing components is currently unsupported",
                    child.GetStartLine(),
                    child.GetStartColumn());
            }
            type = new ElementType(name, (MibType)GetValue(child, 0));
            type.Comment = MibAnalyzerUtil.GetComments(node);
            node.AddValue(type);
            return node;
        }

        /**
         * Prints an error about this construct being unsupported.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitOptionalOrDefaultElement(Production node)
        {
            // TODO: implement this method?
            log.AddError(getLocation(node),
                         "optional and default elements are currently " +
                         "unsupported");
            return null;
        }

        /**
         * Adds an array list with symbols or a constraint as the node
         * value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitValueOrConstraintList(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds an array list with symbols as the node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
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
            node.AddValue(GetChildValues(node));
            return node;
        }

        /**
         * Adds a value symbol as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitNamedNumber(Production node)
        {

            MibValueSymbol symbol;
            string name;
            MibValue value;

            name = GetStringValue(GetChildAt(node, 0), 0);
            value = (MibValue)GetValue(GetChildAt(node, 2), 0);
            symbol = new MibValueSymbol(getLocation(node),
                                            null,
                                            name,
                                            null,
                                            value);
            node.AddValue(symbol);
            return node;
        }

        /**
         * Adds a MIB value as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitNumber(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds a MIB type constraint as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitConstraintList(Production node)
        {
            Constraint result = null;
            System.Collections.ArrayList values;
            Constraint c;

            values = GetChildValues(node);
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

        /**
         * Adds a MIB type constraint as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitConstraint(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds a MIB type constraint as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitValueConstraintList(Production node)
        {
            return ExitConstraintList(node);
        }

        /**
         * Adds a MIB type value or value range constraint as a node
         * value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitValueConstraint(Production node)
        {

            System.Collections.ArrayList list = GetChildValues(node);
            NumberValue lower = null;
            NumberValue upper = null;
            bool strictLower = false;
            bool strictUpper = false;
            Object obj;

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
                obj = new ValueConstraint(getLocation(node), lower);
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    obj = list[i];
                    if (obj is Boolean && strictLower == false)
                    {
                        strictLower = (Boolean)obj;
                    }
                    else if (obj is Boolean)
                    {
                        strictUpper = (Boolean)obj;
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
                obj = new ValueRangeConstraint(getLocation(node),
                                                       lower,
                                                       strictLower,
                                                       upper,
                                                       strictUpper);
            }
            node.AddValue(obj);
            return node;
        }

        /**
         * Adds the upper end point and strict inequality flags as node
         * values.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitValueRange(Production node)
        {

            Node child;

            // Check for strict lower end point
            child = GetChildAt(node, 0);
            if (child.GetId() == (int)Asn1Constants.LESS_THAN)
            {
                node.AddValue(true);
            }
            else
            {
                node.AddValue(false);
            }

            // Add upper end point (or null)
            child = GetChildAt(node, node.GetChildCount() - 1);
            node.AddValue(child.GetValue(0));

            // Check for strict upper end point
            child = GetChildAt(node, node.GetChildCount() - 2);
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

        /**
         * Adds a MIB value or null as a node value. The null value is
         * used to represent a minimum value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitLowerEndPoint(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds a MIB value or null as a node value. The null value is
         * used to represent a maximum value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitUpperEndPoint(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds a MIB type size constraint as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitSizeConstraint(Production node)
        {

            Constraint c;

            c = (Constraint)GetValue(GetChildAt(node, 1), 0);
            node.AddValue(new SizeConstraint(getLocation(node), c));
            return node;
        }

        /**
         * Removes this node from the parse tree, and prints an error
         * about this construct being unsupported.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitAlphabetConstraint(Production node)
        {
            // TODO: implement alphabet constraints
            log.AddError(getLocation(node),
                         "FROM constraints are currently unsupported");
            return null;
        }

        /**
         * Removes this node from the parse tree, and prints an error
         * about this construct being unsupported.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitContainedTypeConstraint(Production node)
        {
            // TODO: implement contained type constraints
            log.AddError(getLocation(node),
                         "INCLUDES constraints are currently unsupported");
            return null;
        }

        /**
         * Removes this node from the parse tree, and prints an error
         * about this construct being unsupported.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitInnerTypeConstraint(Production node)
        {
            // TODO: implement inner type constraints
            log.AddError(getLocation(node),
                         "WITH COMPONENT(S) constraints are currently " +
                         "unsupported");
            return null;
        }

        /**
         * Creates a value symbol and adds it to the MIB. Also removes
         * this node from the parse tree.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitValueAssignment(Production node)
        {

            string name;
            MibType type;
            MibValue value;
            MibValueSymbol symbol;

            // Check value name
            name = GetStringValue(GetChildAt(node, 0), 0);
            if (currentMib.GetSymbol(name) != null)
            {
                throw new ParseException(
                    ParseException.ErrorType.ANALYSIS,
                    "a symbol '" + name + "' already present in the MIB",
                    node.GetStartLine(),
                    node.GetStartColumn());
            }
            if (!Char.IsLower(name[0]))
            {
                log.AddWarning(getLocation(node),
                               "value identifier '" + name + "' doesn't " +
                               "start with a lowercase character");
            }

            // Create value symbol
            type = (MibType)GetValue(GetChildAt(node, 1), 0);
            value = (MibValue)GetValue(GetChildAt(node, 3), 0);
            symbol = new MibValueSymbol(getLocation(node),
                                            currentMib,
                                            name,
                                            type,
                                            value);
            symbol.Comment = MibAnalyzerUtil.GetComments(node);

            return null;
        }

        /**
         * Adds a MIB value as the node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitValue(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds a value reference as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitDefinedValue(Production node)
        {

            ValueReference vref;
            IMibContext local = getContext();
            string name;
            Node child;

            // Check for module reference
            child = GetChildAt(node, 0);
            if (child.GetId() == (int)Asn1Constants.MODULE_REFERENCE)
            {
                name = GetStringValue(child, 0);
                local = currentMib.GetImport(name);
                if (local == null)
                {
                    throw new ParseException(
                        ParseException.ErrorType.ANALYSIS,
                        "referenced module not imported '" + name + "'",
                        child.GetStartLine(),
                        child.GetStartColumn());
                }
                child = GetChildAt(node, 1);
            }

            // Create value reference
            name = GetStringValue(child, 0);
            vref = new ValueReference(getLocation(node), local, name);
            node.AddValue(vref);
            return node;
        }

        /**
         * Adds a MIB value as the node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitBuiltinValue(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds a MIB null value as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitNullValue(Production node)
        {
            node.AddValue(NullValue.NULL);
            return node;
        }

        /**
         * Adds a MIB boolean value as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
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

        /**
         * Adds a MIB number value as the node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitSpecialRealValue(Production node)
        {

            double d;

            if (GetChildAt(node, 0).GetId() == (int)Asn1Constants.PLUS_INFINITY)
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

        /**
         * Adds a MIB number value as the node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitNumberValue(Production node)
        {

            BigInteger number;

            if (GetChildAt(node, 0).GetId() == (int)Asn1Constants.MINUS)
            {
                number = (BigInteger) GetValue(GetChildAt(node, 1), 0);
                number = -number;
            }
            else
            {
                number = (BigInteger) GetValue(GetChildAt(node, 0), 0);
            }
            node.AddValue(new NumberValue(number));
            return node;
        }

        /**
         * Adds a MIB number value as the node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitBinaryValue(Production node)
        {

            Node child;
            BigInteger number;
            string text;

            child = GetChildAt(node, 0);
            number = (BigInteger) child.GetValue(0);
            text = (string)child.GetValue(1);
            node.AddValue(new BinaryNumberValue(number, text.Length));
            return node;
        }

        /**
         * Adds a MIB number value as the node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitHexadecimalValue(Production node)
        {

            Node child;
            BigInteger number;
            string text;

            child = GetChildAt(node, 0);
            number = (BigInteger)child.GetValue(0);
            text = (string)child.GetValue(1);
            node.AddValue(new HexNumberValue(number, text.Length));
            return node;
        }

        /**
         * Adds a MIB string value as the node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitStringValue(Production node)
        {

            string str;

            str = GetStringValue(GetChildAt(node, 0), 0);
            node.AddValue(new StringValue(str));
            return node;
        }

        /**
         * Adds a MIB value as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitBitOrObjectIdentifierValue(Production node)
        {

            if (MibAnalyzerUtil.IsBitValue(node))
            {
                return ExitBitValue(node);
            }
            else
            {
                return ExitObjectIdentifierValue(node);
            }
        }

        /**
         * Adds a MIB bit set value as the node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitBitValue(Production node)
        {
            System.Collections.ArrayList components = GetChildValues(node);
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

        /**
         * Adds a MIB object identifier value as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitObjectIdentifierValue(Production node)
        {

            System.Collections.ArrayList components = GetChildValues(node);
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
                        parent = new ValueReference(getLocation(node),
                                                    getContext(),
                                                    DefaultContext.CCITT);
                    }
                    else if (parent == null && value == 1)
                    {
                        parent = new ValueReference(getLocation(node),
                                                    getContext(),
                                                    DefaultContext.ISO);
                    }
                    else if (parent == null && value == 2)
                    {
                        parent = new ValueReference(getLocation(node),
                                                    getContext(),
                                                    DefaultContext.JOINTISOCCITT);
                    }
                    else if (parent is ObjectIdentifierValue)
                    {
                        try
                        {
                            parent = new ObjectIdentifierValue(
                                            getLocation(node),
                                            (ObjectIdentifierValue)parent,
                                            number.Name,
                                            value);
                        }
                        catch (MibException e)
                        {
                            log.AddError(e.Location, e.Message);
                            parent = null;
                        }
                    }
                    else
                    {
                        parent = new ObjectIdentifierValue(
                                            getLocation(node),
                                            (ValueReference)parent,
                                            number.Name,
                                            value);
                    }
                }
                else if (parent != null)
                {
                    throw new ParseException(
                        ParseException.ErrorType.ANALYSIS,
                        "object identifier component '" + number.Name +
                        "' has been previously defined, remove any " +
                        "components to the left",
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

        /**
         * Adds all the named numbers as the node values.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitNameValueList(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds a named number as the node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitNameValueComponent(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }


        /**
         * Adds a named number as the node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitNameOrNumber(Production node)
        {

            NamedNumber value;
            Object obj;
            ValueReference vref;

            obj = GetValue(GetChildAt(node, 0), 0);
            if (obj is BigInteger)
            {
                value = new NamedNumber((BigInteger) obj);
            }
            else if (obj is string)
            {
                vref = new ValueReference(getLocation(node),
                                         getContext(),
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

        /**
         * Adds a named number as the node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitNameAndNumber(Production node)
        {

            NamedNumber value;
            string name;
            Object obj;

            name = GetStringValue(GetChildAt(node, 0), 0);
            obj = GetValue(GetChildAt(node, 2), 0);
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

        /**
         * Adds an SNMP type as the node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitDefinedMacroType(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds the defined macro name as the node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitDefinedMacroName(Production node)
        {
            node.AddValue(((Token)node.GetChildAt(0)).GetImage());
            return node;
        }

        /**
         * Adds an SNMP module identity as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitSnmpModuleIdentityMacroType(Production node)
        {

            string update;
            string org;
            string contact;
            string desc;
            System.Collections.ArrayList revisions = new System.Collections.ArrayList();

            currentMib.SmiVersion = 2;
            update = GetStringValue(GetChildAt(node, 1), 0);
            org = GetStringValue(GetChildAt(node, 2), 0);
            contact = GetStringValue(GetChildAt(node, 3), 0);
            desc = GetStringValue(GetChildAt(node, 4), 0);
            for (int i = 5; i < node.GetChildCount(); i++)
            {
                revisions.Add(GetValue(GetChildAt(node, i), 0));
            }
            node.AddValue(new SnmpModuleIdentity(update,
                                                 org,
                                                 contact,
                                                 desc,
                                                 revisions.OfType<SnmpRevision>().ToList()));
            return node;
        }

        /**
         * Called when exiting a parse tree node.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitSnmpObjectIdentityMacroType(Production node)
        {

            SnmpStatus status;
            string desc;
            string sref;

            currentMib.SmiVersion = 2;
            status = (SnmpStatus)GetValue(GetChildAt(node, 1), 0);
            desc = GetStringValue(GetChildAt(node, 2), 0);
            if (node.GetChildCount() <= 3)
            {
                sref = null;
            }
            else
            {
                sref = GetStringValue(GetChildAt(node, 3), 0);
            }
            node.AddValue(new SnmpObjectIdentity(status, desc, sref));
            return node;
        }

        /**
         * Adds the syntax type to the MIB context stack if possible.
         *
         * @param node           the parent node
         * @param child          the child node, or null
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override void ChildSnmpObjectTypeMacroType(Production node, Node child)
        {

            MibType type;

            if (child.GetId() == (int)Asn1Constants.SNMP_SYNTAX_PART)
            {
                type = (MibType)GetValue(child, 0);
                if (type is IMibContext)
                {
                    pushContextExtension((IMibContext)type);
                }
            }
            node.AddChild(child);
        }

        /**
         * Adds an SNMP object type as a node value. This method also
         * removes any syntax type from the MIB context stack if needed.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitSnmpObjectTypeMacroType(Production node)
        {

            SnmpObjectType type;
            MibType syntax = null;
            string units = null;
            SnmpAccess access = null;
            SnmpStatus status = null;
            string desc = null;
            string sref = null;
            Object index = null;
            MibValue defVal = null;
            Node child;

            for (int i = 0; i < node.GetChildCount(); i++)
            {
                child = node.GetChildAt(i);
                switch ((Asn1Constants)child.GetId())
                {
                    case Asn1Constants.SNMP_SYNTAX_PART:
                        syntax = (MibType)GetValue(child, 0);
                        if (syntax is IMibContext)
                        {
                            popContext();
                        }
                        syntax.Comment = MibAnalyzerUtil.GetComments(child);
                        break;
                    case Asn1Constants.SNMP_UNITS_PART:
                        units = GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_ACCESS_PART:
                        access = (SnmpAccess)GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_STATUS_PART:
                        status = (SnmpStatus)GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_DESCR_PART:
                        desc = GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_REFER_PART:
                        sref = GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_INDEX_PART:
                        index = GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_DEF_VAL_PART:
                        defVal = (MibValue)GetValue(child, 0);
                        break;
                }
            }
            if (index is System.Collections.ArrayList)
            {
                type = new SnmpObjectType(syntax,
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
                type = new SnmpObjectType(syntax,
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

        /**
         * Adds an SNMP notification type as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitSnmpNotificationTypeMacroType(Production node)
        {

            System.Collections.ArrayList objects = new System.Collections.ArrayList();
            SnmpStatus status = null;
            string desc = null;
            string sref = null;
            Node child;

            currentMib.SmiVersion = 2;
            for (int i = 0; i < node.GetChildCount(); i++)
            {
                child = node.GetChildAt(i);
                switch ((Asn1Constants)child.GetId())
                {
                    case Asn1Constants.SNMP_OBJECTS_PART:
                        objects = (System.Collections.ArrayList)GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_STATUS_PART:
                        status = (SnmpStatus)GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_DESCR_PART:
                        desc = GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_REFER_PART:
                        sref = GetStringValue(child, 0);
                        break;
                }
            }
            node.AddValue(new SnmpNotificationType(objects.OfType<MibValue>().ToList(),
                                                   status,
                                                   desc,
                                                   sref));
            return node;
        }

        /**
         * Adds an SNMP trap type as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
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
                        enterprise = (MibValue)GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_VAR_PART:
                        vars = child.Values;
                        break;
                    case Asn1Constants.SNMP_DESCR_PART:
                        desc = GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_REFER_PART:
                        sref = GetStringValue(child, 0);
                        break;
                }
            }
            node.AddValue(new SnmpTrapType(enterprise, vars.OfType<MibValue>().ToList(), desc, sref));
            return node;
        }

        /**
         * Adds an SNMP textual convention as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitSnmpTextualConventionMacroType(Production node)
        {

            string display = null;
            SnmpStatus status = null;
            string desc = null;
            string sref = null;
            MibType syntax = null;
            Node child;

            currentMib.SmiVersion = 2;
            for (int i = 0; i < node.GetChildCount(); i++)
            {
                child = node.GetChildAt(i);
                switch ((Asn1Constants)child.GetId())
                {
                    case Asn1Constants.SNMP_DISPLAY_PART:
                        display = GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_STATUS_PART:
                        status = (SnmpStatus)GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_DESCR_PART:
                        desc = GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_REFER_PART:
                        sref = GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_SYNTAX_PART:
                        syntax = (MibType)GetValue(child, 0);
                        syntax.Comment = (MibAnalyzerUtil.GetComments(child));
                        break;
                }
            }
            node.AddValue(new SnmpTextualConvention(display,
                                                    status,
                                                    desc,
                                                    sref,
                                                    syntax));
            return node;
        }

        /**
         * Adds an SNMP object group as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitSnmpObjectGroupMacroType(Production node)
        {

            System.Collections.ArrayList objects;
            SnmpStatus status;
            string desc;
            string sref;

            currentMib.SmiVersion = 2;
            objects = (System.Collections.ArrayList)GetValue(GetChildAt(node, 1), 0);
            status = (SnmpStatus)GetValue(GetChildAt(node, 2), 0);
            desc = GetStringValue(GetChildAt(node, 3), 0);
            if (node.GetChildCount() <= 4)
            {
                sref = null;
            }
            else
            {
                sref = GetStringValue(GetChildAt(node, 4), 0);
            }
            node.AddValue(new SnmpObjectGroup(objects.OfType<MibValue>().ToList(), status, desc, sref));
            return node;
        }

        /**
         * Adds an SNMP notification group as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitSnmpNotificationGroupMacroType(Production node)
        {

            System.Collections.ArrayList notifications;
            SnmpStatus status;
            string desc;
            string sref;

            currentMib.SmiVersion = 2;
            notifications = GetChildAt(node, 1).Values;
            status = (SnmpStatus)GetValue(GetChildAt(node, 2), 0);
            desc = GetStringValue(GetChildAt(node, 3), 0);
            if (node.GetChildCount() <= 4)
            {
                sref = null;
            }
            else
            {
                sref = GetStringValue(GetChildAt(node, 4), 0);
            }
            node.AddValue(new SnmpNotificationGroup(notifications.OfType<MibValue>().ToList(),
                                                    status,
                                                    desc,
                                                    sref));
            return node;
        }

        /**
         * Adds an SNMP module compliance type as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitSnmpModuleComplianceMacroType(Production node)
        {

            SnmpStatus status = null;
            string desc = null;
            string sref = null;
            System.Collections.ArrayList modules = new System.Collections.ArrayList();
            Node child;

            currentMib.SmiVersion = 2;
            for (int i = 0; i < node.GetChildCount(); i++)
            {
                child = node.GetChildAt(i);
                switch ((Asn1Constants)child.GetId())
                {
                    case Asn1Constants.SNMP_STATUS_PART:
                        status = (SnmpStatus)GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_DESCR_PART:
                        desc = GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_REFER_PART:
                        sref = GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_MODULE_PART:
                        modules.Add(GetValue(child, 0));
                        break;
                }
            }
            node.AddValue(new SnmpModuleCompliance(status,
                                                   desc,
                                                   sref,
                                                   modules.OfType<SnmpModule>().ToList()));
            return node;
        }

        /**
         * Adds an SNMP agent capabilities as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitSnmpAgentCapabilitiesMacroType(Production node)
        {

            string prod = null;
            SnmpStatus status = null;
            string desc = null;
            string sref = null;
            System.Collections.ArrayList modules = new System.Collections.ArrayList();
            Node child;

            currentMib.SmiVersion = 2;
            for (int i = 0; i < node.GetChildCount(); i++)
            {
                child = node.GetChildAt(i);
                switch ((Asn1Constants)child.GetId())
                {
                    case Asn1Constants.SNMP_PRODUCT_RELEASE_PART:
                        prod = GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_STATUS_PART:
                        status = (SnmpStatus)GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_DESCR_PART:
                        desc = GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_REFER_PART:
                        sref = GetStringValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_MODULE_SUPPORT_PART:
                        modules.Add(GetValue(child, 0));
                        break;
                }
            }
            node.AddValue(new SnmpAgentCapabilities(prod,
                                                    status,
                                                    desc,
                                                    sref,
                                                    modules.OfType<SnmpModuleSupport>().ToList()));
            return node;
        }

        /**
         * Adds the last update string as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSnmpUpdatePart(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds the organization name as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSnmpOrganizationPart(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds the organization contact info as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSnmpContactPart(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds the description string as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSnmpDescrPart(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds an SNMP revision as the node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitSnmpRevisionPart(Production node)
        {

            SnmpRevision rev;
            MibValue value;
            string desc;

            value = (MibValue)GetValue(GetChildAt(node, 1), 0);
            desc = GetStringValue(GetChildAt(node, 3), 0);
            rev = new SnmpRevision(value, desc);
            rev.setComment(MibAnalyzerUtil.GetComments(node));
            node.AddValue(rev);
            return node;
        }

        /**
         * Adds an SNMP status as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitSnmpStatusPart(Production node)
        {

            Node child;
            string name;

            child = GetChildAt(node, 1);
            name = GetStringValue(child, 0);
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

        /**
         * Adds the reference string as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSnmpReferPart(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds a MIB type as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSnmpSyntaxPart(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds the units string as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSnmpUnitsPart(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds the SNMP access as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitSnmpAccessPart(Production node)
        {

            Node child;
            string name;

            child = GetChildAt(node, 0);
            if (child.GetId() != (int)Asn1Constants.ACCESS)
            {
                currentMib.SmiVersion = 2;
            }
            child = GetChildAt(node, 1);
            name = GetStringValue(child, 0);
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

        /**
         * Adds either a list of value or a single value as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitSnmpIndexPart(Production node)
        {

            if (GetChildAt(node, 0).GetId() == (int)Asn1Constants.INDEX)
            {
                node.AddValue(GetChildValues(node));
            }
            else
            {
                node.AddValues(GetChildValues(node));
            }
            return node;
        }

        /**
         * Adds the index MIB values as node values.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitIndexValueList(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds the index MIB value as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitIndexValue(Production node)
        {
            SnmpIndex index;
            Object obj = GetChildValues(node)[0];

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

        /**
         * Adds the index MIB type as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitIndexType(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds the default MIB value as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSnmpDefValPart(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds a list of MIB values as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSnmpObjectsPart(Production node)
        {
            node.AddValue(GetChildValues(node));
            return node;
        }

        /**
         * Adds the MIB values as node values.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitValueList(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds the enterprise MIB value as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSnmpEnterprisePart(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds the variable MIB values as node values.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSnmpVarPart(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds the display hint as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSnmpDisplayPart(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds the MIB values as node values.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSnmpNotificationsPart(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds an SNMP module as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
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
                        name = GetStringValue(child, 0);
                        popContext();
                        break;
                    case Asn1Constants.SNMP_MANDATORY_PART:
                        groups = (System.Collections.ArrayList)GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_COMPLIANCE_PART:
                        modules.Add(GetValue(child, 0));
                        break;
                }
            }
            module = new SnmpModule(name, groups.OfType<MibValue>().ToList(), modules.OfType<SnmpCompliance>().ToList());
            module.Comment = comment;
            node.AddValue(module);
            return node;
        }

        /**
         * Adds the module name as a node value. This method also sets
         * current MIB context to the referenced module. The imports are
         * implicit, meaning that symbol names do not have to be listed
         * in order to be imported.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitSnmpModuleImport(Production node)
        {

            MibImport imp;
            string module;

            // Load referenced module
            module = GetStringValue(GetChildAt(node, 0), 0);
            loader.scheduleLoad(module);

            // Create module reference and context
            imp = new MibImport(loader, getLocation(node), module, null);
            currentMib.AddImport(imp);
            pushContextExtension(imp);

            // Return results
            node.AddValue(module);
            return node;
        }

        /**
         * Adds the list of group values as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSnmpMandatoryPart(Production node)
        {
            node.AddValue(GetChildValues(node));
            return node;
        }

        /**
         * Adds an SNMP compliance object as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSnmpCompliancePart(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds an SNMP compliance object as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
        public override Node ExitComplianceGroup(Production node)
        {

            SnmpCompliance comp;
            MibValue value;
            string desc;

            value = (MibValue)GetValue(GetChildAt(node, 1), 0);
            desc = GetStringValue(GetChildAt(node, 2), 0);
            comp = new SnmpCompliance(true, value, null, null, null, desc);
            comp.setComment(MibAnalyzerUtil.GetComments(node));
            node.AddValue(comp);
            return node;
        }

        /**
         * Adds an SNMP compliance object as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
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
                        value = (MibValue)GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_SYNTAX_PART:
                        syntax = (MibType)GetValue(child, 0);
                        syntax.Comment = MibAnalyzerUtil.GetComments(child);
                        break;
                    case Asn1Constants.SNMP_WRITE_SYNTAX_PART:
                        write = (MibType)GetValue(child, 0);
                        write.Comment = MibAnalyzerUtil.GetComments(child);
                        break;
                    case Asn1Constants.SNMP_ACCESS_PART:
                        access = (SnmpAccess)GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_DESCR_PART:
                        desc = GetStringValue(child, 0);
                        break;
                }
            }
            comp = new SnmpCompliance(false, value, syntax, write, access, desc);
            comp.setComment(MibAnalyzerUtil.GetComments(node));
            node.AddValue(comp);
            return node;
        }

        /**
         * Adds the MIB type as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSnmpWriteSyntaxPart(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds the product release string as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSnmpProductReleasePart(Production node)
        {
            node.AddValues(GetChildValues(node));
            return node;
        }

        /**
         * Adds an SNMP module support as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
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
                        module = GetStringValue(child, 0);
                        popContext();
                        break;
                    case Asn1Constants.VALUE_LIST:
                        groups = child.Values;
                        break;
                    case Asn1Constants.SNMP_VARIATION_PART:
                        vars.Add(GetValue(child, 0));
                        break;
                }
            }
            node.Values.Add(new SnmpModuleSupport(module, groups.OfType<MibValue>().ToList(), vars.OfType<SnmpVariation>().ToList()));
            return node;
        }

        /**
         * Modifies the MIB context stack to make sure all references are
         * interpreted in the context of the symbol being modified.
         *
         * @param node           the parent node
         * @param child          the child node, or null
         *
         * @throws ParseException if the node analysis discovered errors
         */
        protected void childSnmpVariationPart(Production node, Node child)
        {

            MibType type;
            IMibContext context;

            if (child.GetId() == (int)Asn1Constants.VALUE)
            {
                context = new MibTypeContext(GetValue(child, 0));
                pushContextExtension(context);
            }
            else if (child.GetId() == (int)Asn1Constants.SNMP_SYNTAX_PART)
            {
                type = (MibType)GetValue(child, 0);
                if (type is IMibContext)
                {
                    pushContextExtension((IMibContext)type);
                }
            }
            node.AddChild(child);
        }

        /**
         * Adds an SNMP variation as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         *
         * @throws ParseException if the node analysis discovered errors
         */
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
                        value = (MibValue)GetValue(child, 0);
                        popContext();
                        break;
                    case Asn1Constants.SNMP_SYNTAX_PART:
                        syntax = (MibType)GetValue(child, 0);
                        if (syntax is IMibContext)
                        {
                            popContext();
                        }
                        syntax.Comment = MibAnalyzerUtil.GetComments(child);
                        break;
                    case Asn1Constants.SNMP_WRITE_SYNTAX_PART:
                        write = (MibType)GetValue(child, 0);
                        write.Comment = MibAnalyzerUtil.GetComments(child);
                        break;
                    case Asn1Constants.SNMP_ACCESS_PART:
                        access = (SnmpAccess)GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_CREATION_PART:
                        reqs = (ArrayList)GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_DEF_VAL_PART:
                        defVal = (MibValue)GetValue(child, 0);
                        break;
                    case Asn1Constants.SNMP_DESCR_PART:
                        desc = GetStringValue(child, 0);
                        break;
                }
            }
            node.Values.Add(new SnmpVariation(value,
                                            syntax,
                                            write,
                                            access,
                                            reqs.OfType<MibValue>().ToList(),
                                            defVal,
                                            desc));
            return node;
        }

        /**
         * Adds a list of the MIB values as a node value.
         *
         * @param node           the node being exited
         *
         * @return the node to add to the parse tree
         */
        public override Node ExitSnmpCreationPart(Production node)
        {
            node.Values.Add(GetChildValues(node));
            return node;
        }


        /**
         * Returns the location of a specified node.
         *
         * @param node           the parse tree node
         *
         * @return the file location of the node
         */
        private FileLocation getLocation(Node node)
        {
            return new FileLocation(file,
                                    node.GetStartLine(),
                                    node.GetStartColumn());
        }

        /**
         * Returns the top context on the context stack.
         *
         * @return the top context on the context stack
         */
        private IMibContext getContext()
        {
            return (IMibContext)contextStack[contextStack.Count - 1];
        }

        /**
         * Adds a new context to the top of the context stack.
         *
         * @param context        the context to add
         */
        private void pushContext(IMibContext context)
        {
            contextStack.Add(context);
        }

        /**
         * Adds an extension to the current context to the top of the
         * context stack. A new compound context will be created by
         * appending the top context to the specified one.
         *
         * @param context        the context extension to add
         */
        private void pushContextExtension(IMibContext context)
        {
            pushContext(new CompoundContext(context, getContext()));
        }

        /**
         * Removes the top context on the context stack.
         */
        private void popContext()
        {
            contextStack.RemoveAt(contextStack.Count - 1);
        }
    }

}
