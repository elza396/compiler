using System;
using System.Collections.Generic;

namespace Compiler
{
    public class Syntaxis
    {
        static HashSet<byte> followers = new HashSet<byte>();
        static bool ifStart = true;
        static bool ifFlag;
        private static int beginEndCount;
        // public static int count;
        static CToken cur_token;
        private static Dictionary<string, CIdentToken> identifiers = new Dictionary<string, CIdentToken>();

        static bool Belong(byte element, HashSet<byte> set)
        {
            return set.Contains(element);
        }

        static void SkipTo(HashSet<byte> where)
        {
            while (!Belong(cur_token.Code, where))
            {
                cur_token = LexicalAnalyzer.NextSym();
            }
        }
        static void SkipTo2(HashSet<byte> start, HashSet<byte> follow)
        {
            while (!(Belong(cur_token.Code, start) || Belong(cur_token.Code, follow)))
            {
                cur_token = LexicalAnalyzer.NextSym();
            }
        }

        static void SetDisjunct(HashSet<byte> set1, HashSet<byte> set2, out HashSet<byte> set3)
        {
            set3 = new HashSet<byte>();
            set3.UnionWith(set2);
            set3.UnionWith(set1);
        }

        static void Accept(byte symbolExpected)
        {
            if (cur_token.Code == symbolExpected)
            {
                cur_token = LexicalAnalyzer.NextSym();
            }
            else
            {
                InputOutput.Error(symbolExpected, LexicalAnalyzer.token);
            }
        }

        static void Block(HashSet<byte> followers)
        {
            HashSet<byte> ptra;
            StFoll obj = new StFoll();
            if (!Belong(cur_token.Code, obj.sf[StFoll.begpart]))
            {
                InputOutput.Error(18); // ошибка в разделе описаний
                SkipTo2(obj.sf[StFoll.begpart], followers);
            }
            if (Belong(cur_token.Code, obj.sf[StFoll.begpart]))
            {
                LabelPart();
                SetDisjunct(obj.sf[StFoll.st_typepart], followers, out ptra);
                ConstPart(ptra);
                SetDisjunct(obj.sf[StFoll.st_varpart], followers, out ptra);
                // TypePart(ptra);
                SetDisjunct(obj.sf[StFoll.st_procfuncpart], followers, out ptra);
                VarPart(ptra);
                // ProcFuncPart(ptra);
                StatPart(followers);
                if (!Belong(cur_token.Code, followers))
                {
                    InputOutput.Error(6); // запрещенный символ
                    SkipTo(followers);
                }
            }
        }

        static void LabelPart() // раздел меток
        {
            if (cur_token.Code == LexicalAnalyzer.labelsy)
            {
                Accept(LexicalAnalyzer.labelsy);
                if (cur_token.Code == LexicalAnalyzer.intc || cur_token.Code == LexicalAnalyzer.ident)
                {
                    cur_token = LexicalAnalyzer.NextSym();
                }
                else
                {
                    InputOutput.Error(26, LexicalAnalyzer.token);
                }

                while (cur_token.Code == LexicalAnalyzer.comma)
                {
                    cur_token = LexicalAnalyzer.NextSym();
                    if (cur_token.Code == LexicalAnalyzer.intc || cur_token.Code == LexicalAnalyzer.ident)
                    {
                        cur_token = LexicalAnalyzer.NextSym();
                    }
                    else
                    {
                        InputOutput.Error(26, LexicalAnalyzer.token);
                    }
                }
                Accept(LexicalAnalyzer.semicolon);
            }
        }

