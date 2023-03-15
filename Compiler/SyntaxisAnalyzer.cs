// using System;
// using System.Collections.Generic;
//
// namespace Compiler
// {
//     class SyntaxisAnalyzer
//     {
//         static HashSet<byte> followers = new HashSet<byte>();
//         static bool ifStart = true;
//         public static int count; // ?
//         static bool ifFlag;
//         private static int beginEndCount;
//         static void Accept(byte symbolExpected)
//         {
//             if (LexicalAnalyzer.symbol == symbolExpected)
//             {
//                 LexicalAnalyzer.NextSym();
//             }
//             else
//             {
//                 InputOutput.Error(symbolExpected, LexicalAnalyzer.token);
//             }
//         }
//
//         static bool Belong(byte element, HashSet<byte> set)
//         {
//             return set.Contains(element);
//         }
//
//         static void SkipTo(HashSet<byte> where)
//         {
//             while (!Belong(LexicalAnalyzer.symbol, where))
//             {
//                 LexicalAnalyzer.NextSym();
//             }
//         }
//         static void SkipTo2(HashSet<byte> start, HashSet<byte> follow)
//         {
//             while (!(Belong(LexicalAnalyzer.symbol, start) || Belong(LexicalAnalyzer.symbol, follow)))
//             {
//                 LexicalAnalyzer.NextSym();
//             }
//         }
//
//         static void SetDisjunct(HashSet<byte> set1, HashSet<byte> set2, out HashSet<byte> set3)
//         {
//             set3 = new HashSet<byte>();
//             set3.UnionWith(set2);
//             set3.UnionWith(set1);
//         }
//
//         static public void Start()
//         {
//             if (ifStart)
//             {
//                 // SemanticAnalyzer.TreeNode temp = null; // добавляем новую область видимости новую голову
//                 // SemanticAnalyzer.heads.Push(temp);
//                 HashSet<byte> ptra = new HashSet<byte>();
//                 StFoll obj = new StFoll();
//                 SetDisjunct(obj.sf[StFoll.st_typepart], followers, out ptra); //?
//                 // union(followers, allCodes.begpart);                // add "var", "function" and the rest in followers
//                 followers.Add(LexicalAnalyzer.point); // ?
//
//                 LexicalAnalyzer.NextSym();
//                 ptra.Add(LexicalAnalyzer.programsy); // ?
//                 if (!Belong(LexicalAnalyzer.symbol, ptra))
//                 {
//                     InputOutput.Error(3, LexicalAnalyzer.token);
//                     SkipTo2(ptra, followers);
//                 }
//                 else
//                 {
//                     Accept(LexicalAnalyzer.programsy);
//                     Accept(LexicalAnalyzer.ident);
//                     Accept(LexicalAnalyzer.semicolon);
//                 }
//                 ifStart = false;
//             }
//             else
//             {
//                 Block(followers);
//                 if (!ifFlag)
//                 {
//                     LexicalAnalyzer.NextSym();
//                 }
//                 ifFlag = false; // ?
//             }
//         }
//
//         static void LabelPart() // раздел меток
//         {
//             if (LexicalAnalyzer.symbol == LexicalAnalyzer.labelsy)
//             {
//                 Accept(LexicalAnalyzer.labelsy);
//                 if (LexicalAnalyzer.symbol == LexicalAnalyzer.intc || LexicalAnalyzer.symbol == LexicalAnalyzer.ident)
//                 {
//                     LexicalAnalyzer.NextSym();
//                 }
//                 else
//                 {
//                     InputOutput.Error(26, LexicalAnalyzer.token);
//                 }
//
//                 while (LexicalAnalyzer.symbol == LexicalAnalyzer.comma)
//                 {
//                     LexicalAnalyzer.NextSym();
//                     if (LexicalAnalyzer.symbol == LexicalAnalyzer.intc || LexicalAnalyzer.symbol == LexicalAnalyzer.ident)
//                     {
//                         LexicalAnalyzer.NextSym();
//                     }
//                     else
//                     {
//                         InputOutput.Error(26, LexicalAnalyzer.token);
//                     }
//                 }
//                 Accept(LexicalAnalyzer.semicolon);
//             }
//         }
//
//         static void ConstPart(HashSet<byte> followers) //раздел констант, не знаю нужен ли, но пусть будет
//         {
//             if (LexicalAnalyzer.symbol == LexicalAnalyzer.constsy)
//             {
//                 Accept(LexicalAnalyzer.constsy);
//                 Accept(LexicalAnalyzer.ident);
//                 Accept(LexicalAnalyzer.equal);
//                 if (LexicalAnalyzer.symbol == LexicalAnalyzer.intc ||
//                     LexicalAnalyzer.symbol == LexicalAnalyzer.floatc ||
//                     LexicalAnalyzer.symbol == LexicalAnalyzer.charc) // ? либо строка
//                 {
//                     LexicalAnalyzer.NextSym();
//                 }
//
//                 while (LexicalAnalyzer.symbol == LexicalAnalyzer.semicolon)
//                 {
//                     LexicalAnalyzer.NextSym();
//                     if (LexicalAnalyzer.symbol == LexicalAnalyzer.typesy || LexicalAnalyzer.symbol == LexicalAnalyzer.varsy) // ?
//                     {
//                         break;
//                     }
//                     Accept(LexicalAnalyzer.ident);
//                     Accept(LexicalAnalyzer.equal);
//                     if (LexicalAnalyzer.symbol == LexicalAnalyzer.intc ||
//                         LexicalAnalyzer.symbol == LexicalAnalyzer.floatc ||
//                         LexicalAnalyzer.symbol == LexicalAnalyzer.charc)
//                     {
//                         LexicalAnalyzer.NextSym();
//                     }
//                     else
//                     {
//                         InputOutput.Error(26, LexicalAnalyzer.token);
//                     }
//
//                 }
//                 Accept(LexicalAnalyzer.semicolon);
//             }
//         }
//
//         static void Block(HashSet<byte> followers)
//         {
//             HashSet<byte> ptra;
//             StFoll obj = new StFoll();
//             if (!Belong(LexicalAnalyzer.symbol, obj.sf[StFoll.begpart]))
//             {
//                 InputOutput.Error(18, LexicalAnalyzer.token); // ошибка в разделе описаний
//                 SkipTo2(obj.sf[StFoll.begpart], followers);
//             }
//             if (Belong(LexicalAnalyzer.symbol, obj.sf[StFoll.begpart]))
//             {
//                 LabelPart();
//                 SetDisjunct(obj.sf[StFoll.st_typepart], followers, out ptra);
//                 ConstPart(ptra);
//                 SetDisjunct(obj.sf[StFoll.st_varpart], followers, out ptra);
//                 // TypePart(ptra); // а надо ли нам это?
//                 SetDisjunct(obj.sf[StFoll.st_procfuncpart], followers, out ptra);
//                 VarPart(ptra);
//                 //ProcFuncPart(ptra); // не реализовано
//                 StatPart(followers);
//                 if (!Belong(LexicalAnalyzer.symbol, followers))
//                 {
//                     InputOutput.Error(6); // запрещенный символ
//                     SkipTo(followers);
//                 }
//             }
//         }
//
//         static void BeginEnd() // составной
//         {
//             if (LexicalAnalyzer.beginsy == LexicalAnalyzer.symbol)
//             {
//                 ifFlag = true;
//                 beginEndCount++;
//                 LexicalAnalyzer.NextSym();
//             }
//             else if (LexicalAnalyzer.endsy == LexicalAnalyzer.symbol)
//             {
//                 ifFlag = true;
//                 beginEndCount--;
//                 LexicalAnalyzer.NextSym();
//                 if (beginEndCount != 0)
//                 {
//                     Accept(LexicalAnalyzer.semicolon);
//                 }
//                 else
//                 {
//                     Accept(LexicalAnalyzer.point);
//                 }
//             }
//         }
//
//         static void VarPart(HashSet<byte> followers)
//         {
//             HashSet<byte> ptra;
//             StFoll obj = new StFoll();
//             if (!Belong(LexicalAnalyzer.symbol, obj.sf[StFoll.st_varpart]))
//             {
//                 InputOutput.Error(18); // ошибка в разделе описаний
//                 SkipTo2(obj.sf[StFoll.st_varpart], followers);
//             }
//             if (LexicalAnalyzer.symbol == LexicalAnalyzer.varsy)
//             {
//                 LexicalAnalyzer.NextSym();
//                 SetDisjunct(obj.sf[StFoll.after_var], followers, out ptra);
//                 do
//                 {
//                     VarDeclaration(ptra);
//                     Accept(LexicalAnalyzer.semicolon);
//                 }
//                 while (LexicalAnalyzer.symbol == LexicalAnalyzer.ident);
//                 if (!Belong(LexicalAnalyzer.symbol, followers))
//                 {
//                     InputOutput.Error(6); // запрещенный символ
//                     SkipTo(followers);
//                 }
//             }
//         }
//
//         static void VarDeclaration(HashSet<byte> followers)
//         {
//             StFoll obj = new StFoll();
//             if (!Belong(LexicalAnalyzer.symbol, obj.sf[StFoll.id_starters]))
//             {
//                 InputOutput.Error(2);
//                 SkipTo2(obj.sf[StFoll.id_starters], followers);
//             }
//             if (LexicalAnalyzer.symbol == LexicalAnalyzer.ident)
//             {
//                 // InputOutput.varList = null;
//                 // NewVariable();
//                 LexicalAnalyzer.NextSym();
//                 while (LexicalAnalyzer.symbol == LexicalAnalyzer.comma)
//                 {
//                     LexicalAnalyzer.NextSym();
//                     // NewVariable();
//                     Accept(LexicalAnalyzer.ident);
//                 }
//                 Accept(LexicalAnalyzer.colon);
//                 // InputOutput.varType = Typе(followers);
//                 // внешняя переменная varType содержит адрес дескриптора типа для однотипных переменных
//                 // AddAttributes();
//                 if (LexicalAnalyzer.symbol == LexicalAnalyzer.arraysy)
//                 {
//                     // SemanticAnalyzer.tempIdTypeArray = SemanticAnalyzer.arrays; // тип идентификаторов массив
//                     ArrayType(followers); // массив?
//                 }
//                 else Type(followers);
//                 if (!Belong(LexicalAnalyzer.symbol, followers))
//                 {
//                     InputOutput.Error(6); // запрещенный символ
//                     SkipTo(followers);
//                 }
//             }
//         }
//
//         static void ArrayType(HashSet<byte> followers)
//         {
//             Accept(LexicalAnalyzer.arraysy);
//             Accept(LexicalAnalyzer.lbracket);
//             SimpleType();
//             while (LexicalAnalyzer.symbol == LexicalAnalyzer.comma)
//             {
//                 Accept(LexicalAnalyzer.comma);
//                 SimpleType();
//             }
//             Accept(LexicalAnalyzer.rbracket);
//             Accept(LexicalAnalyzer.ofsy);
//             Type(followers);
//         }
//
//         static void SimpleType()
//         {
//             StFoll obj = new StFoll();
//             if (!Belong(LexicalAnalyzer.symbol, obj.sf[StFoll.id_starters]))
//             {
//                 LexicalAnalyzer.NextSym();
//             }
//             else
//             {
//                 Accept(LexicalAnalyzer.intc);
//             }
//         }
//
//         static void Type(HashSet<byte> followers) // простые типы
//         {
//             StFoll obj = new StFoll();
//             if (!Belong(LexicalAnalyzer.symbol, obj.sf[StFoll.types]))
//             {
//                 // // SemanticAnalyzer.tempIdType = SemanticAnalyzer.scalars;
//                 // LexicalAnalyzer.NextSym();
//                 InputOutput.Error(10);
//                 SkipTo2(obj.sf[StFoll.types], followers);
//             }
//             else
//             {
//                 if (LexicalAnalyzer.symbol == LexicalAnalyzer.integersy)
//                 {
//                     Accept(LexicalAnalyzer.integersy);
//                 }
//                 else if (LexicalAnalyzer.symbol == LexicalAnalyzer.floatsy)
//                 {
//                     Accept(LexicalAnalyzer.floatsy);
//                 }
//                 else
//                 {
//                     Accept(LexicalAnalyzer.stringsy);
//                 }
//
//             }
//         }
//
//         static void StatPart(HashSet<byte> followers) // анализ конструкции <оператор>
//         {
//             if (LexicalAnalyzer.symbol == LexicalAnalyzer.intc) // семантический анализ метки
//             {
//                 LexicalAnalyzer.NextSym();
//                 Accept(LexicalAnalyzer.colon);
//             }
//             switch (LexicalAnalyzer.symbol)
//             {
//                 case LexicalAnalyzer.ident:
//                     // анализ оператора присваивания
//                     Assignment(followers);
//                     // else
//                     //     // анализ вызова процедуры
//                     //     CallProc(followers);
//                     break;
//                 case LexicalAnalyzer.beginsy:
//                     beginEndCount++;
//                     LexicalAnalyzer.NextSym();
//                     StatPart(followers);
//                     Accept(LexicalAnalyzer.endsy);
//                     if (beginEndCount == 0)
//                     {
//                         Accept(LexicalAnalyzer.point);
//                     }
//                     else
//                     {
//                         Accept(LexicalAnalyzer.semicolon);
//                     }
//                     break;
//                 //     CompoundStatement(followers); break;
//                 case LexicalAnalyzer.ifsy:
//                     IfStatement(followers); break;
//                 // case LexicalAnalyzer.whilesy:
//                 //     WhileStatement(followers); break;
//                 // case LexicalAnalyzer.repeatsy:
//                 //     RepeatStatement(followers); break;
//                 // case LexicalAnalyzer.forsy:
//                 //     ForStatement(followers); break;
//                 // case LexicalAnalyzer.casesy:
//                     // CaseStatement(followers); break;
//                 // case LexicalAnalyzer.withsy:
//                 //     WithStatement(followers); break;
//                 // case LexicalAnalyzer.gotosy:
//                 //     GotoStatement(followers); break;
//
//             }
//         }
//
//         static void IfStatement(HashSet<byte> followers) // условный оператор
//         {
//             if (LexicalAnalyzer.symbol == LexicalAnalyzer.ifsy)
//             {
//                 ifFlag = true;
//                 Accept(LexicalAnalyzer.ifsy);
//                 Expression(followers);
//
//                 Accept(LexicalAnalyzer.thensy);
//                 StatPart(followers);
//
//                 if (LexicalAnalyzer.symbol == LexicalAnalyzer.elsesy)
//                 {
//                     Accept(LexicalAnalyzer.elsesy);
//                     IfStatement(followers);
//                     Assignment(followers);
//                 }
//                 else StatPart(followers); // если это не else, то чтобы не потерять строку рекурсивно запускаем
//             }
//         }
//
//
//         static void Expression(HashSet<byte> ptra) // выражение
//         {
//             StFoll obj = new StFoll();
//             if (!Belong(LexicalAnalyzer.symbol, obj.sf[StFoll.st_expressions]))
//             {
//                 InputOutput.Error(3, LexicalAnalyzer.token);
//                 SkipTo2(ptra, followers);
//             }
//
//             if ( LexicalAnalyzer.symbol == LexicalAnalyzer.ident
//                  || obj.sf[StFoll.values].Contains(LexicalAnalyzer.symbol) )
//             {
//                 if(LexicalAnalyzer.symbol == LexicalAnalyzer.ident)
//                 {
//                     // if(SemanticAnalyzer.searchIdent(SemanticAnalyzer.name) is null)
//                     //     accept(LexicalAnalyzer.varsy, "Ошибка! Идентификатор не определен");
//                     // GeneratorCodes.push_reference(LexicalAnalyzer.integersy, 0, 017,
//                     //     (ulong)(SemanticAnalyzer.searchIdent(SemanticAnalyzer.name)).ofSet);
//                 }
//                 count++;
//                 LexicalAnalyzer.NextSym();
//
//                 while (obj.sf[StFoll.expressions].Contains(LexicalAnalyzer.symbol))
//                 {
//                     LexicalAnalyzer.NextSym();
//                     if (LexicalAnalyzer.symbol == LexicalAnalyzer.ident
//                         || obj.sf[StFoll.values].Contains(LexicalAnalyzer.symbol))
//                     {
//                         if (LexicalAnalyzer.symbol == LexicalAnalyzer.ident)
//                         {
//                             // if (SemanticAnalyzer.searchIdent(SemanticAnalyzer.name) is null)
//                             //     accept(LexicalAnalyzer.varsy, "Ошибка! Идентификатор не определен");
//                             // GeneratorCodes.push_reference(LexicalAnalyzer.integersy, 0, 017,
//                             // (ulong)(SemanticAnalyzer.searchIdent(SemanticAnalyzer.name)).ofSet);
//                             // GeneratorCodes.multop((ulong)LexicalAnalyzer.star, LexicalAnalyzer.integersy);
//                         }
//                         LexicalAnalyzer.NextSym();
//                     }
//                     else
//                     {
//                         Accept(LexicalAnalyzer.ident);
//                         break;
//                     }
//                 }
//             }
//         }
//
//         static void Assignment(HashSet<byte> followers) // присваивание
//         {
//             StFoll obj = new StFoll();
//             if (LexicalAnalyzer.symbol == LexicalAnalyzer.ident)
//             {
//                 // if (SemanticAnalyzer.searchIdent(SemanticAnalyzer.name) is null)
//                 //     Accept(LexicalAnalyzer.varsy);
//                 //
//                 ifFlag = true;
//                 LexicalAnalyzer.NextSym();
//                 if (obj.sf[StFoll.assigns].Contains(LexicalAnalyzer.symbol))
//                 {
//                     LexicalAnalyzer.NextSym();
//                     Expression(followers);
//                     Accept(LexicalAnalyzer.semicolon);
//                 }
//                 else
//                 {
//                     InputOutput.Error(LexicalAnalyzer.symbol, LexicalAnalyzer.token);
//                 }
//             }
//         }
//     }
// }