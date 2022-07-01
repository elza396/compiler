using System;
using System.Collections.Generic;

namespace Compiler
{
    class SyntaxisAnalyzer
    {
        static HashSet<byte> followers = new HashSet<byte>(); // внешние
        static bool ifStart = true; // ?
        public static int count = 0; // ?
        static bool ifFlag = false; // ?
        static int beginEndCount = 0; // ?
        static void Accept(byte symbolExpected)
        {
            if (LexicalAnalyzer.symbol == symbolExpected)
            {
                LexicalAnalyzer.NextSym();
            }
            else
            {
                InputOutput.Error(symbolExpected, LexicalAnalyzer.token);
            }
        }

        static bool Belong(byte element, HashSet<byte> set)
        {
            return set.Contains(element);
        }

        static void SkipTo(HashSet<byte> where)
        {
            while (!Belong(LexicalAnalyzer.symbol, where))
            {
                LexicalAnalyzer.NextSym();
            }
        }
        static void SkipTo2(HashSet<byte> start, HashSet<byte> follow)
        {
            while (!(Belong(LexicalAnalyzer.symbol, start) || Belong(LexicalAnalyzer.symbol, follow)))
            {
                LexicalAnalyzer.NextSym();
            }
        }

        static void SetDisjunct(HashSet<byte> set1, HashSet<byte> set2, out HashSet<byte> set3)
        {
            set3 = new HashSet<byte>();
            set3.UnionWith(set2);
            set3.UnionWith(set1);
        }

        SyntaxisAnalyzer()
        {
            if (ifStart)
            {
                // SemanticAnalyzer.TreeNode temp = null; // добавляем новую обасть видимости новую голову
                // SemanticAnalyzer.heads.Push(temp);
                HashSet<byte> ptra = new HashSet<byte>();
                StFoll obj = new StFoll();
                SetDisjunct(obj.sf[StFoll.st_typepart], followers, out ptra);
                // union(followers, allCodes.begpart);                // add "var", "function" and the rest in followers
                followers.Add(LexicalAnalyzer.point); // ?

                ptra.Add(LexicalAnalyzer.programsy);
                if (!Belong(LexicalAnalyzer.symbol, ptra))
                {
                    InputOutput.Error(3, LexicalAnalyzer.token);
                    SkipTo2(ptra, followers);
                }
                else
                {
                    Accept(LexicalAnalyzer.programsy);
                    Accept(LexicalAnalyzer.ident);
                    Accept(LexicalAnalyzer.semicolon);
                }
                ifStart = false;
            }
            else
            {
                Block(followers);
                if (!ifFlag)
                {
                    LexicalAnalyzer.NextSym();
                }
                ifFlag = false; // ?
            }
        }

        static void LabelPart() // раздел меток
        {
            if (LexicalAnalyzer.symbol == LexicalAnalyzer.labelsy)
            {
                Accept(LexicalAnalyzer.labelsy);
                if (LexicalAnalyzer.symbol == LexicalAnalyzer.intc || LexicalAnalyzer.symbol == LexicalAnalyzer.ident)
                {
                    LexicalAnalyzer.NextSym();
                }
                else
                {
                    InputOutput.Error(26, LexicalAnalyzer.token);
                }

                while (LexicalAnalyzer.symbol == LexicalAnalyzer.comma)
                {
                    LexicalAnalyzer.NextSym();
                    if (LexicalAnalyzer.symbol == LexicalAnalyzer.intc || LexicalAnalyzer.symbol == LexicalAnalyzer.ident)
                    {
                        LexicalAnalyzer.NextSym();
                    }
                    else
                    {
                        InputOutput.Error(26, LexicalAnalyzer.token);
                    }
                }
                Accept(LexicalAnalyzer.semicolon);
            }
        }