        static void ConstPart(HashSet<byte> followers) //раздел констант
        {
            if (cur_token.Code == LexicalAnalyzer.constsy)
            {
                Accept(LexicalAnalyzer.constsy);
                Accept(LexicalAnalyzer.ident);
                Accept(LexicalAnalyzer.equal);
                if (cur_token.Code == LexicalAnalyzer.intc ||
                    cur_token.Code == LexicalAnalyzer.floatc ||
                    cur_token.Code == LexicalAnalyzer.charc)
                {
                    cur_token = LexicalAnalyzer.NextSym();
                }

                while (cur_token.Code == LexicalAnalyzer.semicolon)
                {
                    cur_token = LexicalAnalyzer.NextSym();
                    if (cur_token.Code == LexicalAnalyzer.typesy || cur_token.Code == LexicalAnalyzer.varsy)
                    {
                        break;
                    }
                    Accept(LexicalAnalyzer.ident);
                    Accept(LexicalAnalyzer.equal);
                    if (cur_token.Code == LexicalAnalyzer.intc ||
                        cur_token.Code == LexicalAnalyzer.floatc ||
                        cur_token.Code == LexicalAnalyzer.charc)
                    {
                        cur_token = LexicalAnalyzer.NextSym();
                    }
                    else
                    {
                        InputOutput.Error(26, LexicalAnalyzer.token);
                    }

                }
                Accept(LexicalAnalyzer.semicolon);
            }
        }

        static void VarPart(HashSet<byte> followers)
        {
            HashSet<byte> ptra; // храним внешние символы
            StFoll obj = new StFoll();
            if (!Belong(cur_token.Code, obj.sf[StFoll.st_varpart]))
            {
                InputOutput.Error(18); // ошибка в разделе описаний
                SkipTo2(obj.sf[StFoll.st_varpart], followers);
            }
            if (cur_token.Code == LexicalAnalyzer.varsy)
            {
                cur_token = LexicalAnalyzer.NextSym();
                SetDisjunct(obj.sf[StFoll.after_var], followers, out ptra);
                do
                {
                    VarDeclaration(ptra);
                    Accept(LexicalAnalyzer.semicolon);
                }
                while (cur_token.Code == LexicalAnalyzer.ident);
                if (!Belong(cur_token.Code, followers))
                {
                    InputOutput.Error(6); // запрещенный символ
                    SkipTo(followers);
                }
            }
        }

        static void VarDeclaration(HashSet<byte> followers)
        {
            StFoll obj = new StFoll();
            if (!Belong(cur_token.Code, obj.sf[StFoll.id_starters]))
            {
                InputOutput.Error(2); // должно идти имя
                SkipTo2(obj.sf[StFoll.id_starters], followers);
            }
            List<CIdentToken> current_variables = new List<CIdentToken>();
            if (cur_token.Code == LexicalAnalyzer.ident)
            {
                string cur_name = ((CIdentToken)cur_token).Name;
                if (identifiers.ContainsKey(cur_name))
                {
                    // ошибка 101: имя описано повторно
                    InputOutput.Error(101, cur_token.Position);
                }
                else
                {
                    current_variables.Add((CIdentToken)cur_token);
                }
                Accept(LexicalAnalyzer.ident);

                while (cur_token.Code == LexicalAnalyzer.comma)
                {
                    cur_token = LexicalAnalyzer.NextSym();
                    cur_name = ((CIdentToken)cur_token).Name;
                    if (identifiers.ContainsKey(cur_name) || current_variables.Find(x => x.Name == cur_name) != null)
                        // ошибка 101: имя описано повторно
                        InputOutput.Error(101, cur_token.Position);
                    else
                        current_variables.Add((CIdentToken)cur_token);
                    Accept(LexicalAnalyzer.ident);
                }
                Accept(LexicalAnalyzer.colon);

                // InputOutput.varType = Typе(followers);
                // if (cur_token.Code == LexicalAnalyzer.arraysy)
                // {
                //     // SemanticAnalyzer.tempIdTypeArray = SemanticAnalyzer.arrays; // тип идентификаторов массив
                //     ArrayType(followers); // массив
                // }
                // else Type(followers);
                VariableType current_type = Type(followers);
                foreach (CIdentToken token in current_variables)
                {
                    token.Variable_type = current_type;
                    identifiers.Add(token.Name, token);
                }

                if (!Belong(cur_token.Code, followers))
                {
                    InputOutput.Error(6); // запрещенный символ
                    SkipTo(followers);
                }
            }
        }

