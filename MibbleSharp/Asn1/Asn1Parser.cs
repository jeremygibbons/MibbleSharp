/*
 * Asn1Parser.cs
 *
 * THIS FILE HAS BEEN GENERATED AUTOMATICALLY. DO NOT EDIT!
 *
 * This work is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published
 * by the Free Software Foundation; either version 2 of the License,
 * or (at your option) any later version.
 *
 * This work is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307
 * USA
 *
 * Copyright (c) 2004-2009 Per Cederberg. All rights reserved.
 */

using System.IO;

using PerCederberg.Grammatica.Runtime;

namespace MibbleSharp.Asn1
{

    /**
     * <remarks>A token stream parser.</remarks>
     */
    internal class Asn1Parser : RecursiveDescentParser
    {

        /**
         * <summary>An enumeration with the generated production node
         * identity constants.</summary>
         */
        private enum SynteticPatterns
        {
            SUBPRODUCTION_1 = 3001,
            SUBPRODUCTION_2 = 3002,
            SUBPRODUCTION_3 = 3003,
            SUBPRODUCTION_4 = 3004,
            SUBPRODUCTION_5 = 3005,
            SUBPRODUCTION_6 = 3006,
            SUBPRODUCTION_7 = 3007
        }

        /**
         * <summary>Creates a new parser with a default analyzer.</summary>
         *
         * <param name='input'>the input stream to read from</param>
         *
         * <exception cref='ParserCreationException'>if the parser
         * couldn't be initialized correctly</exception>
         */
        public Asn1Parser(TextReader input)
            : base(input)
        {

            CreatePatterns();
        }

        /**
         * <summary>Creates a new parser.</summary>
         *
         * <param name='input'>the input stream to read from</param>
         *
         * <param name='analyzer'>the analyzer to parse with</param>
         *
         * <exception cref='ParserCreationException'>if the parser
         * couldn't be initialized correctly</exception>
         */
        public Asn1Parser(TextReader input, Asn1Analyzer analyzer)
            : base(input, analyzer)
        {

            CreatePatterns();
        }

        /**
         * <summary>Creates a new tokenizer for this parser. Can be overridden
         * by a subclass to provide a custom implementation.</summary>
         *
         * <param name='input'>the input stream to read from</param>
         *
         * <returns>the tokenizer created</returns>
         *
         * <exception cref='ParserCreationException'>if the tokenizer
         * couldn't be initialized correctly</exception>
         */
        protected override Tokenizer NewTokenizer(TextReader input)
        {
            return new Asn1Tokenizer(input);
        }