        static void ConstPart() //раздел констант, не знаю нужен ли, но пусть будет
        {
            if (LexicalAnalyzer.symbol == LexicalAnalyzer.constsy)
            {
                Accept(LexicalAnalyzer.constsy);
                Accept(LexicalAnalyzer.ident);
                Accept(LexicalAnalyzer.equal);
                if (LexicalAnalyzer.symbol == LexicalAnalyzer.intc ||
                    LexicalAnalyzer.symbol == LexicalAnalyzer.floatc ||
                    LexicalAnalyzer.symbol == LexicalAnalyzer.charc) // ? либо строка
                {
                    LexicalAnalyzer.NextSym();
                }
            }
            else
            {
                InputOutput.Error(26, LexicalAnalyzer.token);
            }

            while (LexicalAnalyzer.symbol == LexicalAnalyzer.semicolon)
            {
                LexicalAnalyzer.NextSym();
                if (LexicalAnalyzer.symbol == LexicalAnalyzer.typesy || LexicalAnalyzer.symbol == LexicalAnalyzer.varsy) // ?
                {
                    break;
                }
                Accept(LexicalAnalyzer.ident);
                Accept(LexicalAnalyzer.equal);
                if (LexicalAnalyzer.symbol == LexicalAnalyzer.intc ||
                    LexicalAnalyzer.symbol == LexicalAnalyzer.floatc ||
                    LexicalAnalyzer.symbol == LexicalAnalyzer.charc)
                {
                    LexicalAnalyzer.NextSym();
                }
                else
                {
                    InputOutput.Error(26, LexicalAnalyzer.token);
                }

            }
            Accept(LexicalAnalyzer.semicolon);
        }

        static void Block(HashSet<byte> followers)
        {
            HashSet<byte> ptra;
            StFoll obj = new StFoll();
            if (!Belong(LexicalAnalyzer.symbol, obj.sf[StFoll.begpart]))
            {
                InputOutput.Error(18); // ошибка в разделе описаний
                SkipTo2(obj.sf[StFoll.begpart], followers);
            }
            if (Belong(LexicalAnalyzer.symbol, obj.sf[StFoll.begpart]))
            {
                LabelPart();
                SetDisjunct(obj.sf[StFoll.st_typepart], followers, out ptra);
                ConstPart();
                SetDisjunct(obj.sf[StFoll.st_varpart], followers, out ptra);
                // TypePart(ptra); // а надо ли нам это?
                SetDisjunct(obj.sf[StFoll.st_procfuncpart], followers, out ptra);
                VarPart(ptra);
                //ProcFuncPart(ptra); // не реализовано
                // StatPart(followers);
                if (!Belong(LexicalAnalyzer.symbol, followers))
                {
                    InputOutput.Error(6); // запрещенный символ
                    SkipTo(followers);
                }
            }
        }

        static void VarPart(HashSet<byte> followers)
        {
            HashSet<byte> ptra;
            StFoll obj = new StFoll();
            if (!Belong(LexicalAnalyzer.symbol, obj.sf[StFoll.st_varpart]))
            {
                InputOutput.Error(18); // ошибка в разделе описаний
                SkipTo2(obj.sf[StFoll.st_varpart], followers);
            }
            if (LexicalAnalyzer.symbol == LexicalAnalyzer.varsy)
            {
                LexicalAnalyzer.NextSym();
                SetDisjunct(obj.sf[StFoll.after_var], followers, out ptra);
                do
                {
                    VarDeclaration(ptra);
                    Accept(LexicalAnalyzer.semicolon);
                }
                while (LexicalAnalyzer.symbol == LexicalAnalyzer.ident);
                if (!Belong(LexicalAnalyzer.symbol, followers))
                {
                    InputOutput.Error(6); // запрещенный символ
                    SkipTo(followers);
                }
            }
        }