        static VariableType Type(HashSet<byte> followers) // простые типы
        {
            StFoll obj = new StFoll();
            VariableType result = VariableType.vartUndef;
            if (!Belong(cur_token.Code, obj.sf[StFoll.types]))
            {
                InputOutput.Error(10);
                SkipTo2(obj.sf[StFoll.types], followers);
            }
            else
            {
                if (cur_token.Code == LexicalAnalyzer.integersy)
                {
                    result = VariableType.vartInteger;
                    Accept(LexicalAnalyzer.integersy);
                }
                else if (cur_token.Code == LexicalAnalyzer.floatsy)
                {
                    result = VariableType.vartFloat;
                    Accept(LexicalAnalyzer.floatsy);
                }
                else if (cur_token.Code == LexicalAnalyzer.booleansy)
                {
                    result = VariableType.vartBoolean;
                    Accept(LexicalAnalyzer.booleansy);
                }
                else
                {
                    result = VariableType.vartString;
                    Accept(LexicalAnalyzer.stringsy);
                }
            }

            return result;
        }

        // static void ArrayType(HashSet<byte> followers)
        // {
        //     Accept(LexicalAnalyzer.arraysy);
        //     Accept(LexicalAnalyzer.lbracket);
        //     SimpleType();
        //     while (cur_token.Code == LexicalAnalyzer.comma)
        //     {
        //         Accept(LexicalAnalyzer.comma);
        //         SimpleType();
        //     }
        //     Accept(LexicalAnalyzer.rbracket);
        //     Accept(LexicalAnalyzer.ofsy);
        //     Type(followers);
        // }
        //
        // static void SimpleType()
        // {
        //     StFoll obj = new StFoll();
        //     if (!Belong(cur_token.Code, obj.sf[StFoll.id_starters]))
        //     {
        //         cur_token = LexicalAnalyzer.NextSym();
        //     }
        //     else
        //     {
        //         Accept(LexicalAnalyzer.intc);
        //     }
        // }

        static void StatPart(HashSet<byte> followers) // анализ конструкции <оператор>
        {
            // if (cur_token.Code == LexicalAnalyzer.intc) // семантический анализ метки TODO доделать!
            // {
            //     cur_token = LexicalAnalyzer.NextSym();
            //     Accept(LexicalAnalyzer.colon);
            // }
            switch (cur_token.Code)
            {
                case LexicalAnalyzer.ident: // анализ оператора присваивания
                    Assignment(followers);
                    // else
                    //     // анализ вызова процедуры
                    //     CallProc(followers);
                    break;
                case LexicalAnalyzer.beginsy:
                    beginEndCount++;
                    cur_token = LexicalAnalyzer.NextSym();
                    StatPart(followers);
                    Accept(LexicalAnalyzer.endsy);
                    beginEndCount--;
                    if (beginEndCount == 0)
                    {
                        Accept(LexicalAnalyzer.point);
                    }
                    else
                    {
                        Accept(LexicalAnalyzer.semicolon);
                    }
                    break;
                case LexicalAnalyzer.ifsy:
                    IfStatement(followers);
                    break;
                // case LexicalAnalyzer.whilesy:
                //     WhileStatement(followers); break;
                // case LexicalAnalyzer.repeatsy:
                //     RepeatStatement(followers); break;
                // case LexicalAnalyzer.forsy:
                //     ForStatement(followers); break;
                // case LexicalAnalyzer.casesy:
                // CaseStatement(followers); break;
                // case LexicalAnalyzer.withsy:
                //     WithStatement(followers); break;
                // case LexicalAnalyzer.gotosy: TODO нужно доделать
                //     GotoStatement(followers); break;
            }
        }

        static void IfStatement(HashSet<byte> followers) // условный оператор
        {
            if (cur_token.Code == LexicalAnalyzer.ifsy)
            {
                Accept(LexicalAnalyzer.ifsy);
                ifFlag = true;
                TextPosition expr_start = cur_token.Position;
                followers.Add(LexicalAnalyzer.thensy);
                bool is_logic = VariableType.vartBoolean == Expression(followers);
                if (!is_logic)
                    // ошибка 135: тип операнда должен быть BOOLEAN
                    InputOutput.Error(135, expr_start);

                Accept(LexicalAnalyzer.thensy);
                followers.Add(LexicalAnalyzer.elsesy);
                StatPart(followers);

                if (cur_token.Code == LexicalAnalyzer.elsesy)
                {
                    Accept(LexicalAnalyzer.elsesy);
                    IfStatement(followers);
                    Assignment(followers);
                }
                else StatPart(followers); // если это не else, то чтобы не потерять строку рекурсивно запускаем
            }
        }

