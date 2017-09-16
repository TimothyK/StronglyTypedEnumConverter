using System.Text;

namespace StronglyTypedEnumConverter
{
    /// <summary>
    /// Wrapper for StringBuilder that adds some C# specific functions
    /// </summary>
    internal class CSharpBuilder 
    {
        private readonly LanguageVersion _version;
        private readonly StringBuilder _builder = new StringBuilder();

        public CSharpBuilder(LanguageVersion version)
        {
            _version = version;            
        }

        public override string ToString() => _builder.ToString();

        public CSharpBuilder Append(string value)
        {
            _builder.Append(value);
            return this;
        }

        public CSharpBuilder AppendLine(string value = null)
        {
            _builder.AppendLine(value);
            return this;
        }

        public CSharpBuilder Indent(int count) => Append(new string(' ', count * 4));

        public CSharpBuilder NameOf(string value)
        {
            if (_version >= LanguageVersion.CSharp6)
                Append($"nameof({value})");
            else
                Append($"\"{value}\"");

            return this;
        }

        public CSharpBuilder ExpressionBody(string returnValue)
        {
            if (_version >= LanguageVersion.CSharp6)
                return Append($" => {returnValue};");

            AppendLine();
            Indent(1).AppendLine("{");
            Indent(2).AppendLine($"return {returnValue};");
            Indent(1).AppendLine("}");

            return this;
        }



    }
}