        static void VarDeclaration(HashSet<byte> followers)
        {
            StFoll obj = new StFoll();
            if (!Belong(LexicalAnalyzer.symbol, obj.sf[StFoll.id_starters]))
            {
                InputOutput.Error(2);
                SkipTo2(obj.sf[StFoll.id_starters], followers);
            }
            if (LexicalAnalyzer.symbol == LexicalAnalyzer.ident)
            {
                // InputOutput.varList = null;
                // NewVariable();
                Accept(LexicalAnalyzer.ident);
                // LexicalAnalyzer.NextSym(); ???????
                while (LexicalAnalyzer.symbol == LexicalAnalyzer.comma)
                {
                    LexicalAnalyzer.NextSym();
                    // NewVariable();
                    Accept(LexicalAnalyzer.ident);
                }
                Accept(LexicalAnalyzer.colon);
                // InputOutput.varType = Typе(followers);
                // внешняя переменная varType содержит адрес дескриптора типа для однотипных переменных
                // AddAttributes();
                if (LexicalAnalyzer.symbol == LexicalAnalyzer.arraysy)
                {
                    // SemanticAnalyzer.tempIdTypeArray = SemanticAnalyzer.arrays; // тип идентификаторов массив
                    ArrayType(); // массив?
                }
                else Type();
                if (!Belong(LexicalAnalyzer.symbol, followers))
                {
                    InputOutput.Error(6); // запрещенный символ
                    SkipTo(followers);
                }
            }
        }

        static void ArrayType()
        {
            Accept(LexicalAnalyzer.arraysy);
            Accept(LexicalAnalyzer.lbracket);
            SimpleType();
            while (LexicalAnalyzer.symbol == LexicalAnalyzer.comma)
            {
                Accept(LexicalAnalyzer.comma);
                SimpleType();
            }
            Accept(LexicalAnalyzer.rbracket);
            Accept(LexicalAnalyzer.ofsy);
            Type();
        }

        static void SimpleType()
        {
            StFoll obj = new StFoll();
            if (!Belong(LexicalAnalyzer.symbol, obj.sf[StFoll.id_starters]))
            {
                Accept(LexicalAnalyzer.intc);
            }
            else LexicalAnalyzer.NextSym();
        }

        static void Type() // простые типы
        {
            StFoll obj = new StFoll();
            if (!Belong(LexicalAnalyzer.symbol, obj.sf[StFoll.types]))
                Accept(LexicalAnalyzer.integersy);
            else
            {
                // SemanticAnalyzer.tempIdType = SemanticAnalyzer.scalars;
                LexicalAnalyzer.NextSym();
            }
        }