        // <оператор присваивания>::=<переменная>:=<выражение>| <имя функции>:=<выражение>
        static void Assignment(HashSet<byte> followers) // присваивание
        {
            StFoll obj = new StFoll();
            if (cur_token.Code == LexicalAnalyzer.ident)
            {
                VariableType ident_type = GetIdentType(followers);
                Accept(LexicalAnalyzer.ident);
                if (obj.sf[StFoll.assigns].Contains(cur_token.Code))
                {
                    cur_token = LexicalAnalyzer.NextSym();
                    TextPosition assignment_position = cur_token.Position;
                    VariableType expression_type = Expression(followers);
                    if (!CanAssign(ident_type, expression_type) && ident_type != VariableType.vartUndef &&
                        expression_type != VariableType.vartUndef)
                    {
                        // ошибка 145: конфликт типов
                        InputOutput.Error(145, assignment_position);
                        SkipTo(followers);
                    }
                    Accept(LexicalAnalyzer.semicolon);
                }
                else
                {
                    InputOutput.Error(cur_token.Code, LexicalAnalyzer.token);
                    SkipTo(followers);
                }
            }
        }

        static bool CanAssign(VariableType first_type, VariableType second_type)
        {
            bool result = false;
            if (first_type == VariableType.vartUndef || second_type == VariableType.vartUndef)
            {
                return result;
            }
            switch (first_type)
            {
                case VariableType.vartInteger:
                    result = second_type == VariableType.vartInteger;
                    break;
                case VariableType.vartFloat:
                    result = second_type == VariableType.vartFloat || second_type == VariableType.vartInteger;
                    break;
                case VariableType.vartString:
                    result = second_type == VariableType.vartString;
                    break;
                case VariableType.vartBoolean:
                    result = second_type == VariableType.vartBoolean;
                    break;
            }
            return result;
        }

        static VariableType GetIdentType(HashSet<byte> followers)
        {
            VariableType result = VariableType.vartUndef;
            if (cur_token == null)
                return result;
            string cur_name = ((CIdentToken)cur_token).Name;

            if (!identifiers.ContainsKey(cur_name))
            {
                // ошибка 104: имя не описано
                InputOutput.Error(104, cur_token.Position);
                ((CIdentToken)cur_token).Variable_type = VariableType.vartUndef;
                identifiers.Add(cur_name, (CIdentToken)cur_token);
                result = VariableType.vartUndef;
                SkipTo(followers);
            }
            else
            {
                result = identifiers[cur_name].Variable_type;
            }
            return result;
        }

        static VariableType Expression(HashSet<byte> ptra) // выражение
        {
            StFoll obj = new StFoll();
            if (!Belong(cur_token.Code, obj.sf[StFoll.st_expressions]))
            {
                InputOutput.Error(144, LexicalAnalyzer.token);
                SkipTo2(ptra, followers);
            }

            VariableType result = VariableType.vartUndef;

            if (obj.sf[StFoll.values].Contains(cur_token.Code))
            {
                SetDisjunct(obj.sf[StFoll.expressions], followers, out ptra);
                result = SimpleExpression(ptra);

                if (obj.sf[StFoll.comparisonOperators].Contains(cur_token.Code))
                {
                    TextPosition operator_pos = cur_token.Position;
                    byte? comparisonOperator = Operator(obj.sf[StFoll.comparisonOperators], ptra);
                    result = OperationType(result, SimpleExpression(followers), operator_pos, comparisonOperator);
                }
            }
            return result;
        }


