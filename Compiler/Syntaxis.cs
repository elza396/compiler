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
        public static int count;
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
                    IfStatement(followers); break;
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
                ifFlag = true;
                Accept(LexicalAnalyzer.ifsy);
                Expression(followers);

                Accept(LexicalAnalyzer.thensy);
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

        static void Assignment(HashSet<byte> followers) // присваивание
        {
            StFoll obj = new StFoll();
            if (cur_token.Code == LexicalAnalyzer.ident)
            {
                // if (SemanticAnalyzer.searchIdent(SemanticAnalyzer.name) is null)
                //     Accept(LexicalAnalyzer.varsy);
                //
                ifFlag = true;
                cur_token = LexicalAnalyzer.NextSym();
                if (obj.sf[StFoll.assigns].Contains(cur_token.Code))
                {
                    cur_token = LexicalAnalyzer.NextSym();
                    Expression(followers);
                    Accept(LexicalAnalyzer.semicolon);
                }
                else
                {
                    InputOutput.Error(cur_token.Code, LexicalAnalyzer.token);
                }
            }
        }

        static void Expression(HashSet<byte> ptra) // выражение
        {
            StFoll obj = new StFoll();
            if (!Belong(cur_token.Code, obj.sf[StFoll.st_expressions]))
            {
                InputOutput.Error(3, LexicalAnalyzer.token);
                SkipTo2(ptra, followers);
            }

            if (cur_token.Code == LexicalAnalyzer.ident
                 || obj.sf[StFoll.values].Contains(cur_token.Code) )
            {
                if(cur_token.Code == LexicalAnalyzer.ident)
                {
                    // if(SemanticAnalyzer.searchIdent(SemanticAnalyzer.name) is null)
                    //     accept(LexicalAnalyzer.varsy, "Ошибка! Идентификатор не определен");
                    // GeneratorCodes.push_reference(LexicalAnalyzer.integersy, 0, 017,
                    //     (ulong)(SemanticAnalyzer.searchIdent(SemanticAnalyzer.name)).ofSet);
                }
                count++;
                cur_token = LexicalAnalyzer.NextSym();

                while (obj.sf[StFoll.expressions].Contains(cur_token.Code))
                {
                    cur_token = LexicalAnalyzer.NextSym();
                    if (cur_token.Code == LexicalAnalyzer.ident
                        || obj.sf[StFoll.values].Contains(cur_token.Code))
                    {
                        if (cur_token.Code == LexicalAnalyzer.ident)
                        {
                            // if (SemanticAnalyzer.searchIdent(SemanticAnalyzer.name) is null)
                            //     accept(LexicalAnalyzer.varsy, "Ошибка! Идентификатор не определен");
                            // GeneratorCodes.push_reference(LexicalAnalyzer.integersy, 0, 017,
                            // (ulong)(SemanticAnalyzer.searchIdent(SemanticAnalyzer.name)).ofSet);
                            // GeneratorCodes.multop((ulong)LexicalAnalyzer.star, LexicalAnalyzer.integersy);
                        }
                        cur_token = LexicalAnalyzer.NextSym();
                    }
                    else
                    {
                        Accept(LexicalAnalyzer.ident);
                        break;
                    }
                }
            }
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