        // void Statement(HashSet<byte> followers) // анализ конструкции <оператор>
        // {
        //     if (LexicalAnalyzer.symbol == LexicalAnalyzer.intc) // семантический анализ метки
        //     {
        //         // ...
        //         LexicalAnalyzer.NextSym();
        //         Accept(LexicalAnalyzer.colon);
        //     }
        //     switch (LexicalAnalyzer.symbol)
        //     {
        //         case LexicalAnalyzer.ident:
        //             if (
        //                // идентификатор — имя поля, переменной или функции
        //                )
        //             // анализ оператора присваивания
        //                 Assignment(followers);
        //             else
        //                 // анализ вызова процедуры
        //                 CallProc(followers);
        //             break;
        //         case LexicalAnalyzer.beginsy:
        //             CompoundStatement(followers); break;
        //         case LexicalAnalyzer.ifsy:
        //             IfStatement(followers); break;
        //         case LexicalAnalyzer.whilesy:
        //             WhileStatement(followers); break;
        //         case LexicalAnalyzer.repeatsy:
        //             RepeatStatement(followers); break;
        //         case LexicalAnalyzer.forsy:
        //             ForStatement(followers); break;
        //         case LexicalAnalyzer.casesy:
        //             CaseStatement(followers); break;
        //         case LexicalAnalyzer.withsy:
        //             WithStatement(followers); break;
        //         case LexicalAnalyzer.gotosy:
        //             GotoStatement(followers); break;
        //         case LexicalAnalyzer.semicolon:
        //         case LexicalAnalyzer.endsy:
        //         case LexicalAnalyzer.untilsy:
        //         case LexicalAnalyzer.elsesy: break; // в случае пустого оператора
        //     }
        // }

//         void programme()
//                 /* создание элемента стека для фиктивной области действия */
//         {
//             SemanticAnalyzer.OpenScope();
//             /* построение ТИ и ТТ фиктивной области действия */
//             InputOutput.boolType = SemanticAnalyzer.NewType(SemanticAnalyzer.enums);
//             SearchInTable("false");
//             /* построение вершины в ТИ для константы false */
//             entry = newident(hashresult, addrname, CONSTS);
//             entry->idtype = booltype;
//             /* создание элемента списка констант в дескрипторе
//             перечислимого типа */
//             (booltype->casetype).firstconst =
//             newconst(addrname);
//             /* далее - аналогичные действия
//             для константы true */
//             search_in_table("true");
//             entry = newident(hashresult, addrname, CONSTS);
//             entry->idtype = booltype;
//             (booltype->casetype).firstconst->next =
//             newconst(addrname);
//             /* продолжаем строить дескрипторы
//             стандартных типов*/
//             realtype = newtype(SCALARS);
//             chartype = newtype(SCALARS);
//             inttype = newtype(SCALARS);
//             texttype = newtype(FILES);
//             (texttype->casetype).basetype = chartype;
//             /* заносим в таблицу имен и ТИ остальные
//             стандартные идентификаторы */
//             search_in_table("integer");
//             entry = newident(hashresult, addrname, TYPES);
//             entry->idtype = inttype;
//             search_in_table("maxint");
//             entry = newident(hashresult, addrname, CONSTS);
//             (entry->casenode).constvalue.intval = MAXCONST;
//             /* значение константы MAXCONST зависит
//             от конкретной вычислительной машины */
//             entry->idtype = inttype;
//             search_in_table("boolean");
// ...
// search_in_table("real");
//             entry = newident(hashresult, addrname, TYPES);
//             entry->idtype = realtype;
//             /* создание элемента стека для области
//             действия основной программы */
//             open_scope;
//             /* синтаксический анализ */
//             accept(programsy); accept(ident);
//             accept(leftpar); accept(ident);
//             while (symbol == comma)
//             {
//                 nextsym();
//                 accept(ident);
//             }
//             accept(rightpar); accept(semicolon);
//             block(); accept(point);
//         }

        // public class ListRec
        // {
        //     public SemanticAnalyzer.TreeNode  idR; /* адрес вершины таблицы идентификаторов */
        //     public ListRec next; /* адрес следующего элемента списка */
        // }
        //
        // void NewVariable()
        // /* создание элемента вспомогательного списка;
        // в момент вызова этой функции информация о типе
        // идентификатора еще не прочитана анализатором */
        // {
        //     ListRec listEntry; /* указатель на текущий элемент вспомогательного списка */
        //     if (LexicalAnalyzer.symbol == LexicalAnalyzer.ident)
        //     {
        //         listEntry = new ListRec();
        //         listEntry.idR = NewIdent(hashResult, addrName, SemanticAnalyzer.vars);
        //         listEntry.next = InputOutput.varList;
        //         InputOutput.varList = listEntry;
        //     }
        // }

        // void AddAttributes()
        // /* присваивает значение полю idType для вершин, адреса которых содержатся во вспомогательном списке */
        // {
        //     ListRec listEntry; /* указатель на вершину вспомогательного списка */
        //     listEntry = InputOutput.varList;
        //     while (listEntry != null)
        //     {
        //         listEntry.idR.idType = varType;
        //         /* внешняя переменная varType содержит адрес дескриптора типа для однотипных переменных */
        //         listEntry = listEntry.next;
        //     }
        // }