        static byte? Operator(HashSet<byte> operators, HashSet<byte> followers)
        {
            if (!Belong(cur_token.Code, operators))
            {
                // ошибка 183: запрещенная в данном контексте операция
                InputOutput.Error(183, cur_token.Position);
                SkipTo(followers);
            }
            byte operator_res;
            if (Belong(cur_token.Code, operators))
            {
                operator_res = cur_token.Code;
                Accept(operator_res);
                return operator_res;
            }
            return null;
        }
        // <простое выражение>::=<знак><слагаемое>{<аддитивная операция><слагаемое>}
        // <аддитивная операция>::=+|-
        static VariableType SimpleExpression(HashSet<byte> followers)
        {
            StFoll obj = new StFoll();
            HashSet<byte> ptra;

            if (!Belong(cur_token.Code, obj.sf[StFoll.values]))
            {
                // ошибка 144: недопустимый тип выражения
                InputOutput.Error(144, cur_token.Position);
                SkipTo(followers);
            }
            VariableType result = VariableType.vartUndef;
            if (Belong(cur_token.Code, obj.sf[StFoll.values]))
            {
                bool unary_sign = false;
                TextPosition unary_sign_pos = cur_token.Position;
                if (cur_token.Code == LexicalAnalyzer.plus || cur_token.Code == LexicalAnalyzer.minus)
                {
                    Accept(cur_token.Code);
                    unary_sign = true;
                }
                SetDisjunct(obj.sf[StFoll.addingOperators], followers, out ptra);
                result = OperationType(unary_sign, unary_sign_pos, Term(ptra));
                while (Belong(cur_token.Code, obj.sf[StFoll.addingOperators]))
                {
                    SetDisjunct(obj.sf[StFoll.values], followers, out ptra);

                    TextPosition operator_pos = cur_token.Position;
                    byte? addingOperator = Operator(obj.sf[StFoll.addingOperators], ptra);
                    SetDisjunct(obj.sf[StFoll.multiplyingOperators], followers, out ptra);
                    result = OperationType(result, Term(ptra), operator_pos, addingOperator);
                }
            }
            return result;
        }

        static VariableType OperationType(VariableType first_type, VariableType second_type, TextPosition operator_pos, byte? operation)
        {
            StFoll obj = new StFoll();

            VariableType result = VariableType.vartUndef;
            if (operation == null)
            {
                return result;
            }
            if (first_type == VariableType.vartUndef || second_type == VariableType.vartUndef)
                return VariableType.vartUndef;
            if (Belong((byte)operation, obj.sf[StFoll.comparisonOperators]))
                if (first_type == second_type ||
                    first_type == VariableType.vartInteger && second_type == VariableType.vartFloat ||
                    first_type == VariableType.vartFloat && second_type == VariableType.vartInteger)
                    result = VariableType.vartBoolean;
                else
                    // ошибка 186: несоответствие типов для операции отношения
                    InputOutput.Error(186, operator_pos);
            if (Belong((byte)operation, obj.sf[StFoll.addingOperators]) || Belong((byte)operation, obj.sf[StFoll.multiplyingOperators]))
                switch (operation)
                {
                    // складывать можно все пары, кроме boolean, а разные типы только между числовыми
                    case LexicalAnalyzer.plus:
                        if (first_type == second_type && first_type != VariableType.vartBoolean)
                            result = first_type;
                        else if (first_type == VariableType.vartInteger && second_type == VariableType.vartFloat ||
                           first_type == VariableType.vartFloat && second_type == VariableType.vartInteger)
                            result = VariableType.vartFloat;
                        // ошибка 211: недопустимые типы операндов операции + или —
                        else InputOutput.Error(211, operator_pos);
                        break;
                    // вычитать, умножать и делить можно только числовые
                    case LexicalAnalyzer.star:
                    case LexicalAnalyzer.slash:
                    case LexicalAnalyzer.minus:
                        // если делить инт на инт, будет real
                        if (first_type == second_type && first_type != VariableType.vartBoolean && first_type != VariableType.vartString)
                            result = operation == LexicalAnalyzer.slash ? VariableType.vartFloat : first_type;
                        // операции с real дают real
                        else if (first_type == VariableType.vartInteger && second_type == VariableType.vartFloat ||
                           first_type == VariableType.vartFloat && second_type == VariableType.vartInteger)
                            result = VariableType.vartFloat;
                        else if (operation == LexicalAnalyzer.minus)
                            // ошибка 211: недопустимые типы операндов операции + или —
                            InputOutput.Error(211, operator_pos);
                        else if (operation == LexicalAnalyzer.slash)
                            // ошибка 214: недопустимые типы операндов операции /
                            InputOutput.Error(214, operator_pos);
                        else if (operation == LexicalAnalyzer.star)
                            // ошибка 213: недопустимые типы операндов операции *
                            InputOutput.Error(213, operator_pos);
                        break;
                    // логические только между boolean
                    case LexicalAnalyzer.andsy:
                    case LexicalAnalyzer.orsy:
                        if (first_type == second_type && first_type == VariableType.vartBoolean)
                            result = VariableType.vartBoolean;
                        else
                        {
                            // ошибка 210: операнды AND, NOT, OR должны быть булевыми
                            InputOutput.Error(210, operator_pos);
                        }
                        break;
                }
            return result;
        }

