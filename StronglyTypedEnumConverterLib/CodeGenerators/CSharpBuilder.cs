using System.Text;

namespace StronglyTypedEnumConverter
{
    /// <summary>
    /// Wrapper for StringBuilder that adds some C# specific functions
    /// </summary>
    internal class CSharpBuilder 
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public CSharpBuilder()
        {
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

        public CSharpBuilder NameOf(string value) => Append($"nameof({value})");

        public CSharpBuilder ExpressionBody(string returnValue) => Append($" => {returnValue};");



    }
}