        // SemanticAnalyzer.TypeRec SimpleType(HashSet<byte> followers) // анализ конструкции <простой тип>
        // {
        //     SemanticAnalyzer.TypeRec typEntry = null; // указатель на дескриптор типа
        //     SemanticAnalyzer.TreeNode idEntry; // указатель на вершину в ТИ
        //     ReestrConsts constList = null; // указатель на элемент списка констант в дескрипторе перечислимого типа
        //     switch (LexicalAnalyzer.symbol)
        //     {
        //         case LexicalAnalyzer.leftpar: // анализ описания перечислимого типа
        //             typEntry = SemanticAnalyzer.NewType(SemanticAnalyzer.enums); // строим "каркас" дескриптора типа
        //             do
        //             {
        //                 LexicalAnalyzer.NextSym();
        //                 if (LexicalAnalyzer.symbol == LexicalAnalyzer.ident)
        //                 {
        //                     // создаем вершину в ТИ для константы перечислимого типа
        //                     idEntry = SemanticAnalyzer.NewIdent(hashResult, addrName, SemanticAnalyzer.consts);
        //                     // запоминаем указатель на дескриптор типа константы в ТИ
        //                     idEntry.idType = typEntry;
        //                     // запоминаем имя константы в ТИ
        //                     (idEntry.caseNode).constValue.enumVal = addrName;
        //                     if (constList == null)
        //                         // создаем первый элемент списка констант в дескрипторе перечислимого типа
        //                         typEntry.caseType.firstConst = constList = NewConst(addrName);
        //                     else
        //                     {
        //                         // создаем следующий элемент списка констант в дескрипторе перечислимого типа
        //                         constList.next = NewConst(addrName);
        //                         constList = constList.next;
        //                     }
        //                 }
        //                 Accept(LexicalAnalyzer.ident);
        //             }
        //             while (LexicalAnalyzer.symbol == LexicalAnalyzer.comma);
        //             Accept(LexicalAnalyzer.rightpar);
        //         break;
        //     case ident: . . .
        //     default: . . .
        //    }
        //   return typEntry;
        // }

        // void Statement(HashSet<byte> followers) // анализ конструкции <оператор>
        // {
        //     if (LexicalAnalyzer.symbol == LexicalAnalyzer.intc) // семантический анализ метки
        //     {
        //         // ...
        //         LexicalAnalyzer.NextSym();
        //         Accept(LexicalAnalyzer.colon);
        //     }
        //     switch (LexicalAnalyzer.symbol)
        //     {
        //         case LexicalAnalyzer.ident:
        //             if (
        //                // идентификатор — имя поля, переменной или функции
        //                )
        //             // анализ оператора присваивания
        //                 Assignment(followers);
        //             else
        //                 // анализ вызова процедуры
        //                 CallProc(followers);
        //             break;
        //         case LexicalAnalyzer.beginsy:
        //             CompoundStatement(followers); break;
        //         case LexicalAnalyzer.ifsy:
        //             IfStatement(followers); break;
        //         case LexicalAnalyzer.whilesy:
        //             WhileStatement(followers); break;
        //         case LexicalAnalyzer.repeatsy:
        //             RepeatStatement(followers); break;
        //         case LexicalAnalyzer.forsy:
        //             ForStatement(followers); break;
        //         case LexicalAnalyzer.casesy:
        //             CaseStatement(followers); break;
        //         case LexicalAnalyzer.withsy:
        //             WithStatement(followers); break;
        //         case LexicalAnalyzer.gotosy:
        //             GotoStatement(followers); break;
        //         case LexicalAnalyzer.semicolon:
        //         case LexicalAnalyzer.endsy:
        //         case LexicalAnalyzer.untilsy:
        //         case LexicalAnalyzer.elsesy: break; // в случае пустого оператора
        //     }
        // }

        // void IfStatement(HashSet<byte> followers)
        // {
        //     SemanticAnalyzer.TypeRec expType; // указатель на дескриптор типа выражения
        //     HashSet<byte> ptra = null; // множество внешних символов
        //     LexicalAnalyzer.NextSym();
        //     // формирование множества внешних cимволов для конструкции <выражение>
        //     SetDisjunct(af_iftrue, followers, ptra);
        //     expType = Expression(ptra);
        //     // проверка типа выражения
        //     if (!Compatible(expType, InputOutput.boolType))
        //         InputOutput.Error(135); // тип операнда должен быть boolean
        //     Accept(LexicalAnalyzer.thensy);
        //     // формирование множества внешних символов для конструкции <оператор>
        //     SetDisjunct(af_iffalse, followers, ptra);
        //     Statement(ptra);
        //     if (LexicalAnalyzer.symbol == LexicalAnalyzer.elsesy)
        //     {
        //         LexicalAnalyzer.NextSym();
        //         Statement(followers);
        //     }
        // }

