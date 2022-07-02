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
            assigns = 9;

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

            sf[types] = new HashSet<byte>();
            sf[types].Add(LexicalAnalyzer.integersy);
            sf[types].Add(LexicalAnalyzer.floatsy);
            sf[types].Add(LexicalAnalyzer.stringsy);

            sf[expressions] = new HashSet<byte>();
            sf[expressions].Add(LexicalAnalyzer.ident);
            sf[expressions].Add(LexicalAnalyzer.intc);
            sf[expressions].Add(LexicalAnalyzer.floatc);
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
        }

    }
}