        /**
         * <summary>Initializes the parser by creating all the production
         * patterns.</summary>
         *
         * <exception cref='ParserCreationException'>if the parser
         * couldn't be initialized correctly</exception>
         */
        private void CreatePatterns()
        {
            ProductionPattern pattern;
            ProductionPatternAlternative alt;

            pattern = new ProductionPattern((int)Asn1Constants.START,
                                            "Start");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.MODULE_DEFINITION, 1, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.MODULE_DEFINITION,
                                            "ModuleDefinition");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.MODULE_IDENTIFIER, 1, 1);
            alt.AddToken((int)Asn1Constants.DEFINITIONS, 1, 1);
            alt.AddProduction((int)Asn1Constants.TAG_DEFAULT, 0, 1);
            alt.AddToken((int)Asn1Constants.DEFINITION, 1, 1);
            alt.AddToken((int)Asn1Constants.BEGIN, 1, 1);
            alt.AddProduction((int)Asn1Constants.MODULE_BODY, 0, 1);
            alt.AddToken((int)Asn1Constants.END, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.MODULE_IDENTIFIER,
                                            "ModuleIdentifier");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            alt.AddProduction((int)Asn1Constants.OBJECT_IDENTIFIER_VALUE, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.MODULE_REFERENCE,
                                            "ModuleReference");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            alt.AddToken((int)Asn1Constants.DOT, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.TAG_DEFAULT,
                                            "TagDefault");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.EXPLICIT, 1, 1);
            alt.AddToken((int)Asn1Constants.TAGS, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IMPLICIT, 1, 1);
            alt.AddToken((int)Asn1Constants.TAGS, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.MODULE_BODY,
                                            "ModuleBody");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.EXPORT_LIST, 0, 1);
            alt.AddProduction((int)Asn1Constants.IMPORT_LIST, 0, 1);
            alt.AddProduction((int)Asn1Constants.ASSIGNMENT_LIST, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.EXPORT_LIST,
                                            "ExportList");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.EXPORTS, 1, 1);
            alt.AddProduction((int)Asn1Constants.SYMBOL_LIST, 0, 1);
            alt.AddToken((int)Asn1Constants.SEMI_COLON, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.IMPORT_LIST,
                                            "ImportList");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IMPORTS, 1, 1);
            alt.AddProduction((int)Asn1Constants.SYMBOLS_FROM_MODULE, 0, -1);
            alt.AddToken((int)Asn1Constants.SEMI_COLON, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SYMBOLS_FROM_MODULE,
                                            "SymbolsFromModule");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.SYMBOL_LIST, 1, 1);
            alt.AddToken((int)Asn1Constants.FROM, 1, 1);
            alt.AddProduction((int)Asn1Constants.MODULE_IDENTIFIER, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SYMBOL_LIST,
                                            "SymbolList");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.SYMBOL, 1, 1);
            alt.AddProduction((int)SynteticPatterns.SUBPRODUCTION_1, 0, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SYMBOL,
                                            "Symbol");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.DEFINED_MACRO_NAME, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.ASSIGNMENT_LIST,
                                            "AssignmentList");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.ASSIGNMENT, 1, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.ASSIGNMENT,
                                            "Assignment");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.MACRO_DEFINITION, 1, 1);
            alt.AddToken((int)Asn1Constants.SEMI_COLON, 0, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.TYPE_ASSIGNMENT, 1, 1);
            alt.AddToken((int)Asn1Constants.SEMI_COLON, 0, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.VALUE_ASSIGNMENT, 1, 1);
            alt.AddToken((int)Asn1Constants.SEMI_COLON, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.MACRO_DEFINITION,
                                            "MacroDefinition");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.MACRO_REFERENCE, 1, 1);
            alt.AddToken((int)Asn1Constants.MACRO, 1, 1);
            alt.AddToken((int)Asn1Constants.DEFINITION, 1, 1);
            alt.AddProduction((int)Asn1Constants.MACRO_BODY, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.MACRO_REFERENCE,
                                            "MacroReference");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.DEFINED_MACRO_NAME, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.MACRO_BODY,
                                            "MacroBody");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.BEGIN, 1, 1);
            alt.AddProduction((int)Asn1Constants.MACRO_BODY_ELEMENT, 0, -1);
            alt.AddToken((int)Asn1Constants.END, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.MODULE_REFERENCE, 1, 1);
            alt.AddProduction((int)Asn1Constants.MACRO_REFERENCE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.MACRO_BODY_ELEMENT,
                                            "MacroBodyElement");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.LEFT_PAREN, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.RIGHT_PAREN, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.VERTICAL_BAR, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.DEFINITION, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.INTEGER, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.REAL, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.BOOLEAN, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.NULL, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.BIT, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.OCTET, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.STRING, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.OBJECT, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IDENTIFIER, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.QUOTED_STRING, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.TYPE_ASSIGNMENT,
                                            "TypeAssignment");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            alt.AddToken((int)Asn1Constants.DEFINITION, 1, 1);
            alt.AddProduction((int)Asn1Constants.TYPE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.TYPE,
                                            "Type");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.BUILTIN_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.DEFINED_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.DEFINED_MACRO_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.DEFINED_TYPE,
                                            "DefinedType");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.MODULE_REFERENCE, 0, 1);
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE_OR_CONSTRAINT_LIST, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.BUILTIN_TYPE,
                                            "BuiltinType");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.NULL_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.BOOLEAN_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.REAL_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.INTEGER_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.OBJECT_IDENTIFIER_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.STRING_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.BIT_STRING_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.BITS_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.SEQUENCE_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.SEQUENCE_OF_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.SET_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.SET_OF_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.CHOICE_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.ENUMERATED_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.SELECTION_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.TAGGED_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.ANY_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.NULL_TYPE,
                                            "NullType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.NULL, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.BOOLEAN_TYPE,
                                            "BooleanType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.BOOLEAN, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.REAL_TYPE,
                                            "RealType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.REAL, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.INTEGER_TYPE,
                                            "IntegerType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.INTEGER, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE_OR_CONSTRAINT_LIST, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.OBJECT_IDENTIFIER_TYPE,
                                            "ObjectIdentifierType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.OBJECT, 1, 1);
            alt.AddToken((int)Asn1Constants.IDENTIFIER, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.STRING_TYPE,
                                            "StringType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.OCTET, 1, 1);
            alt.AddToken((int)Asn1Constants.STRING, 1, 1);
            alt.AddProduction((int)Asn1Constants.CONSTRAINT_LIST, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.BIT_STRING_TYPE,
                                            "BitStringType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.BIT, 1, 1);
            alt.AddToken((int)Asn1Constants.STRING, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE_OR_CONSTRAINT_LIST, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.BITS_TYPE,
                                            "BitsType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.BITS, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE_OR_CONSTRAINT_LIST, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SEQUENCE_TYPE,
                                            "SequenceType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.SEQUENCE, 1, 1);
            alt.AddToken((int)Asn1Constants.LEFT_BRACE, 1, 1);
            alt.AddProduction((int)Asn1Constants.ELEMENT_TYPE_LIST, 0, 1);
            alt.AddToken((int)Asn1Constants.RIGHT_BRACE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SEQUENCE_OF_TYPE,
                                            "SequenceOfType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.SEQUENCE, 1, 1);
            alt.AddProduction((int)Asn1Constants.CONSTRAINT_LIST, 0, 1);
            alt.AddToken((int)Asn1Constants.OF, 1, 1);
            alt.AddProduction((int)Asn1Constants.TYPE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SET_TYPE,
                                            "SetType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.SET, 1, 1);
            alt.AddToken((int)Asn1Constants.LEFT_BRACE, 1, 1);
            alt.AddProduction((int)Asn1Constants.ELEMENT_TYPE_LIST, 0, 1);
            alt.AddToken((int)Asn1Constants.RIGHT_BRACE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SET_OF_TYPE,
                                            "SetOfType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.SET, 1, 1);
            alt.AddProduction((int)Asn1Constants.SIZE_CONSTRAINT, 0, 1);
            alt.AddToken((int)Asn1Constants.OF, 1, 1);
            alt.AddProduction((int)Asn1Constants.TYPE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.CHOICE_TYPE,
                                            "ChoiceType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.CHOICE, 1, 1);
            alt.AddToken((int)Asn1Constants.LEFT_BRACE, 1, 1);
            alt.AddProduction((int)Asn1Constants.ELEMENT_TYPE_LIST, 1, 1);
            alt.AddToken((int)Asn1Constants.RIGHT_BRACE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.ENUMERATED_TYPE,
                                            "EnumeratedType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.ENUMERATED, 1, 1);
            alt.AddProduction((int)Asn1Constants.NAMED_NUMBER_LIST, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SELECTION_TYPE,
                                            "SelectionType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            alt.AddToken((int)Asn1Constants.LESS_THAN, 1, 1);
            alt.AddProduction((int)Asn1Constants.TYPE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.TAGGED_TYPE,
                                            "TaggedType");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.TAG, 1, 1);
            alt.AddProduction((int)Asn1Constants.EXPLICIT_OR_IMPLICIT_TAG, 0, 1);
            alt.AddProduction((int)Asn1Constants.TYPE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.TAG,
                                            "Tag");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.LEFT_BRACKET, 1, 1);
            alt.AddProduction((int)Asn1Constants.CLASS, 0, 1);
            alt.AddToken((int)Asn1Constants.NUMBER_STRING, 1, 1);
            alt.AddToken((int)Asn1Constants.RIGHT_BRACKET, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.CLASS,
                                            "Class");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.UNIVERSAL, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.APPLICATION, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.PRIVATE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.EXPLICIT_OR_IMPLICIT_TAG,
                                            "ExplicitOrImplicitTag");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.EXPLICIT, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IMPLICIT, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.ANY_TYPE,
                                            "AnyType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.ANY, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.ANY, 1, 1);
            alt.AddToken((int)Asn1Constants.DEFINED, 1, 1);
            alt.AddToken((int)Asn1Constants.BY, 1, 1);
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.ELEMENT_TYPE_LIST,
                                            "ElementTypeList");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.ELEMENT_TYPE, 1, 1);
            alt.AddProduction((int)SynteticPatterns.SUBPRODUCTION_2, 0, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.ELEMENT_TYPE,
                                            "ElementType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 0, 1);
            alt.AddProduction((int)Asn1Constants.TYPE, 1, 1);
            alt.AddProduction((int)Asn1Constants.OPTIONAL_OR_DEFAULT_ELEMENT, 0, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 0, 1);
            alt.AddToken((int)Asn1Constants.COMPONENTS, 1, 1);
            alt.AddToken((int)Asn1Constants.OF, 1, 1);
            alt.AddProduction((int)Asn1Constants.TYPE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.OPTIONAL_OR_DEFAULT_ELEMENT,
                                            "OptionalOrDefaultElement");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.OPTIONAL, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.DEFAULT, 1, 1);
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 0, 1);
            alt.AddProduction((int)Asn1Constants.VALUE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.VALUE_OR_CONSTRAINT_LIST,
                                            "ValueOrConstraintList");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.NAMED_NUMBER_LIST, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.CONSTRAINT_LIST, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.NAMED_NUMBER_LIST,
                                            "NamedNumberList");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.LEFT_BRACE, 1, 1);
            alt.AddProduction((int)Asn1Constants.NAMED_NUMBER, 1, 1);
            alt.AddProduction((int)SynteticPatterns.SUBPRODUCTION_3, 0, -1);
            alt.AddToken((int)Asn1Constants.RIGHT_BRACE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.NAMED_NUMBER,
                                            "NamedNumber");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            alt.AddToken((int)Asn1Constants.LEFT_PAREN, 1, 1);
            alt.AddProduction((int)Asn1Constants.NUMBER, 1, 1);
            alt.AddToken((int)Asn1Constants.RIGHT_PAREN, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.NUMBER,
                                            "Number");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.NUMBER_VALUE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.BINARY_VALUE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.HEXADECIMAL_VALUE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.DEFINED_VALUE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.CONSTRAINT_LIST,
                                            "ConstraintList");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.LEFT_PAREN, 1, 1);
            alt.AddProduction((int)Asn1Constants.CONSTRAINT, 1, 1);
            alt.AddProduction((int)SynteticPatterns.SUBPRODUCTION_4, 0, -1);
            alt.AddToken((int)Asn1Constants.RIGHT_PAREN, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.CONSTRAINT,
                                            "Constraint");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.VALUE_CONSTRAINT, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.SIZE_CONSTRAINT, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.ALPHABET_CONSTRAINT, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.CONTAINED_TYPE_CONSTRAINT, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.INNER_TYPE_CONSTRAINT, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.VALUE_CONSTRAINT_LIST,
                                            "ValueConstraintList");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.LEFT_PAREN, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE_CONSTRAINT, 1, 1);
            alt.AddProduction((int)SynteticPatterns.SUBPRODUCTION_5, 0, -1);
            alt.AddToken((int)Asn1Constants.RIGHT_PAREN, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.VALUE_CONSTRAINT,
                                            "ValueConstraint");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.LOWER_END_POINT, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE_RANGE, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.VALUE_RANGE,
                                            "ValueRange");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.LESS_THAN, 0, 1);
            alt.AddToken((int)Asn1Constants.DOUBLE_DOT, 1, 1);
            alt.AddToken((int)Asn1Constants.LESS_THAN, 0, 1);
            alt.AddProduction((int)Asn1Constants.UPPER_END_POINT, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.LOWER_END_POINT,
                                            "LowerEndPoint");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.VALUE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.MIN, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.UPPER_END_POINT,
                                            "UpperEndPoint");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.VALUE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.MAX, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SIZE_CONSTRAINT,
                                            "SizeConstraint");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.SIZE, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE_CONSTRAINT_LIST, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.ALPHABET_CONSTRAINT,
                                            "AlphabetConstraint");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.FROM, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE_CONSTRAINT_LIST, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.CONTAINED_TYPE_CONSTRAINT,
                                            "ContainedTypeConstraint");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.INCLUDES, 1, 1);
            alt.AddProduction((int)Asn1Constants.TYPE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.INNER_TYPE_CONSTRAINT,
                                            "InnerTypeConstraint");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.WITH, 1, 1);
            alt.AddToken((int)Asn1Constants.COMPONENT, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE_OR_CONSTRAINT_LIST, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.WITH, 1, 1);
            alt.AddToken((int)Asn1Constants.COMPONENTS, 1, 1);
            alt.AddProduction((int)Asn1Constants.COMPONENTS_LIST, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.COMPONENTS_LIST,
                                            "ComponentsList");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.LEFT_BRACE, 1, 1);
            alt.AddProduction((int)Asn1Constants.COMPONENT_CONSTRAINT, 1, 1);
            alt.AddProduction((int)Asn1Constants.COMPONENTS_LIST_TAIL, 0, -1);
            alt.AddToken((int)Asn1Constants.RIGHT_BRACE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.LEFT_BRACE, 1, 1);
            alt.AddToken((int)Asn1Constants.TRIPLE_DOT, 1, 1);
            alt.AddProduction((int)Asn1Constants.COMPONENTS_LIST_TAIL, 1, -1);
            alt.AddToken((int)Asn1Constants.RIGHT_BRACE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.COMPONENTS_LIST_TAIL,
                                            "ComponentsListTail");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.COMMA, 1, 1);
            alt.AddProduction((int)Asn1Constants.COMPONENT_CONSTRAINT, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.COMPONENT_CONSTRAINT,
                                            "ComponentConstraint");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            alt.AddProduction((int)Asn1Constants.COMPONENT_VALUE_PRESENCE, 0, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.COMPONENT_VALUE_PRESENCE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.COMPONENT_VALUE_PRESENCE,
                                            "ComponentValuePresence");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.VALUE_OR_CONSTRAINT_LIST, 1, 1);
            alt.AddProduction((int)Asn1Constants.COMPONENT_PRESENCE, 0, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.COMPONENT_PRESENCE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.COMPONENT_PRESENCE,
                                            "ComponentPresence");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.PRESENT, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.ABSENT, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.OPTIONAL, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.VALUE_ASSIGNMENT,
                                            "ValueAssignment");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            alt.AddProduction((int)Asn1Constants.TYPE, 1, 1);
            alt.AddToken((int)Asn1Constants.DEFINITION, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.VALUE,
                                            "Value");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.BUILTIN_VALUE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.DEFINED_VALUE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.DEFINED_VALUE,
                                            "DefinedValue");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.MODULE_REFERENCE, 0, 1);
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.BUILTIN_VALUE,
                                            "BuiltinValue");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.NULL_VALUE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.BOOLEAN_VALUE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.SPECIAL_REAL_VALUE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.NUMBER_VALUE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.BINARY_VALUE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.HEXADECIMAL_VALUE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.STRING_VALUE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.BIT_OR_OBJECT_IDENTIFIER_VALUE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.NULL_VALUE,
                                            "NullValue");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.NULL, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.BOOLEAN_VALUE,
                                            "BooleanValue");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.TRUE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.FALSE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SPECIAL_REAL_VALUE,
                                            "SpecialRealValue");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.PLUS_INFINITY, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.MINUS_INFINITY, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.NUMBER_VALUE,
                                            "NumberValue");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.MINUS, 0, 1);
            alt.AddToken((int)Asn1Constants.NUMBER_STRING, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.BINARY_VALUE,
                                            "BinaryValue");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.BINARY_STRING, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.HEXADECIMAL_VALUE,
                                            "HexadecimalValue");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.HEXADECIMAL_STRING, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.STRING_VALUE,
                                            "StringValue");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.QUOTED_STRING, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.BIT_OR_OBJECT_IDENTIFIER_VALUE,
                                            "BitOrObjectIdentifierValue");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.NAME_VALUE_LIST, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.BIT_VALUE,
                                            "BitValue");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.NAME_VALUE_LIST, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.OBJECT_IDENTIFIER_VALUE,
                                            "ObjectIdentifierValue");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.NAME_VALUE_LIST, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.NAME_VALUE_LIST,
                                            "NameValueList");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.LEFT_BRACE, 1, 1);
            alt.AddProduction((int)Asn1Constants.NAME_VALUE_COMPONENT, 0, -1);
            alt.AddToken((int)Asn1Constants.RIGHT_BRACE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.NAME_VALUE_COMPONENT,
                                            "NameValueComponent");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.COMMA, 0, 1);
            alt.AddProduction((int)Asn1Constants.NAME_OR_NUMBER, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.NAME_OR_NUMBER,
                                            "NameOrNumber");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.NUMBER_STRING, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.NAME_AND_NUMBER, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.NAME_AND_NUMBER,
                                            "NameAndNumber");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            alt.AddToken((int)Asn1Constants.LEFT_PAREN, 1, 1);
            alt.AddToken((int)Asn1Constants.NUMBER_STRING, 1, 1);
            alt.AddToken((int)Asn1Constants.RIGHT_PAREN, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            alt.AddToken((int)Asn1Constants.LEFT_PAREN, 1, 1);
            alt.AddProduction((int)Asn1Constants.DEFINED_VALUE, 1, 1);
            alt.AddToken((int)Asn1Constants.RIGHT_PAREN, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.DEFINED_MACRO_TYPE,
                                            "DefinedMacroType");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.SNMP_MODULE_IDENTITY_MACRO_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.SNMP_OBJECT_IDENTITY_MACRO_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.SNMP_OBJECT_TYPE_MACRO_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.SNMP_NOTIFICATION_TYPE_MACRO_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.SNMP_TRAP_TYPE_MACRO_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.SNMP_TEXTUAL_CONVENTION_MACRO_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.SNMP_OBJECT_GROUP_MACRO_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.SNMP_NOTIFICATION_GROUP_MACRO_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.SNMP_MODULE_COMPLIANCE_MACRO_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.SNMP_AGENT_CAPABILITIES_MACRO_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.DEFINED_MACRO_NAME,
                                            "DefinedMacroName");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.MODULE_IDENTITY, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.OBJECT_IDENTITY, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.OBJECT_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.NOTIFICATION_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.TRAP_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.TEXTUAL_CONVENTION, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.OBJECT_GROUP, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.NOTIFICATION_GROUP, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.MODULE_COMPLIANCE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.AGENT_CAPABILITIES, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_MODULE_IDENTITY_MACRO_TYPE,
                                            "SnmpModuleIdentityMacroType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.MODULE_IDENTITY, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_UPDATE_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_ORGANIZATION_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_CONTACT_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_DESCR_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_REVISION_PART, 0, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_OBJECT_IDENTITY_MACRO_TYPE,
                                            "SnmpObjectIdentityMacroType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.OBJECT_IDENTITY, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_STATUS_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_DESCR_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_REFER_PART, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_OBJECT_TYPE_MACRO_TYPE,
                                            "SnmpObjectTypeMacroType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.OBJECT_TYPE, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_SYNTAX_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_UNITS_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_ACCESS_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_STATUS_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_DESCR_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_REFER_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_INDEX_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_DEF_VAL_PART, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_NOTIFICATION_TYPE_MACRO_TYPE,
                                            "SnmpNotificationTypeMacroType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.NOTIFICATION_TYPE, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_OBJECTS_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_STATUS_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_DESCR_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_REFER_PART, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_TRAP_TYPE_MACRO_TYPE,
                                            "SnmpTrapTypeMacroType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.TRAP_TYPE, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_ENTERPRISE_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_VAR_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_DESCR_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_REFER_PART, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_TEXTUAL_CONVENTION_MACRO_TYPE,
                                            "SnmpTextualConventionMacroType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.TEXTUAL_CONVENTION, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_DISPLAY_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_STATUS_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_DESCR_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_REFER_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_SYNTAX_PART, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_OBJECT_GROUP_MACRO_TYPE,
                                            "SnmpObjectGroupMacroType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.OBJECT_GROUP, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_OBJECTS_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_STATUS_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_DESCR_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_REFER_PART, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_NOTIFICATION_GROUP_MACRO_TYPE,
                                            "SnmpNotificationGroupMacroType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.NOTIFICATION_GROUP, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_NOTIFICATIONS_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_STATUS_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_DESCR_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_REFER_PART, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_MODULE_COMPLIANCE_MACRO_TYPE,
                                            "SnmpModuleComplianceMacroType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.MODULE_COMPLIANCE, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_STATUS_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_DESCR_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_REFER_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_MODULE_PART, 1, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_AGENT_CAPABILITIES_MACRO_TYPE,
                                            "SnmpAgentCapabilitiesMacroType");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.AGENT_CAPABILITIES, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_PRODUCT_RELEASE_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_STATUS_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_DESCR_PART, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_REFER_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_MODULE_SUPPORT_PART, 0, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_UPDATE_PART,
                                            "SnmpUpdatePart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.LAST_UPDATED, 1, 1);
            alt.AddToken((int)Asn1Constants.QUOTED_STRING, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_ORGANIZATION_PART,
                                            "SnmpOrganizationPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.ORGANIZATION, 1, 1);
            alt.AddToken((int)Asn1Constants.QUOTED_STRING, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_CONTACT_PART,
                                            "SnmpContactPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.CONTACT_INFO, 1, 1);
            alt.AddToken((int)Asn1Constants.QUOTED_STRING, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_DESCR_PART,
                                            "SnmpDescrPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.DESCRIPTION, 1, 1);
            alt.AddToken((int)Asn1Constants.QUOTED_STRING, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_REVISION_PART,
                                            "SnmpRevisionPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.REVISION, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE, 1, 1);
            alt.AddToken((int)Asn1Constants.DESCRIPTION, 1, 1);
            alt.AddToken((int)Asn1Constants.QUOTED_STRING, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_STATUS_PART,
                                            "SnmpStatusPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.STATUS, 1, 1);
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_REFER_PART,
                                            "SnmpReferPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.REFERENCE, 1, 1);
            alt.AddToken((int)Asn1Constants.QUOTED_STRING, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_SYNTAX_PART,
                                            "SnmpSyntaxPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.SYNTAX, 1, 1);
            alt.AddProduction((int)Asn1Constants.TYPE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_UNITS_PART,
                                            "SnmpUnitsPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.UNITS, 1, 1);
            alt.AddToken((int)Asn1Constants.QUOTED_STRING, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_ACCESS_PART,
                                            "SnmpAccessPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.ACCESS, 1, 1);
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.MAX_ACCESS, 1, 1);
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.MIN_ACCESS, 1, 1);
            alt.AddToken((int)Asn1Constants.IDENTIFIER_STRING, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_INDEX_PART,
                                            "SnmpIndexPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.INDEX, 1, 1);
            alt.AddToken((int)Asn1Constants.LEFT_BRACE, 1, 1);
            alt.AddProduction((int)Asn1Constants.INDEX_VALUE_LIST, 1, 1);
            alt.AddToken((int)Asn1Constants.RIGHT_BRACE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.AUGMENTS, 1, 1);
            alt.AddToken((int)Asn1Constants.LEFT_BRACE, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE, 1, 1);
            alt.AddToken((int)Asn1Constants.RIGHT_BRACE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.INDEX_VALUE_LIST,
                                            "IndexValueList");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.INDEX_VALUE, 1, 1);
            alt.AddProduction((int)SynteticPatterns.SUBPRODUCTION_6, 0, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.INDEX_VALUE,
                                            "IndexValue");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.VALUE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.IMPLIED, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.INDEX_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.INDEX_TYPE,
                                            "IndexType");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.INTEGER_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.STRING_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.OBJECT_IDENTIFIER_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_DEF_VAL_PART,
                                            "SnmpDefValPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.DEFVAL, 1, 1);
            alt.AddToken((int)Asn1Constants.LEFT_BRACE, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE, 1, 1);
            alt.AddToken((int)Asn1Constants.RIGHT_BRACE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_OBJECTS_PART,
                                            "SnmpObjectsPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.OBJECTS, 1, 1);
            alt.AddToken((int)Asn1Constants.LEFT_BRACE, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE_LIST, 1, 1);
            alt.AddToken((int)Asn1Constants.RIGHT_BRACE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.VALUE_LIST,
                                            "ValueList");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.VALUE, 1, 1);
            alt.AddProduction((int)SynteticPatterns.SUBPRODUCTION_7, 0, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_ENTERPRISE_PART,
                                            "SnmpEnterprisePart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.ENTERPRISE, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_VAR_PART,
                                            "SnmpVarPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.VARIABLES, 1, 1);
            alt.AddToken((int)Asn1Constants.LEFT_BRACE, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE_LIST, 1, 1);
            alt.AddToken((int)Asn1Constants.RIGHT_BRACE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_DISPLAY_PART,
                                            "SnmpDisplayPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.DISPLAY_HINT, 1, 1);
            alt.AddToken((int)Asn1Constants.QUOTED_STRING, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_NOTIFICATIONS_PART,
                                            "SnmpNotificationsPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.NOTIFICATIONS, 1, 1);
            alt.AddToken((int)Asn1Constants.LEFT_BRACE, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE_LIST, 1, 1);
            alt.AddToken((int)Asn1Constants.RIGHT_BRACE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_MODULE_PART,
                                            "SnmpModulePart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.MODULE, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_MODULE_IMPORT, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_MANDATORY_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_COMPLIANCE_PART, 0, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_MODULE_IMPORT,
                                            "SnmpModuleImport");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.MODULE_IDENTIFIER, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_MANDATORY_PART,
                                            "SnmpMandatoryPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.MANDATORY_GROUPS, 1, 1);
            alt.AddToken((int)Asn1Constants.LEFT_BRACE, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE_LIST, 1, 1);
            alt.AddToken((int)Asn1Constants.RIGHT_BRACE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_COMPLIANCE_PART,
                                            "SnmpCompliancePart");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.COMPLIANCE_GROUP, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int)Asn1Constants.COMPLIANCE_OBJECT, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.COMPLIANCE_GROUP,
                                            "ComplianceGroup");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.GROUP, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_DESCR_PART, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.COMPLIANCE_OBJECT,
                                            "ComplianceObject");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.OBJECT, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_SYNTAX_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_WRITE_SYNTAX_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_ACCESS_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_DESCR_PART, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_WRITE_SYNTAX_PART,
                                            "SnmpWriteSyntaxPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.WRITE_SYNTAX, 1, 1);
            alt.AddProduction((int)Asn1Constants.TYPE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_PRODUCT_RELEASE_PART,
                                            "SnmpProductReleasePart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.PRODUCT_RELEASE, 1, 1);
            alt.AddToken((int)Asn1Constants.QUOTED_STRING, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_MODULE_SUPPORT_PART,
                                            "SnmpModuleSupportPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.SUPPORTS, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_MODULE_IMPORT, 1, 1);
            alt.AddToken((int)Asn1Constants.INCLUDES, 1, 1);
            alt.AddToken((int)Asn1Constants.LEFT_BRACE, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE_LIST, 1, 1);
            alt.AddToken((int)Asn1Constants.RIGHT_BRACE, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_VARIATION_PART, 0, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_VARIATION_PART,
                                            "SnmpVariationPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.VARIATION, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE, 1, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_SYNTAX_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_WRITE_SYNTAX_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_ACCESS_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_CREATION_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_DEF_VAL_PART, 0, 1);
            alt.AddProduction((int)Asn1Constants.SNMP_DESCR_PART, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)Asn1Constants.SNMP_CREATION_PART,
                                            "SnmpCreationPart");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.CREATION_REQUIRES, 1, 1);
            alt.AddToken((int)Asn1Constants.LEFT_BRACE, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE_LIST, 1, 1);
            alt.AddToken((int)Asn1Constants.RIGHT_BRACE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)SynteticPatterns.SUBPRODUCTION_1,
                                            "Subproduction1");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.COMMA, 1, 1);
            alt.AddProduction((int)Asn1Constants.SYMBOL, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)SynteticPatterns.SUBPRODUCTION_2,
                                            "Subproduction2");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.COMMA, 1, 1);
            alt.AddProduction((int)Asn1Constants.ELEMENT_TYPE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)SynteticPatterns.SUBPRODUCTION_3,
                                            "Subproduction3");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.COMMA, 1, 1);
            alt.AddProduction((int)Asn1Constants.NAMED_NUMBER, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)SynteticPatterns.SUBPRODUCTION_4,
                                            "Subproduction4");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.VERTICAL_BAR, 1, 1);
            alt.AddProduction((int)Asn1Constants.CONSTRAINT, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)SynteticPatterns.SUBPRODUCTION_5,
                                            "Subproduction5");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.VERTICAL_BAR, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE_CONSTRAINT, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)SynteticPatterns.SUBPRODUCTION_6,
                                            "Subproduction6");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.COMMA, 1, 1);
            alt.AddProduction((int)Asn1Constants.INDEX_VALUE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int)SynteticPatterns.SUBPRODUCTION_7,
                                            "Subproduction7");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken((int)Asn1Constants.COMMA, 1, 1);
            alt.AddProduction((int)Asn1Constants.VALUE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);
        }
    }
}
