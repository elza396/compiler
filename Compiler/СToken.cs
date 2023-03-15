using Compiler;

public enum TokenType
{
    ttIdent,
    ttKeyword,
    ttConst
}
public enum VariableType
{
    vartInteger,
    vartFloat,
    vartString,
    vartBoolean,
    vartUndef,
}

public abstract class CToken
{
    private TokenType type;
    private TextPosition position;
    protected byte code;

    protected CToken(TokenType type, TextPosition position, byte code)
    {
        this.type = type;
        this.position = position;
        this.code = code;
    }

    public byte Code { get => code; }
    public TokenType Type { get => type; }
    public TextPosition Position { get => position; }

    public abstract override string ToString();
}

public class CIdentToken : CToken
{
    private string name;
    private VariableType variable_type;
    public CIdentToken(TextPosition position, byte code, string name) : base(TokenType.ttIdent, position, code)
    {
        this.name = name;
    }

    public VariableType Variable_type { get => variable_type; set => variable_type = value; }
    public string Name { get => name; set => name = value; }

    public override string ToString()
    {
        return Name;
    }
}

public class CKeywordToken : CToken
{
    public CKeywordToken(TextPosition position, byte code) : base(TokenType.ttKeyword, position, code) { }

    public override string ToString()
    {
        return code.ToString();
    }
}

public class CConstToken : CToken
{
    CVariant cvalue;
    public CConstToken(TextPosition position, byte code, CVariant value) : base(TokenType.ttConst, position, code)
    {
        this.cvalue = value;
    }

    CVariant CValue { get => cvalue; }

    public override string ToString()
    {
        return cvalue.ToString();
    }
}