        // void ForStatement(HashSet<byte> followers) // анализ конструкции <цикл с параметром>
        // {
        //     SemanticAnalyzer.TypeRec expType; // указатель на дескриптор типа выражения
        //     SemanticAnalyzer.TypeRec varType; // указатель на дескриптор типа параметра цикла
        //     HashSet<byte> ptra; // множество внешних символов
        //     UInt16 codeVar; // код типа переменной
        //     LexicalAnalyzer.NextSym();
        //     // формирование множества внешних символов для конструкции <параметр цикла>
        //     SetDisjunct(af_forassign, followers, ptra);
        //     varType = NameParameter(ptra);
        //     if (varType != null)
        //     {
        //         codeVar = varType.typeCode;
        //         if (codeVar != SemanticAnalyzer.scalars && codeVar != SemanticAnalyzer.enums
        //             && codeVar != SemanticAnalyzer.limiteds || varType == InputOutput.realType)
        //            InputOutput.Error(143); // недопустимый тип параметра цикла
        //     }
        //     Accept(LexicalAnalyzer.assign);
        //     // формирование множества внешних символов для первого выражения
        //     SetDisjunct(af_for1, followers, ptra);
        //     expType = Expression(ptra);
        //     if (!Compatible(varType, expType))
        //         InputOutput.Error(145); // конфликт типов
        //     if ((LexicalAnalyzer.symbol == LexicalAnalyzer.tosy) || (LexicalAnalyzer.symbol == LexicalAnalyzer.downtosy))
        //         LexicalAnalyzer.NextSym();
        //     else InputOutput.Error(55); // должно идти слово to или downto
        //     // формирование множества внешних символов для второго выражения
        //     SetDisjunct(af_whilefor, followers, ptra);
        //     expType = Expression(ptra);
        //     if (!Compatible(varType, expType))
        //         InputOutput.Error(145); // конфликт типов
        //     Accept(LexicalAnalyzer.dosy);
        //     Statement(followers);
        // }

        // public class WithStack
        // {
        //     public SemanticAnalyzer.TypeRec varPtr; // указатель на дескриптор типа переменной-записи
        //     public WithStack nextWith; // указатель на следующий элемент стека
        // }

        // void WithStatement(HashSet<byte> followers) // анализ конструкции <оператор присоединения>
        // {
        //     SemanticAnalyzer.TypeRec varType; // указатель на дескриптор типа переменной
        //     WithStack withPtr; // указатель на текущий элемент стека
        //     byte varCount; // счетчик числа переменных-записей в операторе присоединения
        //     HashSet<byte> ptra; // множество внешних символов
        //     byte i; // вспомогательный счетчик
        //     // формирование внешних символов для конструкции <переменная>
        //     SetDisjunct(af_with, followers, ptra);
        //     // af_with - множество символов, ожидаемых сразу после анализа переменной в заголовке оператора with — "," и "do"
        //     varCount = 0;
        //     do
        //     {
        //         LexicalAnalyzer.NextSym();
        //         varType = Variable(ptra);
        //         if (varType != null)
        //             if (varType.typeCode != SemanticAnalyzer.records)
        //                 InputOutput.Error(140); // переменная не есть запись
        //             else
        //             {
        //                 // создание нового элемента стека
        //                 withPtr = new WithStack();
        //                 withPtr.varPtr = varType;
        //                 withPtr.nextWith = InputOutput.localWith;
        //                 InputOutput.localWith = withPtr;
        //                 varCount++;
        //             }
        //     }
        //     while (LexicalAnalyzer.symbol == LexicalAnalyzer.comma);
        //     Accept(LexicalAnalyzer.dosy);
        //     Statement(followers);
        //     // из стека удаляется такое количество элементов, сколько переменных перечислено в заголовке оператора присоединения
        //     for (i = 0; i < varCount; i++)
        //     {
        //         withPtr = InputOutput.localWith;
        //         InputOutput.localWith = InputOutput.localWith.nextWith;
        //     }
        // }

