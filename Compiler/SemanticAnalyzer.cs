// using System;
// using System.Collections.Generic;
// namespace Compiler
// {
//     class SemanticAnalyzer
//     {
//         public const UInt16
//             progs = 300,
//             types = 301,
//             consts = 302,
//             vars = 303,
//             procs = 304,
//             funcs = 305,
//
//             scalars = 401, /* cтандартный скалярный тип */
//             limiteds = 402, /* ограниченный тип */
//             enums = 403, /* перечислимый тип */
//             arrays = 404, /* регулярный тип (массив) */
//             references = 405, /* ссылочный тип */
//             sets = 406, /* множественный тип */
//             files = 407, /* файловый тип */
//             records = 408; /* комбинированный тип (запись)*/
//
//         class PrFunAsPar
//         {
//             ? /* информация о типе параметра параметра-процедуры, (параметра-функции) */;
//             PrFunAsPar linkPp /* указатель на следующий элемент */;
//         }
//
//         class IdParam // информация о параметрах
//         {
//               ? typeParam; /* информация о типе параметра */
//               int metTransf; /* способ передачи параметра */
//               IdParam linkParam; /* указатель на информацию о следующем параметре */
//               PrFunAsPar par /* указатель на информацию о параметрах параметра-процедуры (функции) */;
//         }
//
//         public class TreeNode
//         {
//             public byte hashValue;  // значение hash-функции
//             public string idName; // адрес имени в таблице имен
//             public UInt16 klass; // способ использования идентификатора
//             public TypeRec idType; // информация о типе
//             public ConstVal constValue; // значение константы
//             // для процедур (функций)
//             public IdParam param; // указатель на информацию о параметрах
//             public bool forw;
//
//             public TreeNode leftLink, rightLink;
//         }
//
//         public class Scope
//         {
//             public TreeNode firstLocal; /* указатель на ТИ области действия */
//             public TypeRec typeChain; /* указатель на таблицу типов области действия */
//             public LabelList labelPointer; /* указатель на таблицу меток области действия */
//             public Scope enclosingScope; /* указатель на элемент стека области действия, непосредственно объемлющей данную */
//         }
//
//
//         TreeNode newIdent(byte hashfunc, // значение hash-функции для идентификатора
//                           string addrName, // адрес имени в таблице имен
//                           UInt16 classUsed /* способ использования идентификатора */)
//         {
//             /* Обработка определяющего вхождения идентификатора;
//             если идентификатор с заданным способом использования
//             ранее был включен в дерево, то результат — null,
//             иначе — ссылка на вершину дерева, соответствующую
//             этому идентификатору */
//             return null;
//         }
//
//         TreeNode searchIdent
//                         (byte hashfunc /* значение hash-функции для идентификатора */,
//                          string addrName /* адрес имени в таблице имен*/,
//                          UInt16[] setOfClass /* множество возможных способов использования идентификатора */)
//         {
//             /*Обработка прикладного вхождения идентификатора —
//             поиск соответствующего ему определяющего вхождения.
//             Результат функции — указатель на вершину дерева,
//             соответствующую данному идентификатору*/
//             return указатель;
//         }
//
//         public static void OpenScope() /* создание элемента стека для текущей области действия */
//         {
//             Scope newScope = new Scope();  /* указатель на новый элемент стека */;
//             newScope.firstLocal = null;
//             newScope.typeChain = null;
//             newScope.labelPointer = null;
//             newScope.enclosingScope = InputOutput.localScope;
//             InputOutput.localScope = newScope;
//         }
//
//         public static void CloseScope() /* удаление таблиц текущей области действия */
//         {
//             InputOutput.localScope = InputOutput.localScope.enclosingScope;
//         }
//
//         public class TypeRec
//         {
//             public TypeRec next;
//             public UInt16 typeCode;
//             public VariaPart caseType;
//         }
//
//         public class LabelList
//         {
//             int meaning /* значение метки */;
//             LabelList nextLabel /* указатель на следующий элемент списка */;
//         }
//
//         public static TypeRec NewType(UInt16 tCode)
//         {
//             TypeRec nw = new TypeRec(); /* указатель на дескриптор типа */
//             nw.typeCode = tCode;
//             nw.next = InputOutput.localScope.typeChain;
//             switch (nw.typeCode)
//             {
//                 case limiteds:
//                     nw.caseType.baseType = null;
//                     break;
//                 case scalars: break;
//                 case enums:
//                     nw.caseType.firstConst = null;
//                     break;
//                 case sets:
//                 case files:
//                 case references:
//                     nw.caseType.baseType = null;
//                     break;
//                 case arrays:
//                     nw.caseType.arrayType.baseType = null;
//                     nw.caseType.arrayType.indexType = null;
//                     break;
//                 case records:
//                     nw.caseType.fields = null;
//                     break;
//             }
//             InputOutput.localScope.typeChain = nw;
//             return nw;
//         }
//     }
// }