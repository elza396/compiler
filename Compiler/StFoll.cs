using System.Collections.Generic;
namespace Compiler
{
    class StFoll
    {
        public HashSet<byte> [] sf = new HashSet<byte>[30];

        public const byte
            begpart = 0,
            st_typepart = 1,
            st_varpart = 2,
            st_procfuncpart = 3,
            id_starters = 4,
            after_var = 5,
            values = 6,
            types = 7,
            expressions = 8,
            st_expressions = 9,
            after_expressions = 10,
            assigns = 11,
            statement = 12,
            comparisonOperators = 13,
            multiplyingOperators = 14,
            addingOperators = 15;

        public StFoll()
        {
            sf[begpart] = new HashSet<byte>();
            sf[begpart].Add(LexicalAnalyzer.labelsy);
            sf[begpart].Add(LexicalAnalyzer.constsy);
            sf[begpart].Add(LexicalAnalyzer.typesy);
            sf[begpart].Add(LexicalAnalyzer.varsy);
            sf[begpart].Add(LexicalAnalyzer.functionsy);
            sf[begpart].Add(LexicalAnalyzer.procedurensy);
            sf[begpart].Add(LexicalAnalyzer.beginsy);

            sf[st_typepart] = new HashSet<byte>();
            sf[st_typepart].Add(LexicalAnalyzer.typesy);
            sf[st_typepart].Add(LexicalAnalyzer.varsy);
            sf[st_typepart].Add(LexicalAnalyzer.functionsy);
            sf[st_typepart].Add(LexicalAnalyzer.procedurensy);
            sf[st_typepart].Add(LexicalAnalyzer.beginsy);

            sf[st_varpart] = new HashSet<byte>();
            sf[st_varpart].Add(LexicalAnalyzer.varsy);
            sf[st_varpart].Add(LexicalAnalyzer.functionsy);
            sf[st_varpart].Add(LexicalAnalyzer.procedurensy);
            sf[st_varpart].Add(LexicalAnalyzer.beginsy);

            sf[st_procfuncpart] = new HashSet<byte>();
            sf[st_procfuncpart].Add(LexicalAnalyzer.functionsy);
            sf[st_procfuncpart].Add(LexicalAnalyzer.procedurensy);
            sf[st_procfuncpart].Add(LexicalAnalyzer.beginsy);

            sf[id_starters] = new HashSet<byte>();
            sf[id_starters].Add(LexicalAnalyzer.ident);

            sf[after_var] = new HashSet<byte>();
            sf[after_var].Add(LexicalAnalyzer.semicolon);

            sf[values] = new HashSet<byte>();
            sf[values].Add(LexicalAnalyzer.ident);
            sf[values].Add(LexicalAnalyzer.intc);
            sf[values].Add(LexicalAnalyzer.floatc);
            sf[values].Add(LexicalAnalyzer.charc);
            sf[values].Add(LexicalAnalyzer.truesy);
            sf[values].Add(LexicalAnalyzer.falsesy);
            sf[values].Add(LexicalAnalyzer.leftpar);
            sf[values].Add(LexicalAnalyzer.plus);
            sf[values].Add(LexicalAnalyzer.minus);

            sf[types] = new HashSet<byte>();
            sf[types].Add(LexicalAnalyzer.integersy);
            sf[types].Add(LexicalAnalyzer.floatsy);
            sf[types].Add(LexicalAnalyzer.stringsy);
            sf[types].Add(LexicalAnalyzer.charsy);
            sf[types].Add(LexicalAnalyzer.arraysy);
            sf[types].Add(LexicalAnalyzer.booleansy);

            sf[st_expressions] = new HashSet<byte>();
            sf[st_expressions].Add(LexicalAnalyzer.ident);
            sf[st_expressions].Add(LexicalAnalyzer.intc);
            sf[st_expressions].Add(LexicalAnalyzer.floatc);
            sf[st_expressions].Add(LexicalAnalyzer.falsesy);
            sf[st_expressions].Add(LexicalAnalyzer.truesy);
            sf[st_expressions].Add(LexicalAnalyzer.stringc);
            sf[st_expressions].Add(LexicalAnalyzer.charc);
            sf[st_expressions].Add(LexicalAnalyzer.leftpar);

            sf[after_expressions] = new HashSet<byte>();
            sf[after_expressions].Add(LexicalAnalyzer.semicolon);
            sf[after_expressions].Add(LexicalAnalyzer.tosy);
            sf[after_expressions].Add(LexicalAnalyzer.downtosy);
            sf[after_expressions].Add(LexicalAnalyzer.dosy);
            sf[after_expressions].Add(LexicalAnalyzer.comma);

            sf[expressions] = new HashSet<byte>();
            sf[expressions].Add(LexicalAnalyzer.star);
            sf[expressions].Add(LexicalAnalyzer.slash);
            sf[expressions].Add(LexicalAnalyzer.equal);
            sf[expressions].Add(LexicalAnalyzer.plus);
            sf[expressions].Add(LexicalAnalyzer.minus);
            sf[expressions].Add(LexicalAnalyzer.latergreater);
            sf[expressions].Add(LexicalAnalyzer.greaterequal);
            sf[expressions].Add(LexicalAnalyzer.laterequal);
            sf[expressions].Add(LexicalAnalyzer.greater);
            sf[expressions].Add(LexicalAnalyzer.later);
            sf[expressions].Add(LexicalAnalyzer.arrow);
            sf[expressions].Add(LexicalAnalyzer.notsy);
            sf[expressions].Add(LexicalAnalyzer.andsy);
            sf[expressions].Add(LexicalAnalyzer.orsy);
            sf[expressions].Add(LexicalAnalyzer.divsy);
            sf[expressions].Add(LexicalAnalyzer.modsy);

            sf[assigns] = new HashSet<byte>();
            sf[assigns].Add(LexicalAnalyzer.assign);
            sf[assigns].Add(LexicalAnalyzer.equalStar);
            sf[assigns].Add(LexicalAnalyzer.equalSlash);
            sf[assigns].Add(LexicalAnalyzer.equalPlus);
            sf[assigns].Add(LexicalAnalyzer.equalMinus);

            sf[statement] = new HashSet<byte>();
            sf[statement].Add(LexicalAnalyzer.ident);
            sf[statement].Add(LexicalAnalyzer.forsy);
            sf[statement].Add(LexicalAnalyzer.whilesy);
            sf[statement].Add(LexicalAnalyzer.repeatsy);
            sf[statement].Add(LexicalAnalyzer.beginsy);

            sf[comparisonOperators] = new HashSet<byte>();
            sf[comparisonOperators].Add(LexicalAnalyzer.latergreater);
            sf[comparisonOperators].Add(LexicalAnalyzer.greaterequal);
            sf[comparisonOperators].Add(LexicalAnalyzer.laterequal);
            sf[comparisonOperators].Add(LexicalAnalyzer.greater);
            sf[comparisonOperators].Add(LexicalAnalyzer.later);
            sf[comparisonOperators].Add(LexicalAnalyzer.equal);

            sf[multiplyingOperators] = new HashSet<byte>();
            sf[multiplyingOperators].Add(LexicalAnalyzer.star);
            sf[multiplyingOperators].Add(LexicalAnalyzer.slash);
            sf[multiplyingOperators].Add(LexicalAnalyzer.modsy);
            sf[multiplyingOperators].Add(LexicalAnalyzer.divsy);
            sf[multiplyingOperators].Add(LexicalAnalyzer.andsy);

            sf[addingOperators] = new HashSet<byte>();
            sf[addingOperators].Add(LexicalAnalyzer.plus);
            sf[addingOperators].Add(LexicalAnalyzer.minus);
            sf[addingOperators].Add(LexicalAnalyzer.orsy);
        }

    }
}