      //   SemanticAnalyzer.TypeRec Expression(HashSet<byte> followers) // анализ конструкции <выражение>
      //   {
      //       SemanticAnalyzer.TypeRec ex1Type = null, ex2Type; // указатели на дескрипторы типов простых выражений
      //       HashSet<byte> afterSimExpr = null; // внешние символы
      //       byte operation; // операция над простыми выражениями
      //       // формирование внешних символов для простого выражения
      //       SetDisjunct(followers, op_rel, afterSimExpr);
      //       // анализ простого выражения
      //       ex1Type = SimpleExpression(afterSimExpr);
      //       if (Belong(LexicalAnalyzer.symbol, op_rel)) // найден знак операции отношения
      //       {
      //           operation = LexicalAnalyzer.symbol; // запоминаем операцию
      //           LexicalAnalyzer.NextSym();
      //           ex2Type = SimpleExpression(followers);
      //           // проверка совместимости типов простых выражений
      //           ex1Type = Comparing(ex1Type, ex2Type, operation);
      //       }
      //       return ex1Type;
      //   }

      //   SemanticAnalyzer.TypeRec SimpleExpression(HashSet<byte> followers) // анализ конструкции <простое выражение>
      //   {
      //       SemanticAnalyzer.TypeRec ex1Type, ex2Type; // указатели на дескрипторы типов слагаемых
      //       HashSet<byte> afterTerm; // внешние символы
      //       bool sign = false; // имеется ли знак у 1-го слагаемого
      //       byte operation; // операция над слагаемыми: +, -, OR
      //       if (LexicalAnalyzer.symbol == LexicalAnalyzer.minus || LexicalAnalyzer.symbol == LexicalAnalyzer.plus)
      //       { sign = true; LexicalAnalyzer.NextSym(); }
      //       // формирование внешних символов для слагаемого
      //       SetDisjunct(op_add, followers, afterTerm);
      //       // анализ слагаемого
      //       ex1Type = Term(afterTerm);
      //       // проверка – правильно ли записан знак перед первым слагаемым?
      //       if (sign) RightSign(ex1Type);
      //       while (Belong(LexicalAnalyzer.symbol, op_add))
      //       {
      //           operation = LexicalAnalyzer.symbol;
      //           LexicalAnalyzer.NextSym();
      //           ex2Type = Term(afterTerm);
      //           // проверка совместимости типов слагаемых
      //           ex1Type = TestAdd(ex1Type, ex2Type, operation);
      //       }
      //       return ex1Type;
      //   }

      //   SemanticAnalyzer.TypeRec Term(HashSet<byte> followers) // анализ конструкции <слагаемое>
      //   {
      //       SemanticAnalyzer.TypeRec ex1Type, ex2Type; // указатели на дескрипторы типов множителей
      //       HashSet<byte> afterFactor; // внешние символы
      //       byte operation; // операция над множителями: *, /, DIV, MOD, AND
      //       // формирование внешних символов для множителя
      //       SetDisjunct(followers, op_mult, afterFactor);
      //       // op_mult - внешняя переменная - указатель на множество символов: *, /, and, div, mod */
      //       // анализ множителя
      //       ex1Type = Factor(afterFactor);
      //       while (Belong(LexicalAnalyzer.symbol, op_mult))
      //       {
      //           operation = LexicalAnalyzer.symbol;
      //           LexicalAnalyzer.NextSym();
      //           ex2Type = Factor(afterFactor);
      //           // проверка совместимости типов множителей
      //           ex1Type = TestMult(ex1Type, ex2Type, operation);
      //       }
      //       return ex1Type;
      //   }

