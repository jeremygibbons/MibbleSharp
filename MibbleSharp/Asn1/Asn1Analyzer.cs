/*
 * Asn1Analyzer.cs
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

using PerCederberg.Grammatica.Runtime;

namespace MibbleSharp.Asn1
{

    /**
     * <remarks>A class providing callback methods for the
     * parser.</remarks>
     */
    internal abstract class Asn1Analyzer : Analyzer
    {

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public override void Enter(Node node)
        {
            switch (node.Id)
            {
                case (int)Asn1Constants.DOT:
                    EnterDot((Token)node);
                    break;
                case (int)Asn1Constants.DOUBLE_DOT:
                    EnterDoubleDot((Token)node);
                    break;
                case (int)Asn1Constants.TRIPLE_DOT:
                    EnterTripleDot((Token)node);
                    break;
                case (int)Asn1Constants.COMMA:
                    EnterComma((Token)node);
                    break;
                case (int)Asn1Constants.SEMI_COLON:
                    EnterSemiColon((Token)node);
                    break;
                case (int)Asn1Constants.LEFT_PAREN:
                    EnterLeftParen((Token)node);
                    break;
                case (int)Asn1Constants.RIGHT_PAREN:
                    EnterRightParen((Token)node);
                    break;
                case (int)Asn1Constants.LEFT_BRACE:
                    EnterLeftBrace((Token)node);
                    break;
                case (int)Asn1Constants.RIGHT_BRACE:
                    EnterRightBrace((Token)node);
                    break;
                case (int)Asn1Constants.LEFT_BRACKET:
                    EnterLeftBracket((Token)node);
                    break;
                case (int)Asn1Constants.RIGHT_BRACKET:
                    EnterRightBracket((Token)node);
                    break;
                case (int)Asn1Constants.MINUS:
                    EnterMinus((Token)node);
                    break;
                case (int)Asn1Constants.LESS_THAN:
                    EnterLessThan((Token)node);
                    break;
                case (int)Asn1Constants.VERTICAL_BAR:
                    EnterVerticalBar((Token)node);
                    break;
                case (int)Asn1Constants.DEFINITION:
                    EnterDefinition((Token)node);
                    break;
                case (int)Asn1Constants.DEFINITIONS:
                    EnterDefinitions((Token)node);
                    break;
                case (int)Asn1Constants.EXPLICIT:
                    EnterExplicit((Token)node);
                    break;
                case (int)Asn1Constants.IMPLICIT:
                    EnterImplicit((Token)node);
                    break;
                case (int)Asn1Constants.TAGS:
                    EnterTags((Token)node);
                    break;
                case (int)Asn1Constants.BEGIN:
                    EnterBegin((Token)node);
                    break;
                case (int)Asn1Constants.END:
                    EnterEnd((Token)node);
                    break;
                case (int)Asn1Constants.EXPORTS:
                    EnterExports((Token)node);
                    break;
                case (int)Asn1Constants.IMPORTS:
                    EnterImports((Token)node);
                    break;
                case (int)Asn1Constants.FROM:
                    EnterFrom((Token)node);
                    break;
                case (int)Asn1Constants.MACRO:
                    EnterMacro((Token)node);
                    break;
                case (int)Asn1Constants.INTEGER:
                    EnterInteger((Token)node);
                    break;
                case (int)Asn1Constants.REAL:
                    EnterReal((Token)node);
                    break;
                case (int)Asn1Constants.BOOLEAN:
                    EnterBoolean((Token)node);
                    break;
                case (int)Asn1Constants.NULL:
                    EnterNull((Token)node);
                    break;
                case (int)Asn1Constants.BIT:
                    EnterBit((Token)node);
                    break;
                case (int)Asn1Constants.OCTET:
                    EnterOctet((Token)node);
                    break;
                case (int)Asn1Constants.STRING:
                    EnterString((Token)node);
                    break;
                case (int)Asn1Constants.ENUMERATED:
                    EnterEnumerated((Token)node);
                    break;
                case (int)Asn1Constants.SEQUENCE:
                    EnterSequence((Token)node);
                    break;
                case (int)Asn1Constants.SET:
                    EnterSet((Token)node);
                    break;
                case (int)Asn1Constants.OF:
                    EnterOf((Token)node);
                    break;
                case (int)Asn1Constants.CHOICE:
                    EnterChoice((Token)node);
                    break;
                case (int)Asn1Constants.UNIVERSAL:
                    EnterUniversal((Token)node);
                    break;
                case (int)Asn1Constants.APPLICATION:
                    EnterApplication((Token)node);
                    break;
                case (int)Asn1Constants.PRIVATE:
                    EnterPrivate((Token)node);
                    break;
                case (int)Asn1Constants.ANY:
                    EnterAny((Token)node);
                    break;
                case (int)Asn1Constants.DEFINED:
                    EnterDefined((Token)node);
                    break;
                case (int)Asn1Constants.BY:
                    EnterBy((Token)node);
                    break;
                case (int)Asn1Constants.OBJECT:
                    EnterObject((Token)node);
                    break;
                case (int)Asn1Constants.IDENTIFIER:
                    EnterIdentifier((Token)node);
                    break;
                case (int)Asn1Constants.INCLUDES:
                    EnterIncludes((Token)node);
                    break;
                case (int)Asn1Constants.MIN:
                    EnterMin((Token)node);
                    break;
                case (int)Asn1Constants.MAX:
                    EnterMax((Token)node);
                    break;
                case (int)Asn1Constants.SIZE:
                    EnterSize((Token)node);
                    break;
                case (int)Asn1Constants.WITH:
                    EnterWith((Token)node);
                    break;
                case (int)Asn1Constants.COMPONENT:
                    EnterComponent((Token)node);
                    break;
                case (int)Asn1Constants.COMPONENTS:
                    EnterComponents((Token)node);
                    break;
                case (int)Asn1Constants.PRESENT:
                    EnterPresent((Token)node);
                    break;
                case (int)Asn1Constants.ABSENT:
                    EnterAbsent((Token)node);
                    break;
                case (int)Asn1Constants.OPTIONAL:
                    EnterOptional((Token)node);
                    break;
                case (int)Asn1Constants.DEFAULT:
                    EnterDefault((Token)node);
                    break;
                case (int)Asn1Constants.TRUE:
                    EnterTrue((Token)node);
                    break;
                case (int)Asn1Constants.FALSE:
                    EnterFalse((Token)node);
                    break;
                case (int)Asn1Constants.PLUS_INFINITY:
                    EnterPlusInfinity((Token)node);
                    break;
                case (int)Asn1Constants.MINUS_INFINITY:
                    EnterMinusInfinity((Token)node);
                    break;
                case (int)Asn1Constants.MODULE_IDENTITY:
                    EnterModuleIdentity((Token)node);
                    break;
                case (int)Asn1Constants.OBJECT_IDENTITY:
                    EnterObjectIdentity((Token)node);
                    break;
                case (int)Asn1Constants.OBJECT_TYPE:
                    EnterObjectType((Token)node);
                    break;
                case (int)Asn1Constants.NOTIFICATION_TYPE:
                    EnterNotificationType((Token)node);
                    break;
                case (int)Asn1Constants.TRAP_TYPE:
                    EnterTrapType((Token)node);
                    break;
                case (int)Asn1Constants.TEXTUAL_CONVENTION:
                    EnterTextualConvention((Token)node);
                    break;
                case (int)Asn1Constants.OBJECT_GROUP:
                    EnterObjectGroup((Token)node);
                    break;
                case (int)Asn1Constants.NOTIFICATION_GROUP:
                    EnterNotificationGroup((Token)node);
                    break;
                case (int)Asn1Constants.MODULE_COMPLIANCE:
                    EnterModuleCompliance((Token)node);
                    break;
                case (int)Asn1Constants.AGENT_CAPABILITIES:
                    EnterAgentCapabilities((Token)node);
                    break;
                case (int)Asn1Constants.LAST_UPDATED:
                    EnterLastUpdated((Token)node);
                    break;
                case (int)Asn1Constants.ORGANIZATION:
                    EnterOrganization((Token)node);
                    break;
                case (int)Asn1Constants.CONTACT_INFO:
                    EnterContactInfo((Token)node);
                    break;
                case (int)Asn1Constants.DESCRIPTION:
                    EnterDescription((Token)node);
                    break;
                case (int)Asn1Constants.REVISION:
                    EnterRevision((Token)node);
                    break;
                case (int)Asn1Constants.STATUS:
                    EnterStatus((Token)node);
                    break;
                case (int)Asn1Constants.REFERENCE:
                    EnterReference((Token)node);
                    break;
                case (int)Asn1Constants.SYNTAX:
                    EnterSyntax((Token)node);
                    break;
                case (int)Asn1Constants.BITS:
                    EnterBits((Token)node);
                    break;
                case (int)Asn1Constants.UNITS:
                    EnterUnits((Token)node);
                    break;
                case (int)Asn1Constants.ACCESS:
                    EnterAccess((Token)node);
                    break;
                case (int)Asn1Constants.MAX_ACCESS:
                    EnterMaxAccess((Token)node);
                    break;
                case (int)Asn1Constants.MIN_ACCESS:
                    EnterMinAccess((Token)node);
                    break;
                case (int)Asn1Constants.INDEX:
                    EnterIndex((Token)node);
                    break;
                case (int)Asn1Constants.AUGMENTS:
                    EnterAugments((Token)node);
                    break;
                case (int)Asn1Constants.IMPLIED:
                    EnterImplied((Token)node);
                    break;
                case (int)Asn1Constants.DEFVAL:
                    EnterDefval((Token)node);
                    break;
                case (int)Asn1Constants.OBJECTS:
                    EnterObjects((Token)node);
                    break;
                case (int)Asn1Constants.ENTERPRISE:
                    EnterEnterprise((Token)node);
                    break;
                case (int)Asn1Constants.VARIABLES:
                    EnterVariables((Token)node);
                    break;
                case (int)Asn1Constants.DISPLAY_HINT:
                    EnterDisplayHint((Token)node);
                    break;
                case (int)Asn1Constants.NOTIFICATIONS:
                    EnterNotifications((Token)node);
                    break;
                case (int)Asn1Constants.MODULE:
                    EnterModule((Token)node);
                    break;
                case (int)Asn1Constants.MANDATORY_GROUPS:
                    EnterMandatoryGroups((Token)node);
                    break;
                case (int)Asn1Constants.GROUP:
                    EnterGroup((Token)node);
                    break;
                case (int)Asn1Constants.WRITE_SYNTAX:
                    EnterWriteSyntax((Token)node);
                    break;
                case (int)Asn1Constants.PRODUCT_RELEASE:
                    EnterProductRelease((Token)node);
                    break;
                case (int)Asn1Constants.SUPPORTS:
                    EnterSupports((Token)node);
                    break;
                case (int)Asn1Constants.VARIATION:
                    EnterVariation((Token)node);
                    break;
                case (int)Asn1Constants.CREATION_REQUIRES:
                    EnterCreationRequires((Token)node);
                    break;
                case (int)Asn1Constants.BINARY_STRING:
                    EnterBinaryString((Token)node);
                    break;
                case (int)Asn1Constants.HEXADECIMAL_STRING:
                    EnterHexadecimalString((Token)node);
                    break;
                case (int)Asn1Constants.QUOTED_STRING:
                    EnterQuotedString((Token)node);
                    break;
                case (int)Asn1Constants.IDENTIFIER_STRING:
                    EnterIdentifierString((Token)node);
                    break;
                case (int)Asn1Constants.NUMBER_STRING:
                    EnterNumberString((Token)node);
                    break;
                case (int)Asn1Constants.START:
                    EnterStart((Production)node);
                    break;
                case (int)Asn1Constants.MODULE_DEFINITION:
                    EnterModuleDefinition((Production)node);
                    break;
                case (int)Asn1Constants.MODULE_IDENTIFIER:
                    EnterModuleIdentifier((Production)node);
                    break;
                case (int)Asn1Constants.MODULE_REFERENCE:
                    EnterModuleReference((Production)node);
                    break;
                case (int)Asn1Constants.TAG_DEFAULT:
                    EnterTagDefault((Production)node);
                    break;
                case (int)Asn1Constants.MODULE_BODY:
                    EnterModuleBody((Production)node);
                    break;
                case (int)Asn1Constants.EXPORT_LIST:
                    EnterExportList((Production)node);
                    break;
                case (int)Asn1Constants.IMPORT_LIST:
                    EnterImportList((Production)node);
                    break;
                case (int)Asn1Constants.SYMBOLS_FROM_MODULE:
                    EnterSymbolsFromModule((Production)node);
                    break;
                case (int)Asn1Constants.SYMBOL_LIST:
                    EnterSymbolList((Production)node);
                    break;
                case (int)Asn1Constants.SYMBOL:
                    EnterSymbol((Production)node);
                    break;
                case (int)Asn1Constants.ASSIGNMENT_LIST:
                    EnterAssignmentList((Production)node);
                    break;
                case (int)Asn1Constants.ASSIGNMENT:
                    EnterAssignment((Production)node);
                    break;
                case (int)Asn1Constants.MACRO_DEFINITION:
                    EnterMacroDefinition((Production)node);
                    break;
                case (int)Asn1Constants.MACRO_REFERENCE:
                    EnterMacroReference((Production)node);
                    break;
                case (int)Asn1Constants.MACRO_BODY:
                    EnterMacroBody((Production)node);
                    break;
                case (int)Asn1Constants.MACRO_BODY_ELEMENT:
                    EnterMacroBodyElement((Production)node);
                    break;
                case (int)Asn1Constants.TYPE_ASSIGNMENT:
                    EnterTypeAssignment((Production)node);
                    break;
                case (int)Asn1Constants.TYPE:
                    EnterType((Production)node);
                    break;
                case (int)Asn1Constants.DEFINED_TYPE:
                    EnterDefinedType((Production)node);
                    break;
                case (int)Asn1Constants.BUILTIN_TYPE:
                    EnterBuiltinType((Production)node);
                    break;
                case (int)Asn1Constants.NULL_TYPE:
                    EnterNullType((Production)node);
                    break;
                case (int)Asn1Constants.BOOLEAN_TYPE:
                    EnterBooleanType((Production)node);
                    break;
                case (int)Asn1Constants.REAL_TYPE:
                    EnterRealType((Production)node);
                    break;
                case (int)Asn1Constants.INTEGER_TYPE:
                    EnterIntegerType((Production)node);
                    break;
                case (int)Asn1Constants.OBJECT_IDENTIFIER_TYPE:
                    EnterObjectIdentifierType((Production)node);
                    break;
                case (int)Asn1Constants.STRING_TYPE:
                    EnterStringType((Production)node);
                    break;
                case (int)Asn1Constants.BIT_STRING_TYPE:
                    EnterBitStringType((Production)node);
                    break;
                case (int)Asn1Constants.BITS_TYPE:
                    EnterBitsType((Production)node);
                    break;
                case (int)Asn1Constants.SEQUENCE_TYPE:
                    EnterSequenceType((Production)node);
                    break;
                case (int)Asn1Constants.SEQUENCE_OF_TYPE:
                    EnterSequenceOfType((Production)node);
                    break;
                case (int)Asn1Constants.SET_TYPE:
                    EnterSetType((Production)node);
                    break;
                case (int)Asn1Constants.SET_OF_TYPE:
                    EnterSetOfType((Production)node);
                    break;
                case (int)Asn1Constants.CHOICE_TYPE:
                    EnterChoiceType((Production)node);
                    break;
                case (int)Asn1Constants.ENUMERATED_TYPE:
                    EnterEnumeratedType((Production)node);
                    break;
                case (int)Asn1Constants.SELECTION_TYPE:
                    EnterSelectionType((Production)node);
                    break;
                case (int)Asn1Constants.TAGGED_TYPE:
                    EnterTaggedType((Production)node);
                    break;
                case (int)Asn1Constants.TAG:
                    EnterTag((Production)node);
                    break;
                case (int)Asn1Constants.CLASS:
                    EnterClass((Production)node);
                    break;
                case (int)Asn1Constants.EXPLICIT_OR_IMPLICIT_TAG:
                    EnterExplicitOrImplicitTag((Production)node);
                    break;
                case (int)Asn1Constants.ANY_TYPE:
                    EnterAnyType((Production)node);
                    break;
                case (int)Asn1Constants.ELEMENT_TYPE_LIST:
                    EnterElementTypeList((Production)node);
                    break;
                case (int)Asn1Constants.ELEMENT_TYPE:
                    EnterElementType((Production)node);
                    break;
                case (int)Asn1Constants.OPTIONAL_OR_DEFAULT_ELEMENT:
                    EnterOptionalOrDefaultElement((Production)node);
                    break;
                case (int)Asn1Constants.VALUE_OR_CONSTRAINT_LIST:
                    EnterValueOrConstraintList((Production)node);
                    break;
                case (int)Asn1Constants.NAMED_NUMBER_LIST:
                    EnterNamedNumberList((Production)node);
                    break;
                case (int)Asn1Constants.NAMED_NUMBER:
                    EnterNamedNumber((Production)node);
                    break;
                case (int)Asn1Constants.NUMBER:
                    EnterNumber((Production)node);
                    break;
                case (int)Asn1Constants.CONSTRAINT_LIST:
                    EnterConstraintList((Production)node);
                    break;
                case (int)Asn1Constants.CONSTRAINT:
                    EnterConstraint((Production)node);
                    break;
                case (int)Asn1Constants.VALUE_CONSTRAINT_LIST:
                    EnterValueConstraintList((Production)node);
                    break;
                case (int)Asn1Constants.VALUE_CONSTRAINT:
                    EnterValueConstraint((Production)node);
                    break;
                case (int)Asn1Constants.VALUE_RANGE:
                    EnterValueRange((Production)node);
                    break;
                case (int)Asn1Constants.LOWER_END_POINT:
                    EnterLowerEndPoint((Production)node);
                    break;
                case (int)Asn1Constants.UPPER_END_POINT:
                    EnterUpperEndPoint((Production)node);
                    break;
                case (int)Asn1Constants.SIZE_CONSTRAINT:
                    EnterSizeConstraint((Production)node);
                    break;
                case (int)Asn1Constants.ALPHABET_CONSTRAINT:
                    EnterAlphabetConstraint((Production)node);
                    break;
                case (int)Asn1Constants.CONTAINED_TYPE_CONSTRAINT:
                    EnterContainedTypeConstraint((Production)node);
                    break;
                case (int)Asn1Constants.INNER_TYPE_CONSTRAINT:
                    EnterInnerTypeConstraint((Production)node);
                    break;
                case (int)Asn1Constants.COMPONENTS_LIST:
                    EnterComponentsList((Production)node);
                    break;
                case (int)Asn1Constants.COMPONENTS_LIST_TAIL:
                    EnterComponentsListTail((Production)node);
                    break;
                case (int)Asn1Constants.COMPONENT_CONSTRAINT:
                    EnterComponentConstraint((Production)node);
                    break;
                case (int)Asn1Constants.COMPONENT_VALUE_PRESENCE:
                    EnterComponentValuePresence((Production)node);
                    break;
                case (int)Asn1Constants.COMPONENT_PRESENCE:
                    EnterComponentPresence((Production)node);
                    break;
                case (int)Asn1Constants.VALUE_ASSIGNMENT:
                    EnterValueAssignment((Production)node);
                    break;
                case (int)Asn1Constants.VALUE:
                    EnterValue((Production)node);
                    break;
                case (int)Asn1Constants.DEFINED_VALUE:
                    EnterDefinedValue((Production)node);
                    break;
                case (int)Asn1Constants.BUILTIN_VALUE:
                    EnterBuiltinValue((Production)node);
                    break;
                case (int)Asn1Constants.NULL_VALUE:
                    EnterNullValue((Production)node);
                    break;
                case (int)Asn1Constants.BOOLEAN_VALUE:
                    EnterBooleanValue((Production)node);
                    break;
                case (int)Asn1Constants.SPECIAL_REAL_VALUE:
                    EnterSpecialRealValue((Production)node);
                    break;
                case (int)Asn1Constants.NUMBER_VALUE:
                    EnterNumberValue((Production)node);
                    break;
                case (int)Asn1Constants.BINARY_VALUE:
                    EnterBinaryValue((Production)node);
                    break;
                case (int)Asn1Constants.HEXADECIMAL_VALUE:
                    EnterHexadecimalValue((Production)node);
                    break;
                case (int)Asn1Constants.STRING_VALUE:
                    EnterStringValue((Production)node);
                    break;
                case (int)Asn1Constants.BIT_OR_OBJECT_IDENTIFIER_VALUE:
                    EnterBitOrObjectIdentifierValue((Production)node);
                    break;
                case (int)Asn1Constants.BIT_VALUE:
                    EnterBitValue((Production)node);
                    break;
                case (int)Asn1Constants.OBJECT_IDENTIFIER_VALUE:
                    EnterObjectIdentifierValue((Production)node);
                    break;
                case (int)Asn1Constants.NAME_VALUE_LIST:
                    EnterNameValueList((Production)node);
                    break;
                case (int)Asn1Constants.NAME_VALUE_COMPONENT:
                    EnterNameValueComponent((Production)node);
                    break;
                case (int)Asn1Constants.NAME_OR_NUMBER:
                    EnterNameOrNumber((Production)node);
                    break;
                case (int)Asn1Constants.NAME_AND_NUMBER:
                    EnterNameAndNumber((Production)node);
                    break;
                case (int)Asn1Constants.DEFINED_MACRO_TYPE:
                    EnterDefinedMacroType((Production)node);
                    break;
                case (int)Asn1Constants.DEFINED_MACRO_NAME:
                    EnterDefinedMacroName((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_MODULE_IDENTITY_MACRO_TYPE:
                    EnterSnmpModuleIdentityMacroType((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_OBJECT_IDENTITY_MACRO_TYPE:
                    EnterSnmpObjectIdentityMacroType((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_OBJECT_TYPE_MACRO_TYPE:
                    EnterSnmpObjectTypeMacroType((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_NOTIFICATION_TYPE_MACRO_TYPE:
                    EnterSnmpNotificationTypeMacroType((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_TRAP_TYPE_MACRO_TYPE:
                    EnterSnmpTrapTypeMacroType((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_TEXTUAL_CONVENTION_MACRO_TYPE:
                    EnterSnmpTextualConventionMacroType((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_OBJECT_GROUP_MACRO_TYPE:
                    EnterSnmpObjectGroupMacroType((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_NOTIFICATION_GROUP_MACRO_TYPE:
                    EnterSnmpNotificationGroupMacroType((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_MODULE_COMPLIANCE_MACRO_TYPE:
                    EnterSnmpModuleComplianceMacroType((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_AGENT_CAPABILITIES_MACRO_TYPE:
                    EnterSnmpAgentCapabilitiesMacroType((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_UPDATE_PART:
                    EnterSnmpUpdatePart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_ORGANIZATION_PART:
                    EnterSnmpOrganizationPart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_CONTACT_PART:
                    EnterSnmpContactPart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_DESCR_PART:
                    EnterSnmpDescrPart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_REVISION_PART:
                    EnterSnmpRevisionPart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_STATUS_PART:
                    EnterSnmpStatusPart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_REFER_PART:
                    EnterSnmpReferPart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_SYNTAX_PART:
                    EnterSnmpSyntaxPart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_UNITS_PART:
                    EnterSnmpUnitsPart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_ACCESS_PART:
                    EnterSnmpAccessPart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_INDEX_PART:
                    EnterSnmpIndexPart((Production)node);
                    break;
                case (int)Asn1Constants.INDEX_VALUE_LIST:
                    EnterIndexValueList((Production)node);
                    break;
                case (int)Asn1Constants.INDEX_VALUE:
                    EnterIndexValue((Production)node);
                    break;
                case (int)Asn1Constants.INDEX_TYPE:
                    EnterIndexType((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_DEF_VAL_PART:
                    EnterSnmpDefValPart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_OBJECTS_PART:
                    EnterSnmpObjectsPart((Production)node);
                    break;
                case (int)Asn1Constants.VALUE_LIST:
                    EnterValueList((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_ENTERPRISE_PART:
                    EnterSnmpEnterprisePart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_VAR_PART:
                    EnterSnmpVarPart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_DISPLAY_PART:
                    EnterSnmpDisplayPart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_NOTIFICATIONS_PART:
                    EnterSnmpNotificationsPart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_MODULE_PART:
                    EnterSnmpModulePart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_MODULE_IMPORT:
                    EnterSnmpModuleImport((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_MANDATORY_PART:
                    EnterSnmpMandatoryPart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_COMPLIANCE_PART:
                    EnterSnmpCompliancePart((Production)node);
                    break;
                case (int)Asn1Constants.COMPLIANCE_GROUP:
                    EnterComplianceGroup((Production)node);
                    break;
                case (int)Asn1Constants.COMPLIANCE_OBJECT:
                    EnterComplianceObject((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_WRITE_SYNTAX_PART:
                    EnterSnmpWriteSyntaxPart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_PRODUCT_RELEASE_PART:
                    EnterSnmpProductReleasePart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_MODULE_SUPPORT_PART:
                    EnterSnmpModuleSupportPart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_VARIATION_PART:
                    EnterSnmpVariationPart((Production)node);
                    break;
                case (int)Asn1Constants.SNMP_CREATION_PART:
                    EnterSnmpCreationPart((Production)node);
                    break;
            }
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public override Node Exit(Node node)
        {
            switch (node.Id)
            {
                case (int)Asn1Constants.DOT:
                    return ExitDot((Token)node);
                case (int)Asn1Constants.DOUBLE_DOT:
                    return ExitDoubleDot((Token)node);
                case (int)Asn1Constants.TRIPLE_DOT:
                    return ExitTripleDot((Token)node);
                case (int)Asn1Constants.COMMA:
                    return ExitComma((Token)node);
                case (int)Asn1Constants.SEMI_COLON:
                    return ExitSemiColon((Token)node);
                case (int)Asn1Constants.LEFT_PAREN:
                    return ExitLeftParen((Token)node);
                case (int)Asn1Constants.RIGHT_PAREN:
                    return ExitRightParen((Token)node);
                case (int)Asn1Constants.LEFT_BRACE:
                    return ExitLeftBrace((Token)node);
                case (int)Asn1Constants.RIGHT_BRACE:
                    return ExitRightBrace((Token)node);
                case (int)Asn1Constants.LEFT_BRACKET:
                    return ExitLeftBracket((Token)node);
                case (int)Asn1Constants.RIGHT_BRACKET:
                    return ExitRightBracket((Token)node);
                case (int)Asn1Constants.MINUS:
                    return ExitMinus((Token)node);
                case (int)Asn1Constants.LESS_THAN:
                    return ExitLessThan((Token)node);
                case (int)Asn1Constants.VERTICAL_BAR:
                    return ExitVerticalBar((Token)node);
                case (int)Asn1Constants.DEFINITION:
                    return ExitDefinition((Token)node);
                case (int)Asn1Constants.DEFINITIONS:
                    return ExitDefinitions((Token)node);
                case (int)Asn1Constants.EXPLICIT:
                    return ExitExplicit((Token)node);
                case (int)Asn1Constants.IMPLICIT:
                    return ExitImplicit((Token)node);
                case (int)Asn1Constants.TAGS:
                    return ExitTags((Token)node);
                case (int)Asn1Constants.BEGIN:
                    return ExitBegin((Token)node);
                case (int)Asn1Constants.END:
                    return ExitEnd((Token)node);
                case (int)Asn1Constants.EXPORTS:
                    return ExitExports((Token)node);
                case (int)Asn1Constants.IMPORTS:
                    return ExitImports((Token)node);
                case (int)Asn1Constants.FROM:
                    return ExitFrom((Token)node);
                case (int)Asn1Constants.MACRO:
                    return ExitMacro((Token)node);
                case (int)Asn1Constants.INTEGER:
                    return ExitInteger((Token)node);
                case (int)Asn1Constants.REAL:
                    return ExitReal((Token)node);
                case (int)Asn1Constants.BOOLEAN:
                    return ExitBoolean((Token)node);
                case (int)Asn1Constants.NULL:
                    return ExitNull((Token)node);
                case (int)Asn1Constants.BIT:
                    return ExitBit((Token)node);
                case (int)Asn1Constants.OCTET:
                    return ExitOctet((Token)node);
                case (int)Asn1Constants.STRING:
                    return ExitString((Token)node);
                case (int)Asn1Constants.ENUMERATED:
                    return ExitEnumerated((Token)node);
                case (int)Asn1Constants.SEQUENCE:
                    return ExitSequence((Token)node);
                case (int)Asn1Constants.SET:
                    return ExitSet((Token)node);
                case (int)Asn1Constants.OF:
                    return ExitOf((Token)node);
                case (int)Asn1Constants.CHOICE:
                    return ExitChoice((Token)node);
                case (int)Asn1Constants.UNIVERSAL:
                    return ExitUniversal((Token)node);
                case (int)Asn1Constants.APPLICATION:
                    return ExitApplication((Token)node);
                case (int)Asn1Constants.PRIVATE:
                    return ExitPrivate((Token)node);
                case (int)Asn1Constants.ANY:
                    return ExitAny((Token)node);
                case (int)Asn1Constants.DEFINED:
                    return ExitDefined((Token)node);
                case (int)Asn1Constants.BY:
                    return ExitBy((Token)node);
                case (int)Asn1Constants.OBJECT:
                    return ExitObject((Token)node);
                case (int)Asn1Constants.IDENTIFIER:
                    return ExitIdentifier((Token)node);
                case (int)Asn1Constants.INCLUDES:
                    return ExitIncludes((Token)node);
                case (int)Asn1Constants.MIN:
                    return ExitMin((Token)node);
                case (int)Asn1Constants.MAX:
                    return ExitMax((Token)node);
                case (int)Asn1Constants.SIZE:
                    return ExitSize((Token)node);
                case (int)Asn1Constants.WITH:
                    return ExitWith((Token)node);
                case (int)Asn1Constants.COMPONENT:
                    return ExitComponent((Token)node);
                case (int)Asn1Constants.COMPONENTS:
                    return ExitComponents((Token)node);
                case (int)Asn1Constants.PRESENT:
                    return ExitPresent((Token)node);
                case (int)Asn1Constants.ABSENT:
                    return ExitAbsent((Token)node);
                case (int)Asn1Constants.OPTIONAL:
                    return ExitOptional((Token)node);
                case (int)Asn1Constants.DEFAULT:
                    return ExitDefault((Token)node);
                case (int)Asn1Constants.TRUE:
                    return ExitTrue((Token)node);
                case (int)Asn1Constants.FALSE:
                    return ExitFalse((Token)node);
                case (int)Asn1Constants.PLUS_INFINITY:
                    return ExitPlusInfinity((Token)node);
                case (int)Asn1Constants.MINUS_INFINITY:
                    return ExitMinusInfinity((Token)node);
                case (int)Asn1Constants.MODULE_IDENTITY:
                    return ExitModuleIdentity((Token)node);
                case (int)Asn1Constants.OBJECT_IDENTITY:
                    return ExitObjectIdentity((Token)node);
                case (int)Asn1Constants.OBJECT_TYPE:
                    return ExitObjectType((Token)node);
                case (int)Asn1Constants.NOTIFICATION_TYPE:
                    return ExitNotificationType((Token)node);
                case (int)Asn1Constants.TRAP_TYPE:
                    return ExitTrapType((Token)node);
                case (int)Asn1Constants.TEXTUAL_CONVENTION:
                    return ExitTextualConvention((Token)node);
                case (int)Asn1Constants.OBJECT_GROUP:
                    return ExitObjectGroup((Token)node);
                case (int)Asn1Constants.NOTIFICATION_GROUP:
                    return ExitNotificationGroup((Token)node);
                case (int)Asn1Constants.MODULE_COMPLIANCE:
                    return ExitModuleCompliance((Token)node);
                case (int)Asn1Constants.AGENT_CAPABILITIES:
                    return ExitAgentCapabilities((Token)node);
                case (int)Asn1Constants.LAST_UPDATED:
                    return ExitLastUpdated((Token)node);
                case (int)Asn1Constants.ORGANIZATION:
                    return ExitOrganization((Token)node);
                case (int)Asn1Constants.CONTACT_INFO:
                    return ExitContactInfo((Token)node);
                case (int)Asn1Constants.DESCRIPTION:
                    return ExitDescription((Token)node);
                case (int)Asn1Constants.REVISION:
                    return ExitRevision((Token)node);
                case (int)Asn1Constants.STATUS:
                    return ExitStatus((Token)node);
                case (int)Asn1Constants.REFERENCE:
                    return ExitReference((Token)node);
                case (int)Asn1Constants.SYNTAX:
                    return ExitSyntax((Token)node);
                case (int)Asn1Constants.BITS:
                    return ExitBits((Token)node);
                case (int)Asn1Constants.UNITS:
                    return ExitUnits((Token)node);
                case (int)Asn1Constants.ACCESS:
                    return ExitAccess((Token)node);
                case (int)Asn1Constants.MAX_ACCESS:
                    return ExitMaxAccess((Token)node);
                case (int)Asn1Constants.MIN_ACCESS:
                    return ExitMinAccess((Token)node);
                case (int)Asn1Constants.INDEX:
                    return ExitIndex((Token)node);
                case (int)Asn1Constants.AUGMENTS:
                    return ExitAugments((Token)node);
                case (int)Asn1Constants.IMPLIED:
                    return ExitImplied((Token)node);
                case (int)Asn1Constants.DEFVAL:
                    return ExitDefval((Token)node);
                case (int)Asn1Constants.OBJECTS:
                    return ExitObjects((Token)node);
                case (int)Asn1Constants.ENTERPRISE:
                    return ExitEnterprise((Token)node);
                case (int)Asn1Constants.VARIABLES:
                    return ExitVariables((Token)node);
                case (int)Asn1Constants.DISPLAY_HINT:
                    return ExitDisplayHint((Token)node);
                case (int)Asn1Constants.NOTIFICATIONS:
                    return ExitNotifications((Token)node);
                case (int)Asn1Constants.MODULE:
                    return ExitModule((Token)node);
                case (int)Asn1Constants.MANDATORY_GROUPS:
                    return ExitMandatoryGroups((Token)node);
                case (int)Asn1Constants.GROUP:
                    return ExitGroup((Token)node);
                case (int)Asn1Constants.WRITE_SYNTAX:
                    return ExitWriteSyntax((Token)node);
                case (int)Asn1Constants.PRODUCT_RELEASE:
                    return ExitProductRelease((Token)node);
                case (int)Asn1Constants.SUPPORTS:
                    return ExitSupports((Token)node);
                case (int)Asn1Constants.VARIATION:
                    return ExitVariation((Token)node);
                case (int)Asn1Constants.CREATION_REQUIRES:
                    return ExitCreationRequires((Token)node);
                case (int)Asn1Constants.BINARY_STRING:
                    return ExitBinaryString((Token)node);
                case (int)Asn1Constants.HEXADECIMAL_STRING:
                    return ExitHexadecimalString((Token)node);
                case (int)Asn1Constants.QUOTED_STRING:
                    return ExitQuotedString((Token)node);
                case (int)Asn1Constants.IDENTIFIER_STRING:
                    return ExitIdentifierString((Token)node);
                case (int)Asn1Constants.NUMBER_STRING:
                    return ExitNumberString((Token)node);
                case (int)Asn1Constants.START:
                    return ExitStart((Production)node);
                case (int)Asn1Constants.MODULE_DEFINITION:
                    return ExitModuleDefinition((Production)node);
                case (int)Asn1Constants.MODULE_IDENTIFIER:
                    return ExitModuleIdentifier((Production)node);
                case (int)Asn1Constants.MODULE_REFERENCE:
                    return ExitModuleReference((Production)node);
                case (int)Asn1Constants.TAG_DEFAULT:
                    return ExitTagDefault((Production)node);
                case (int)Asn1Constants.MODULE_BODY:
                    return ExitModuleBody((Production)node);
                case (int)Asn1Constants.EXPORT_LIST:
                    return ExitExportList((Production)node);
                case (int)Asn1Constants.IMPORT_LIST:
                    return ExitImportList((Production)node);
                case (int)Asn1Constants.SYMBOLS_FROM_MODULE:
                    return ExitSymbolsFromModule((Production)node);
                case (int)Asn1Constants.SYMBOL_LIST:
                    return ExitSymbolList((Production)node);
                case (int)Asn1Constants.SYMBOL:
                    return ExitSymbol((Production)node);
                case (int)Asn1Constants.ASSIGNMENT_LIST:
                    return ExitAssignmentList((Production)node);
                case (int)Asn1Constants.ASSIGNMENT:
                    return ExitAssignment((Production)node);
                case (int)Asn1Constants.MACRO_DEFINITION:
                    return ExitMacroDefinition((Production)node);
                case (int)Asn1Constants.MACRO_REFERENCE:
                    return ExitMacroReference((Production)node);
                case (int)Asn1Constants.MACRO_BODY:
                    return ExitMacroBody((Production)node);
                case (int)Asn1Constants.MACRO_BODY_ELEMENT:
                    return ExitMacroBodyElement((Production)node);
                case (int)Asn1Constants.TYPE_ASSIGNMENT:
                    return ExitTypeAssignment((Production)node);
                case (int)Asn1Constants.TYPE:
                    return ExitType((Production)node);
                case (int)Asn1Constants.DEFINED_TYPE:
                    return ExitDefinedType((Production)node);
                case (int)Asn1Constants.BUILTIN_TYPE:
                    return ExitBuiltinType((Production)node);
                case (int)Asn1Constants.NULL_TYPE:
                    return ExitNullType((Production)node);
                case (int)Asn1Constants.BOOLEAN_TYPE:
                    return ExitBooleanType((Production)node);
                case (int)Asn1Constants.REAL_TYPE:
                    return ExitRealType((Production)node);
                case (int)Asn1Constants.INTEGER_TYPE:
                    return ExitIntegerType((Production)node);
                case (int)Asn1Constants.OBJECT_IDENTIFIER_TYPE:
                    return ExitObjectIdentifierType((Production)node);
                case (int)Asn1Constants.STRING_TYPE:
                    return ExitStringType((Production)node);
                case (int)Asn1Constants.BIT_STRING_TYPE:
                    return ExitBitStringType((Production)node);
                case (int)Asn1Constants.BITS_TYPE:
                    return ExitBitsType((Production)node);
                case (int)Asn1Constants.SEQUENCE_TYPE:
                    return ExitSequenceType((Production)node);
                case (int)Asn1Constants.SEQUENCE_OF_TYPE:
                    return ExitSequenceOfType((Production)node);
                case (int)Asn1Constants.SET_TYPE:
                    return ExitSetType((Production)node);
                case (int)Asn1Constants.SET_OF_TYPE:
                    return ExitSetOfType((Production)node);
                case (int)Asn1Constants.CHOICE_TYPE:
                    return ExitChoiceType((Production)node);
                case (int)Asn1Constants.ENUMERATED_TYPE:
                    return ExitEnumeratedType((Production)node);
                case (int)Asn1Constants.SELECTION_TYPE:
                    return ExitSelectionType((Production)node);
                case (int)Asn1Constants.TAGGED_TYPE:
                    return ExitTaggedType((Production)node);
                case (int)Asn1Constants.TAG:
                    return ExitTag((Production)node);
                case (int)Asn1Constants.CLASS:
                    return ExitClass((Production)node);
                case (int)Asn1Constants.EXPLICIT_OR_IMPLICIT_TAG:
                    return ExitExplicitOrImplicitTag((Production)node);
                case (int)Asn1Constants.ANY_TYPE:
                    return ExitAnyType((Production)node);
                case (int)Asn1Constants.ELEMENT_TYPE_LIST:
                    return ExitElementTypeList((Production)node);
                case (int)Asn1Constants.ELEMENT_TYPE:
                    return ExitElementType((Production)node);
                case (int)Asn1Constants.OPTIONAL_OR_DEFAULT_ELEMENT:
                    return ExitOptionalOrDefaultElement((Production)node);
                case (int)Asn1Constants.VALUE_OR_CONSTRAINT_LIST:
                    return ExitValueOrConstraintList((Production)node);
                case (int)Asn1Constants.NAMED_NUMBER_LIST:
                    return ExitNamedNumberList((Production)node);
                case (int)Asn1Constants.NAMED_NUMBER:
                    return ExitNamedNumber((Production)node);
                case (int)Asn1Constants.NUMBER:
                    return ExitNumber((Production)node);
                case (int)Asn1Constants.CONSTRAINT_LIST:
                    return ExitConstraintList((Production)node);
                case (int)Asn1Constants.CONSTRAINT:
                    return ExitConstraint((Production)node);
                case (int)Asn1Constants.VALUE_CONSTRAINT_LIST:
                    return ExitValueConstraintList((Production)node);
                case (int)Asn1Constants.VALUE_CONSTRAINT:
                    return ExitValueConstraint((Production)node);
                case (int)Asn1Constants.VALUE_RANGE:
                    return ExitValueRange((Production)node);
                case (int)Asn1Constants.LOWER_END_POINT:
                    return ExitLowerEndPoint((Production)node);
                case (int)Asn1Constants.UPPER_END_POINT:
                    return ExitUpperEndPoint((Production)node);
                case (int)Asn1Constants.SIZE_CONSTRAINT:
                    return ExitSizeConstraint((Production)node);
                case (int)Asn1Constants.ALPHABET_CONSTRAINT:
                    return ExitAlphabetConstraint((Production)node);
                case (int)Asn1Constants.CONTAINED_TYPE_CONSTRAINT:
                    return ExitContainedTypeConstraint((Production)node);
                case (int)Asn1Constants.INNER_TYPE_CONSTRAINT:
                    return ExitInnerTypeConstraint((Production)node);
                case (int)Asn1Constants.COMPONENTS_LIST:
                    return ExitComponentsList((Production)node);
                case (int)Asn1Constants.COMPONENTS_LIST_TAIL:
                    return ExitComponentsListTail((Production)node);
                case (int)Asn1Constants.COMPONENT_CONSTRAINT:
                    return ExitComponentConstraint((Production)node);
                case (int)Asn1Constants.COMPONENT_VALUE_PRESENCE:
                    return ExitComponentValuePresence((Production)node);
                case (int)Asn1Constants.COMPONENT_PRESENCE:
                    return ExitComponentPresence((Production)node);
                case (int)Asn1Constants.VALUE_ASSIGNMENT:
                    return ExitValueAssignment((Production)node);
                case (int)Asn1Constants.VALUE:
                    return ExitValue((Production)node);
                case (int)Asn1Constants.DEFINED_VALUE:
                    return ExitDefinedValue((Production)node);
                case (int)Asn1Constants.BUILTIN_VALUE:
                    return ExitBuiltinValue((Production)node);
                case (int)Asn1Constants.NULL_VALUE:
                    return ExitNullValue((Production)node);
                case (int)Asn1Constants.BOOLEAN_VALUE:
                    return ExitBooleanValue((Production)node);
                case (int)Asn1Constants.SPECIAL_REAL_VALUE:
                    return ExitSpecialRealValue((Production)node);
                case (int)Asn1Constants.NUMBER_VALUE:
                    return ExitNumberValue((Production)node);
                case (int)Asn1Constants.BINARY_VALUE:
                    return ExitBinaryValue((Production)node);
                case (int)Asn1Constants.HEXADECIMAL_VALUE:
                    return ExitHexadecimalValue((Production)node);
                case (int)Asn1Constants.STRING_VALUE:
                    return ExitStringValue((Production)node);
                case (int)Asn1Constants.BIT_OR_OBJECT_IDENTIFIER_VALUE:
                    return ExitBitOrObjectIdentifierValue((Production)node);
                case (int)Asn1Constants.BIT_VALUE:
                    return ExitBitValue((Production)node);
                case (int)Asn1Constants.OBJECT_IDENTIFIER_VALUE:
                    return ExitObjectIdentifierValue((Production)node);
                case (int)Asn1Constants.NAME_VALUE_LIST:
                    return ExitNameValueList((Production)node);
                case (int)Asn1Constants.NAME_VALUE_COMPONENT:
                    return ExitNameValueComponent((Production)node);
                case (int)Asn1Constants.NAME_OR_NUMBER:
                    return ExitNameOrNumber((Production)node);
                case (int)Asn1Constants.NAME_AND_NUMBER:
                    return ExitNameAndNumber((Production)node);
                case (int)Asn1Constants.DEFINED_MACRO_TYPE:
                    return ExitDefinedMacroType((Production)node);
                case (int)Asn1Constants.DEFINED_MACRO_NAME:
                    return ExitDefinedMacroName((Production)node);
                case (int)Asn1Constants.SNMP_MODULE_IDENTITY_MACRO_TYPE:
                    return ExitSnmpModuleIdentityMacroType((Production)node);
                case (int)Asn1Constants.SNMP_OBJECT_IDENTITY_MACRO_TYPE:
                    return ExitSnmpObjectIdentityMacroType((Production)node);
                case (int)Asn1Constants.SNMP_OBJECT_TYPE_MACRO_TYPE:
                    return ExitSnmpObjectTypeMacroType((Production)node);
                case (int)Asn1Constants.SNMP_NOTIFICATION_TYPE_MACRO_TYPE:
                    return ExitSnmpNotificationTypeMacroType((Production)node);
                case (int)Asn1Constants.SNMP_TRAP_TYPE_MACRO_TYPE:
                    return ExitSnmpTrapTypeMacroType((Production)node);
                case (int)Asn1Constants.SNMP_TEXTUAL_CONVENTION_MACRO_TYPE:
                    return ExitSnmpTextualConventionMacroType((Production)node);
                case (int)Asn1Constants.SNMP_OBJECT_GROUP_MACRO_TYPE:
                    return ExitSnmpObjectGroupMacroType((Production)node);
                case (int)Asn1Constants.SNMP_NOTIFICATION_GROUP_MACRO_TYPE:
                    return ExitSnmpNotificationGroupMacroType((Production)node);
                case (int)Asn1Constants.SNMP_MODULE_COMPLIANCE_MACRO_TYPE:
                    return ExitSnmpModuleComplianceMacroType((Production)node);
                case (int)Asn1Constants.SNMP_AGENT_CAPABILITIES_MACRO_TYPE:
                    return ExitSnmpAgentCapabilitiesMacroType((Production)node);
                case (int)Asn1Constants.SNMP_UPDATE_PART:
                    return ExitSnmpUpdatePart((Production)node);
                case (int)Asn1Constants.SNMP_ORGANIZATION_PART:
                    return ExitSnmpOrganizationPart((Production)node);
                case (int)Asn1Constants.SNMP_CONTACT_PART:
                    return ExitSnmpContactPart((Production)node);
                case (int)Asn1Constants.SNMP_DESCR_PART:
                    return ExitSnmpDescrPart((Production)node);
                case (int)Asn1Constants.SNMP_REVISION_PART:
                    return ExitSnmpRevisionPart((Production)node);
                case (int)Asn1Constants.SNMP_STATUS_PART:
                    return ExitSnmpStatusPart((Production)node);
                case (int)Asn1Constants.SNMP_REFER_PART:
                    return ExitSnmpReferPart((Production)node);
                case (int)Asn1Constants.SNMP_SYNTAX_PART:
                    return ExitSnmpSyntaxPart((Production)node);
                case (int)Asn1Constants.SNMP_UNITS_PART:
                    return ExitSnmpUnitsPart((Production)node);
                case (int)Asn1Constants.SNMP_ACCESS_PART:
                    return ExitSnmpAccessPart((Production)node);
                case (int)Asn1Constants.SNMP_INDEX_PART:
                    return ExitSnmpIndexPart((Production)node);
                case (int)Asn1Constants.INDEX_VALUE_LIST:
                    return ExitIndexValueList((Production)node);
                case (int)Asn1Constants.INDEX_VALUE:
                    return ExitIndexValue((Production)node);
                case (int)Asn1Constants.INDEX_TYPE:
                    return ExitIndexType((Production)node);
                case (int)Asn1Constants.SNMP_DEF_VAL_PART:
                    return ExitSnmpDefValPart((Production)node);
                case (int)Asn1Constants.SNMP_OBJECTS_PART:
                    return ExitSnmpObjectsPart((Production)node);
                case (int)Asn1Constants.VALUE_LIST:
                    return ExitValueList((Production)node);
                case (int)Asn1Constants.SNMP_ENTERPRISE_PART:
                    return ExitSnmpEnterprisePart((Production)node);
                case (int)Asn1Constants.SNMP_VAR_PART:
                    return ExitSnmpVarPart((Production)node);
                case (int)Asn1Constants.SNMP_DISPLAY_PART:
                    return ExitSnmpDisplayPart((Production)node);
                case (int)Asn1Constants.SNMP_NOTIFICATIONS_PART:
                    return ExitSnmpNotificationsPart((Production)node);
                case (int)Asn1Constants.SNMP_MODULE_PART:
                    return ExitSnmpModulePart((Production)node);
                case (int)Asn1Constants.SNMP_MODULE_IMPORT:
                    return ExitSnmpModuleImport((Production)node);
                case (int)Asn1Constants.SNMP_MANDATORY_PART:
                    return ExitSnmpMandatoryPart((Production)node);
                case (int)Asn1Constants.SNMP_COMPLIANCE_PART:
                    return ExitSnmpCompliancePart((Production)node);
                case (int)Asn1Constants.COMPLIANCE_GROUP:
                    return ExitComplianceGroup((Production)node);
                case (int)Asn1Constants.COMPLIANCE_OBJECT:
                    return ExitComplianceObject((Production)node);
                case (int)Asn1Constants.SNMP_WRITE_SYNTAX_PART:
                    return ExitSnmpWriteSyntaxPart((Production)node);
                case (int)Asn1Constants.SNMP_PRODUCT_RELEASE_PART:
                    return ExitSnmpProductReleasePart((Production)node);
                case (int)Asn1Constants.SNMP_MODULE_SUPPORT_PART:
                    return ExitSnmpModuleSupportPart((Production)node);
                case (int)Asn1Constants.SNMP_VARIATION_PART:
                    return ExitSnmpVariationPart((Production)node);
                case (int)Asn1Constants.SNMP_CREATION_PART:
                    return ExitSnmpCreationPart((Production)node);
            }
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public override void Child(Production node, Node child)
        {
            switch (node.Id)
            {
                case (int)Asn1Constants.START:
                    ChildStart(node, child);
                    break;
                case (int)Asn1Constants.MODULE_DEFINITION:
                    ChildModuleDefinition(node, child);
                    break;
                case (int)Asn1Constants.MODULE_IDENTIFIER:
                    ChildModuleIdentifier(node, child);
                    break;
                case (int)Asn1Constants.MODULE_REFERENCE:
                    ChildModuleReference(node, child);
                    break;
                case (int)Asn1Constants.TAG_DEFAULT:
                    ChildTagDefault(node, child);
                    break;
                case (int)Asn1Constants.MODULE_BODY:
                    ChildModuleBody(node, child);
                    break;
                case (int)Asn1Constants.EXPORT_LIST:
                    ChildExportList(node, child);
                    break;
                case (int)Asn1Constants.IMPORT_LIST:
                    ChildImportList(node, child);
                    break;
                case (int)Asn1Constants.SYMBOLS_FROM_MODULE:
                    ChildSymbolsFromModule(node, child);
                    break;
                case (int)Asn1Constants.SYMBOL_LIST:
                    ChildSymbolList(node, child);
                    break;
                case (int)Asn1Constants.SYMBOL:
                    ChildSymbol(node, child);
                    break;
                case (int)Asn1Constants.ASSIGNMENT_LIST:
                    ChildAssignmentList(node, child);
                    break;
                case (int)Asn1Constants.ASSIGNMENT:
                    ChildAssignment(node, child);
                    break;
                case (int)Asn1Constants.MACRO_DEFINITION:
                    ChildMacroDefinition(node, child);
                    break;
                case (int)Asn1Constants.MACRO_REFERENCE:
                    ChildMacroReference(node, child);
                    break;
                case (int)Asn1Constants.MACRO_BODY:
                    ChildMacroBody(node, child);
                    break;
                case (int)Asn1Constants.MACRO_BODY_ELEMENT:
                    ChildMacroBodyElement(node, child);
                    break;
                case (int)Asn1Constants.TYPE_ASSIGNMENT:
                    ChildTypeAssignment(node, child);
                    break;
                case (int)Asn1Constants.TYPE:
                    ChildType(node, child);
                    break;
                case (int)Asn1Constants.DEFINED_TYPE:
                    ChildDefinedType(node, child);
                    break;
                case (int)Asn1Constants.BUILTIN_TYPE:
                    ChildBuiltinType(node, child);
                    break;
                case (int)Asn1Constants.NULL_TYPE:
                    ChildNullType(node, child);
                    break;
                case (int)Asn1Constants.BOOLEAN_TYPE:
                    ChildBooleanType(node, child);
                    break;
                case (int)Asn1Constants.REAL_TYPE:
                    ChildRealType(node, child);
                    break;
                case (int)Asn1Constants.INTEGER_TYPE:
                    ChildIntegerType(node, child);
                    break;
                case (int)Asn1Constants.OBJECT_IDENTIFIER_TYPE:
                    ChildObjectIdentifierType(node, child);
                    break;
                case (int)Asn1Constants.STRING_TYPE:
                    ChildStringType(node, child);
                    break;
                case (int)Asn1Constants.BIT_STRING_TYPE:
                    ChildBitStringType(node, child);
                    break;
                case (int)Asn1Constants.BITS_TYPE:
                    ChildBitsType(node, child);
                    break;
                case (int)Asn1Constants.SEQUENCE_TYPE:
                    ChildSequenceType(node, child);
                    break;
                case (int)Asn1Constants.SEQUENCE_OF_TYPE:
                    ChildSequenceOfType(node, child);
                    break;
                case (int)Asn1Constants.SET_TYPE:
                    ChildSetType(node, child);
                    break;
                case (int)Asn1Constants.SET_OF_TYPE:
                    ChildSetOfType(node, child);
                    break;
                case (int)Asn1Constants.CHOICE_TYPE:
                    ChildChoiceType(node, child);
                    break;
                case (int)Asn1Constants.ENUMERATED_TYPE:
                    ChildEnumeratedType(node, child);
                    break;
                case (int)Asn1Constants.SELECTION_TYPE:
                    ChildSelectionType(node, child);
                    break;
                case (int)Asn1Constants.TAGGED_TYPE:
                    ChildTaggedType(node, child);
                    break;
                case (int)Asn1Constants.TAG:
                    ChildTag(node, child);
                    break;
                case (int)Asn1Constants.CLASS:
                    ChildClass(node, child);
                    break;
                case (int)Asn1Constants.EXPLICIT_OR_IMPLICIT_TAG:
                    ChildExplicitOrImplicitTag(node, child);
                    break;
                case (int)Asn1Constants.ANY_TYPE:
                    ChildAnyType(node, child);
                    break;
                case (int)Asn1Constants.ELEMENT_TYPE_LIST:
                    ChildElementTypeList(node, child);
                    break;
                case (int)Asn1Constants.ELEMENT_TYPE:
                    ChildElementType(node, child);
                    break;
                case (int)Asn1Constants.OPTIONAL_OR_DEFAULT_ELEMENT:
                    ChildOptionalOrDefaultElement(node, child);
                    break;
                case (int)Asn1Constants.VALUE_OR_CONSTRAINT_LIST:
                    ChildValueOrConstraintList(node, child);
                    break;
                case (int)Asn1Constants.NAMED_NUMBER_LIST:
                    ChildNamedNumberList(node, child);
                    break;
                case (int)Asn1Constants.NAMED_NUMBER:
                    ChildNamedNumber(node, child);
                    break;
                case (int)Asn1Constants.NUMBER:
                    ChildNumber(node, child);
                    break;
                case (int)Asn1Constants.CONSTRAINT_LIST:
                    ChildConstraintList(node, child);
                    break;
                case (int)Asn1Constants.CONSTRAINT:
                    ChildConstraint(node, child);
                    break;
                case (int)Asn1Constants.VALUE_CONSTRAINT_LIST:
                    ChildValueConstraintList(node, child);
                    break;
                case (int)Asn1Constants.VALUE_CONSTRAINT:
                    ChildValueConstraint(node, child);
                    break;
                case (int)Asn1Constants.VALUE_RANGE:
                    ChildValueRange(node, child);
                    break;
                case (int)Asn1Constants.LOWER_END_POINT:
                    ChildLowerEndPoint(node, child);
                    break;
                case (int)Asn1Constants.UPPER_END_POINT:
                    ChildUpperEndPoint(node, child);
                    break;
                case (int)Asn1Constants.SIZE_CONSTRAINT:
                    ChildSizeConstraint(node, child);
                    break;
                case (int)Asn1Constants.ALPHABET_CONSTRAINT:
                    ChildAlphabetConstraint(node, child);
                    break;
                case (int)Asn1Constants.CONTAINED_TYPE_CONSTRAINT:
                    ChildContainedTypeConstraint(node, child);
                    break;
                case (int)Asn1Constants.INNER_TYPE_CONSTRAINT:
                    ChildInnerTypeConstraint(node, child);
                    break;
                case (int)Asn1Constants.COMPONENTS_LIST:
                    ChildComponentsList(node, child);
                    break;
                case (int)Asn1Constants.COMPONENTS_LIST_TAIL:
                    ChildComponentsListTail(node, child);
                    break;
                case (int)Asn1Constants.COMPONENT_CONSTRAINT:
                    ChildComponentConstraint(node, child);
                    break;
                case (int)Asn1Constants.COMPONENT_VALUE_PRESENCE:
                    ChildComponentValuePresence(node, child);
                    break;
                case (int)Asn1Constants.COMPONENT_PRESENCE:
                    ChildComponentPresence(node, child);
                    break;
                case (int)Asn1Constants.VALUE_ASSIGNMENT:
                    ChildValueAssignment(node, child);
                    break;
                case (int)Asn1Constants.VALUE:
                    ChildValue(node, child);
                    break;
                case (int)Asn1Constants.DEFINED_VALUE:
                    ChildDefinedValue(node, child);
                    break;
                case (int)Asn1Constants.BUILTIN_VALUE:
                    ChildBuiltinValue(node, child);
                    break;
                case (int)Asn1Constants.NULL_VALUE:
                    ChildNullValue(node, child);
                    break;
                case (int)Asn1Constants.BOOLEAN_VALUE:
                    ChildBooleanValue(node, child);
                    break;
                case (int)Asn1Constants.SPECIAL_REAL_VALUE:
                    ChildSpecialRealValue(node, child);
                    break;
                case (int)Asn1Constants.NUMBER_VALUE:
                    ChildNumberValue(node, child);
                    break;
                case (int)Asn1Constants.BINARY_VALUE:
                    ChildBinaryValue(node, child);
                    break;
                case (int)Asn1Constants.HEXADECIMAL_VALUE:
                    ChildHexadecimalValue(node, child);
                    break;
                case (int)Asn1Constants.STRING_VALUE:
                    ChildStringValue(node, child);
                    break;
                case (int)Asn1Constants.BIT_OR_OBJECT_IDENTIFIER_VALUE:
                    ChildBitOrObjectIdentifierValue(node, child);
                    break;
                case (int)Asn1Constants.BIT_VALUE:
                    ChildBitValue(node, child);
                    break;
                case (int)Asn1Constants.OBJECT_IDENTIFIER_VALUE:
                    ChildObjectIdentifierValue(node, child);
                    break;
                case (int)Asn1Constants.NAME_VALUE_LIST:
                    ChildNameValueList(node, child);
                    break;
                case (int)Asn1Constants.NAME_VALUE_COMPONENT:
                    ChildNameValueComponent(node, child);
                    break;
                case (int)Asn1Constants.NAME_OR_NUMBER:
                    ChildNameOrNumber(node, child);
                    break;
                case (int)Asn1Constants.NAME_AND_NUMBER:
                    ChildNameAndNumber(node, child);
                    break;
                case (int)Asn1Constants.DEFINED_MACRO_TYPE:
                    ChildDefinedMacroType(node, child);
                    break;
                case (int)Asn1Constants.DEFINED_MACRO_NAME:
                    ChildDefinedMacroName(node, child);
                    break;
                case (int)Asn1Constants.SNMP_MODULE_IDENTITY_MACRO_TYPE:
                    ChildSnmpModuleIdentityMacroType(node, child);
                    break;
                case (int)Asn1Constants.SNMP_OBJECT_IDENTITY_MACRO_TYPE:
                    ChildSnmpObjectIdentityMacroType(node, child);
                    break;
                case (int)Asn1Constants.SNMP_OBJECT_TYPE_MACRO_TYPE:
                    ChildSnmpObjectTypeMacroType(node, child);
                    break;
                case (int)Asn1Constants.SNMP_NOTIFICATION_TYPE_MACRO_TYPE:
                    ChildSnmpNotificationTypeMacroType(node, child);
                    break;
                case (int)Asn1Constants.SNMP_TRAP_TYPE_MACRO_TYPE:
                    ChildSnmpTrapTypeMacroType(node, child);
                    break;
                case (int)Asn1Constants.SNMP_TEXTUAL_CONVENTION_MACRO_TYPE:
                    ChildSnmpTextualConventionMacroType(node, child);
                    break;
                case (int)Asn1Constants.SNMP_OBJECT_GROUP_MACRO_TYPE:
                    ChildSnmpObjectGroupMacroType(node, child);
                    break;
                case (int)Asn1Constants.SNMP_NOTIFICATION_GROUP_MACRO_TYPE:
                    ChildSnmpNotificationGroupMacroType(node, child);
                    break;
                case (int)Asn1Constants.SNMP_MODULE_COMPLIANCE_MACRO_TYPE:
                    ChildSnmpModuleComplianceMacroType(node, child);
                    break;
                case (int)Asn1Constants.SNMP_AGENT_CAPABILITIES_MACRO_TYPE:
                    ChildSnmpAgentCapabilitiesMacroType(node, child);
                    break;
                case (int)Asn1Constants.SNMP_UPDATE_PART:
                    ChildSnmpUpdatePart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_ORGANIZATION_PART:
                    ChildSnmpOrganizationPart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_CONTACT_PART:
                    ChildSnmpContactPart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_DESCR_PART:
                    ChildSnmpDescrPart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_REVISION_PART:
                    ChildSnmpRevisionPart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_STATUS_PART:
                    ChildSnmpStatusPart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_REFER_PART:
                    ChildSnmpReferPart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_SYNTAX_PART:
                    ChildSnmpSyntaxPart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_UNITS_PART:
                    ChildSnmpUnitsPart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_ACCESS_PART:
                    ChildSnmpAccessPart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_INDEX_PART:
                    ChildSnmpIndexPart(node, child);
                    break;
                case (int)Asn1Constants.INDEX_VALUE_LIST:
                    ChildIndexValueList(node, child);
                    break;
                case (int)Asn1Constants.INDEX_VALUE:
                    ChildIndexValue(node, child);
                    break;
                case (int)Asn1Constants.INDEX_TYPE:
                    ChildIndexType(node, child);
                    break;
                case (int)Asn1Constants.SNMP_DEF_VAL_PART:
                    ChildSnmpDefValPart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_OBJECTS_PART:
                    ChildSnmpObjectsPart(node, child);
                    break;
                case (int)Asn1Constants.VALUE_LIST:
                    ChildValueList(node, child);
                    break;
                case (int)Asn1Constants.SNMP_ENTERPRISE_PART:
                    ChildSnmpEnterprisePart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_VAR_PART:
                    ChildSnmpVarPart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_DISPLAY_PART:
                    ChildSnmpDisplayPart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_NOTIFICATIONS_PART:
                    ChildSnmpNotificationsPart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_MODULE_PART:
                    ChildSnmpModulePart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_MODULE_IMPORT:
                    ChildSnmpModuleImport(node, child);
                    break;
                case (int)Asn1Constants.SNMP_MANDATORY_PART:
                    ChildSnmpMandatoryPart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_COMPLIANCE_PART:
                    ChildSnmpCompliancePart(node, child);
                    break;
                case (int)Asn1Constants.COMPLIANCE_GROUP:
                    ChildComplianceGroup(node, child);
                    break;
                case (int)Asn1Constants.COMPLIANCE_OBJECT:
                    ChildComplianceObject(node, child);
                    break;
                case (int)Asn1Constants.SNMP_WRITE_SYNTAX_PART:
                    ChildSnmpWriteSyntaxPart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_PRODUCT_RELEASE_PART:
                    ChildSnmpProductReleasePart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_MODULE_SUPPORT_PART:
                    ChildSnmpModuleSupportPart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_VARIATION_PART:
                    ChildSnmpVariationPart(node, child);
                    break;
                case (int)Asn1Constants.SNMP_CREATION_PART:
                    ChildSnmpCreationPart(node, child);
                    break;
            }
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterDot(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitDot(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterDoubleDot(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitDoubleDot(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterTripleDot(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitTripleDot(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterComma(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitComma(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSemiColon(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSemiColon(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterLeftParen(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitLeftParen(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterRightParen(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitRightParen(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterLeftBrace(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitLeftBrace(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterRightBrace(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitRightBrace(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterLeftBracket(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitLeftBracket(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterRightBracket(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitRightBracket(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterMinus(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitMinus(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterLessThan(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitLessThan(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterVerticalBar(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitVerticalBar(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterDefinition(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitDefinition(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterDefinitions(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitDefinitions(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterExplicit(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitExplicit(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterImplicit(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitImplicit(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterTags(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitTags(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterBegin(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitBegin(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterEnd(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitEnd(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterExports(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitExports(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterImports(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitImports(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterFrom(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitFrom(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterMacro(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitMacro(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterInteger(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitInteger(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterReal(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitReal(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterBoolean(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitBoolean(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterNull(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitNull(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterBit(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitBit(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterOctet(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitOctet(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterString(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitString(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterEnumerated(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitEnumerated(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSequence(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSequence(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSet(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSet(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterOf(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitOf(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterChoice(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitChoice(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterUniversal(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitUniversal(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterApplication(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitApplication(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterPrivate(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitPrivate(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterAny(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitAny(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterDefined(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitDefined(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterBy(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitBy(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterObject(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitObject(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterIdentifier(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitIdentifier(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterIncludes(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitIncludes(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterMin(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitMin(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterMax(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitMax(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSize(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSize(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterWith(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitWith(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterComponent(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitComponent(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterComponents(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitComponents(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterPresent(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitPresent(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterAbsent(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitAbsent(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterOptional(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitOptional(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterDefault(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitDefault(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterTrue(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitTrue(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterFalse(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitFalse(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterPlusInfinity(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitPlusInfinity(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterMinusInfinity(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitMinusInfinity(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterModuleIdentity(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitModuleIdentity(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterObjectIdentity(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitObjectIdentity(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterObjectType(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitObjectType(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterNotificationType(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitNotificationType(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterTrapType(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitTrapType(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterTextualConvention(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitTextualConvention(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterObjectGroup(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitObjectGroup(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterNotificationGroup(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitNotificationGroup(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterModuleCompliance(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitModuleCompliance(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterAgentCapabilities(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitAgentCapabilities(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterLastUpdated(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitLastUpdated(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterOrganization(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitOrganization(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterContactInfo(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitContactInfo(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterDescription(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitDescription(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterRevision(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitRevision(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterStatus(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitStatus(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterReference(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitReference(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSyntax(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSyntax(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterBits(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitBits(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterUnits(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitUnits(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterAccess(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitAccess(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterMaxAccess(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitMaxAccess(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterMinAccess(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitMinAccess(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterIndex(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitIndex(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterAugments(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitAugments(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterImplied(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitImplied(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterDefval(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitDefval(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterObjects(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitObjects(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterEnterprise(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitEnterprise(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterVariables(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitVariables(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterDisplayHint(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitDisplayHint(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterNotifications(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitNotifications(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterModule(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitModule(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterMandatoryGroups(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitMandatoryGroups(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterGroup(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitGroup(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterWriteSyntax(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitWriteSyntax(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterProductRelease(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitProductRelease(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSupports(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSupports(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterVariation(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitVariation(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterCreationRequires(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitCreationRequires(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterBinaryString(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitBinaryString(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterHexadecimalString(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitHexadecimalString(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterQuotedString(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitQuotedString(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterIdentifierString(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitIdentifierString(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterNumberString(Token node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitNumberString(Token node)
        {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterStart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitStart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildStart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterModuleDefinition(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitModuleDefinition(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildModuleDefinition(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterModuleIdentifier(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitModuleIdentifier(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildModuleIdentifier(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterModuleReference(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitModuleReference(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildModuleReference(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterTagDefault(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitTagDefault(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildTagDefault(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterModuleBody(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitModuleBody(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildModuleBody(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterExportList(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitExportList(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildExportList(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterImportList(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitImportList(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildImportList(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSymbolsFromModule(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSymbolsFromModule(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSymbolsFromModule(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSymbolList(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSymbolList(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSymbolList(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSymbol(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSymbol(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSymbol(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterAssignmentList(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitAssignmentList(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildAssignmentList(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterAssignment(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitAssignment(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildAssignment(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterMacroDefinition(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitMacroDefinition(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildMacroDefinition(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterMacroReference(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitMacroReference(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildMacroReference(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterMacroBody(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitMacroBody(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildMacroBody(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterMacroBodyElement(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitMacroBodyElement(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildMacroBodyElement(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterTypeAssignment(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitTypeAssignment(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildTypeAssignment(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterDefinedType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitDefinedType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildDefinedType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterBuiltinType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitBuiltinType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildBuiltinType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterNullType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitNullType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildNullType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterBooleanType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitBooleanType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildBooleanType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterRealType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitRealType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildRealType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterIntegerType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitIntegerType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildIntegerType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterObjectIdentifierType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitObjectIdentifierType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildObjectIdentifierType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterStringType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitStringType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildStringType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterBitStringType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitBitStringType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildBitStringType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterBitsType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitBitsType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildBitsType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSequenceType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSequenceType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSequenceType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSequenceOfType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSequenceOfType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSequenceOfType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSetType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSetType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSetType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSetOfType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSetOfType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSetOfType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterChoiceType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitChoiceType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildChoiceType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterEnumeratedType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitEnumeratedType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildEnumeratedType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSelectionType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSelectionType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSelectionType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterTaggedType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitTaggedType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildTaggedType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterTag(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitTag(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildTag(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterClass(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitClass(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildClass(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterExplicitOrImplicitTag(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitExplicitOrImplicitTag(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildExplicitOrImplicitTag(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterAnyType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitAnyType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildAnyType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterElementTypeList(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitElementTypeList(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildElementTypeList(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterElementType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitElementType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildElementType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterOptionalOrDefaultElement(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitOptionalOrDefaultElement(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildOptionalOrDefaultElement(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterValueOrConstraintList(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitValueOrConstraintList(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildValueOrConstraintList(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterNamedNumberList(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitNamedNumberList(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildNamedNumberList(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterNamedNumber(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitNamedNumber(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildNamedNumber(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterNumber(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitNumber(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildNumber(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterConstraintList(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitConstraintList(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildConstraintList(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterConstraint(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitConstraint(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildConstraint(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterValueConstraintList(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitValueConstraintList(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildValueConstraintList(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterValueConstraint(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitValueConstraint(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildValueConstraint(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterValueRange(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitValueRange(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildValueRange(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterLowerEndPoint(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitLowerEndPoint(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildLowerEndPoint(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterUpperEndPoint(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitUpperEndPoint(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildUpperEndPoint(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSizeConstraint(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSizeConstraint(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSizeConstraint(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterAlphabetConstraint(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitAlphabetConstraint(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildAlphabetConstraint(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterContainedTypeConstraint(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitContainedTypeConstraint(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildContainedTypeConstraint(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterInnerTypeConstraint(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitInnerTypeConstraint(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildInnerTypeConstraint(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterComponentsList(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitComponentsList(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildComponentsList(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterComponentsListTail(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitComponentsListTail(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildComponentsListTail(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterComponentConstraint(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitComponentConstraint(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildComponentConstraint(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterComponentValuePresence(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitComponentValuePresence(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildComponentValuePresence(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterComponentPresence(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitComponentPresence(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildComponentPresence(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterValueAssignment(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitValueAssignment(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildValueAssignment(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterValue(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitValue(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildValue(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterDefinedValue(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitDefinedValue(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildDefinedValue(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterBuiltinValue(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitBuiltinValue(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildBuiltinValue(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterNullValue(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitNullValue(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildNullValue(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterBooleanValue(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitBooleanValue(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildBooleanValue(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSpecialRealValue(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSpecialRealValue(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSpecialRealValue(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterNumberValue(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitNumberValue(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildNumberValue(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterBinaryValue(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitBinaryValue(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildBinaryValue(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterHexadecimalValue(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitHexadecimalValue(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildHexadecimalValue(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterStringValue(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitStringValue(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildStringValue(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterBitOrObjectIdentifierValue(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitBitOrObjectIdentifierValue(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildBitOrObjectIdentifierValue(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterBitValue(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitBitValue(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildBitValue(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterObjectIdentifierValue(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitObjectIdentifierValue(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildObjectIdentifierValue(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterNameValueList(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitNameValueList(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildNameValueList(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterNameValueComponent(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitNameValueComponent(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildNameValueComponent(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterNameOrNumber(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitNameOrNumber(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildNameOrNumber(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterNameAndNumber(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitNameAndNumber(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildNameAndNumber(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterDefinedMacroType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitDefinedMacroType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildDefinedMacroType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterDefinedMacroName(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitDefinedMacroName(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildDefinedMacroName(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpModuleIdentityMacroType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpModuleIdentityMacroType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpModuleIdentityMacroType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpObjectIdentityMacroType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpObjectIdentityMacroType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpObjectIdentityMacroType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpObjectTypeMacroType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpObjectTypeMacroType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpObjectTypeMacroType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpNotificationTypeMacroType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpNotificationTypeMacroType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpNotificationTypeMacroType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpTrapTypeMacroType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpTrapTypeMacroType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpTrapTypeMacroType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpTextualConventionMacroType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpTextualConventionMacroType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpTextualConventionMacroType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpObjectGroupMacroType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpObjectGroupMacroType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpObjectGroupMacroType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpNotificationGroupMacroType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpNotificationGroupMacroType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpNotificationGroupMacroType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpModuleComplianceMacroType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpModuleComplianceMacroType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpModuleComplianceMacroType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpAgentCapabilitiesMacroType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpAgentCapabilitiesMacroType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpAgentCapabilitiesMacroType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpUpdatePart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpUpdatePart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpUpdatePart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpOrganizationPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpOrganizationPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpOrganizationPart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpContactPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpContactPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpContactPart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpDescrPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpDescrPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpDescrPart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpRevisionPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpRevisionPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpRevisionPart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpStatusPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpStatusPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpStatusPart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpReferPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpReferPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpReferPart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpSyntaxPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpSyntaxPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpSyntaxPart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpUnitsPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpUnitsPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpUnitsPart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpAccessPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpAccessPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpAccessPart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpIndexPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpIndexPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpIndexPart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterIndexValueList(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitIndexValueList(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildIndexValueList(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterIndexValue(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitIndexValue(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildIndexValue(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterIndexType(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitIndexType(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildIndexType(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpDefValPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpDefValPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpDefValPart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpObjectsPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpObjectsPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpObjectsPart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterValueList(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitValueList(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildValueList(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpEnterprisePart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpEnterprisePart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpEnterprisePart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpVarPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpVarPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpVarPart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpDisplayPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpDisplayPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpDisplayPart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpNotificationsPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpNotificationsPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpNotificationsPart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpModulePart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpModulePart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpModulePart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpModuleImport(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpModuleImport(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpModuleImport(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpMandatoryPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpMandatoryPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpMandatoryPart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpCompliancePart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpCompliancePart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpCompliancePart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterComplianceGroup(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitComplianceGroup(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildComplianceGroup(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterComplianceObject(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitComplianceObject(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildComplianceObject(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpWriteSyntaxPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpWriteSyntaxPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpWriteSyntaxPart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpProductReleasePart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpProductReleasePart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpProductReleasePart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpModuleSupportPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpModuleSupportPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpModuleSupportPart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpVariationPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpVariationPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpVariationPart(Production node, Node child)
        {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         *
         * <param name='node'>the node being entered</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterSnmpCreationPart(Production node)
        {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         *
         * <param name='node'>the node being exited</param>
         *
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitSnmpCreationPart(Production node)
        {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         *
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         *
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildSnmpCreationPart(Production node, Node child)
        {
            node.AddChild(child);
        }
    }
}