        // проверка корректности типа выражения, перед которым стоит знак (или не стоит)
        static VariableType OperationType(bool unary_sign, TextPosition sign_pos, VariableType type)
        {
            if (!unary_sign)
            {
                return type;
            }
            VariableType result = VariableType.vartUndef;
            switch (type)
            {
                case VariableType.vartInteger:
                case VariableType.vartFloat:
                    result = type;
                    break;
                case VariableType.vartBoolean:
                case VariableType.vartString:
                    // ошибка 184: элемент этого типа не может иметь знак
                    InputOutput.Error(184, sign_pos);
                    break;
            }
            return result;
        }

        // <слагаемое>::=<множитель>{<мультипликативная операция><множитель>}
        // <мультипликативная операция>::=*|/|div|mod|and
        static VariableType Term(HashSet<byte> followers)
        {
            StFoll obj = new StFoll();
            HashSet<byte> ptra;

            if (!Belong(cur_token.Code, obj.sf[StFoll.st_expressions]))
            {
                // ошибка 144: недопустимый тип выражения
                InputOutput.Error(144, cur_token.Position);
                SkipTo(followers);
            }
            VariableType result = VariableType.vartUndef;
            if (Belong(cur_token.Code, obj.sf[StFoll.st_expressions]))
            {
                SetDisjunct(obj.sf[StFoll.st_procfuncpart], followers, out ptra);
                result = Factor(ptra);
                while ((Belong(cur_token.Code, obj.sf[StFoll.multiplyingOperators])))
                {
                    SetDisjunct(obj.sf[StFoll.values], followers, out ptra);
                    TextPosition operator_pos = cur_token.Position;
                    byte? multiplyingOperator = Operator(obj.sf[StFoll.multiplyingOperators], ptra);
                    SetDisjunct(obj.sf[StFoll.multiplyingOperators], followers, out ptra);
                    result = OperationType(result, Factor(ptra), operator_pos, multiplyingOperator);
                }
            }
            return result;
        }