      //   SemanticAnalyzer.TypeRec Factor(HashSet<byte> followers) // анализ конструкции <множитель>
      //   {
      //       SemanticAnalyzer.TypeRec expType; // указатель на дескриптор типа множителя
      //      SemanticAnalyzer.TreeNode node; // указатель на вершину ТИ, соответствующую текущему идентификатору
      //       HashSet<byte> afterExpress = null; // внешние символы
      //       switch (LexicalAnalyzer.symbol)
      //       {
      //           case LexicalAnalyzer.leftpar: // выражение в круглых скобках
      //                  LexicalAnalyzer.NextSym();
      //                  // формирование внешних символов для выражения
      //                  SetDisjunct(followers, rpar, afterExpress);
      //                  expType = Expression(afterExpress);
      //                  Accept(LexicalAnalyzer.rightpar);
      //                  break;
      //           case LexicalAnalyzer.lbracket: // конструктор множества
      //                 expType = Set(followers);
      //                 break;
      //           case LexicalAnalyzer.notsy: // отрицание множителя
      //                 LexicalAnalyzer.NextSym();
      //                 expType = Logical(expType); // проверка типа множителя
      //                 break;
      //           case LexicalAnalyzer.intc: // целая константа
      //                   expType = InputOutput.intType;
      //                   LexicalAnalyzer.NextSym();
      //                   break;
      //           case LexicalAnalyzer.floatc: // вещественная константа
      //                   expType = InputOutput.realType;
      //                   LexicalAnalyzer.NextSym();
      //                   break;
      //           case LexicalAnalyzer.charc: // символьная константа
      //                   expType = InputOutput.charType;
      //                   LexicalAnalyzer.NextSym();
      //                   break;
      //           case LexicalAnalyzer.nilsy: // константа nil
      //                   expType = InputOutput.nilType;
      //                   LexicalAnalyzer.NextSym();
      //                   break;
      //           case LexicalAnalyzer.ident:
      //               if // находимся в контексте оператора with и идентификатор - имя поля
      //               (InputOutput.localWith != null && SearchField() != null)
      //          // функция SearchField() ищет имя поля, используя стек указателей на дескрипторы типов переменных-записей
      //                   expType = Variable(followers);
      //               else
      //               { // ищем имя в ТИ
      //                   node = SearchIdent(hashresult, addrName, set_VARCONFUNCS);
      //                   if (node != null) // нашли имя в ТИ
      //          // по способу использования имени выбираем путь дальнейшего анализа
      //                       switch (node.klass)
      //                       {
      //                           case SemanticAnalyzer.vars: // имя переменной
      //                                   expType = Variable(followers);
      //                                   break;
      //                           case SemanticAnalyzer.consts : // имя константы
      //                                   expType = node.idType;
      //                                   LexicalAnalyzer.NextSym();
      //                                   break;
      //                           case SemanticAnalyzer.funcs: // вызов функции
      //                                   expType = CallFunc(followers, node);
      //                                   break;
      //                           default: /* ошибка */
      //                                   expType = null;
      //                                   break;
      //                       }
      //                   else // имя отсутствует в ТИ
      //                       expType = StandardFunc(followers);
      //             // имя стандартной функции или ошибка
      //           }
      //           break;
      //       }
      //   return expType;
      // }

      //   bool RightSign(SemanticAnalyzer.TypeRec type) // указатель на дескриптор типа слагаемого
      //   // знак ( + или – ) может быть записан перед операндом, если его тип является целым или вещественным, или ограниченным на целом
      //   {
      //       if (type == null || type == InputOutput.intType
      //       || type == InputOutput.realType
      //       || type.typeCode == SemanticAnalyzer.limiteds
      //       && type.caseType.limType.baseType == InputOutput.intType)
      //           return true;
      //       else
      //       {
      //          InputOutput.Error(211); // недопустимые типы операндов операции + или -
      //           return false;
      //       }
      //   }

    }
}