    // <множитель>::=<переменная>|
    //               <константа без знака>|
    //               (<выражение>)|
    ////               <обозначение функции>|
    ////               <множество>|
    //               not <множитель>
    // <константа без знака>::=<число без знака>|
    //                         <строка>|
    //                         <имя константы>|
    ////                       nil
    static VariableType Factor(HashSet<byte> followers)
    {
        StFoll obj = new StFoll();
        // HashSet<byte> ptra;
        if (!Belong(cur_token.Code, obj.sf[StFoll.st_expressions]))
        {
            // оишбка 144: недопустимый тип выражения
            InputOutput.Error(144, cur_token.Position);
            SkipTo(followers);
        }
        VariableType result = VariableType.vartUndef;
        if (Belong(cur_token.Code, obj.sf[StFoll.st_expressions]))
        {
            switch (cur_token.Code)
            {
                case LexicalAnalyzer.ident:
                    result = GetIdentType(followers);
                    Accept(LexicalAnalyzer.ident);
                    break;
                case LexicalAnalyzer.truesy:
                    result = VariableType.vartBoolean;
                    Accept(LexicalAnalyzer.truesy);
                    break;
                case LexicalAnalyzer.falsesy:
                    result = VariableType.vartBoolean;
                    Accept(LexicalAnalyzer.falsesy);
                    break;
                case LexicalAnalyzer.intc:
                    result = VariableType.vartInteger;
                    Accept(LexicalAnalyzer.intc);
                    break;
                case LexicalAnalyzer.floatc:
                    result = VariableType.vartFloat;
                    Accept(LexicalAnalyzer.floatc);
                    break;
                case LexicalAnalyzer.stringc:
                    result = VariableType.vartString;
                    Accept(LexicalAnalyzer.stringc);
                    break;
                case LexicalAnalyzer.leftpar:
                    Accept(LexicalAnalyzer.leftpar);
                    followers.Add(LexicalAnalyzer.rightpar);
                    result = Expression(followers);
                    Accept(LexicalAnalyzer.rightpar);
                    break;
                case LexicalAnalyzer.notsy:
                    Accept(LexicalAnalyzer.notsy);
                    result = Factor(followers);
                    if (result == VariableType.vartBoolean)
                    {
                        // 210: операнды AND, NOT, OR должны быть булевыми
                        InputOutput.Error(210, cur_token.Position);
                    }
                    break;
            }
        }
        return result;
    }

        public static void Programme() /* создание элемента стека для фиктивной области действия */
        {
            cur_token = LexicalAnalyzer.NextSym();
            // SemanticAnalyzer.OpenScope();
            /* построение ТИ и ТТ фиктивной области действия */
            // InputOutput.boolType = SemanticAnalyzer.NewType(SemanticAnalyzer.enums);
            // SearchInTable("false");
            /* построение вершины в ТИ для константы false */
            // entry = newident(hashresult, addrname, CONSTS);
            // entry->idtype = booltype;
            /* создание элемента списка констант в дескрипторе перечислимого типа */
            // (booltype->casetype).firstconst =
            // newconst(addrname);
            /* далее - аналогичные действия для константы true */
            // search_in_table("true");
            // entry = newident(hashresult, addrname, CONSTS);
            // entry->idtype = booltype;
            // (booltype->casetype).firstconst->next =
            // newconst(addrname);
            /* продолжаем строить дескрипторы стандартных типов*/
            // realtype = newtype(SCALARS);
            // chartype = newtype(SCALARS);
            // inttype = newtype(SCALARS);
            // texttype = newtype(FILES);
            // (texttype->casetype).basetype = chartype;
            /* заносим в таблицу имен и ТИ остальные стандартные идентификаторы */
            // search_in_table("integer");
            // entry = newident(hashresult, addrname, TYPES);
            // entry->idtype = inttype;
            // search_in_table("maxint");
            // entry = newident(hashresult, addrname, CONSTS);
            // (entry->casenode).constvalue.intval = MAXCONST;
            /* значение константы MAXCONST зависит от конкретной вычислительной машины */
            // entry->idtype = inttype;
            // search_in_table("boolean");
// ...
// search_in_table("real");
            // entry = newident(hashresult, addrname, TYPES);
            // entry->idtype = realtype;
            /* создание элемента стека для области действия основной программы */
            // open_scope;
            /* синтаксический анализ */
            Accept(LexicalAnalyzer.programsy);
            Accept(LexicalAnalyzer.ident);
            // Accept(LexicalAnalyzer.leftpar); // ?
            // Accept(LexicalAnalyzer.ident);
            // while (LexicalAnalyzer.symbol == LexicalAnalyzer.comma)
            // {
            //     cur_token = LexicalAnalyzer.NextSym();
            //     Accept(LexicalAnalyzer.ident);
            // }
            // Accept(LexicalAnalyzer.rightpar);
            Accept(LexicalAnalyzer.semicolon);
            followers.Add(LexicalAnalyzer.point);
            Block(followers);
        }
    }
}