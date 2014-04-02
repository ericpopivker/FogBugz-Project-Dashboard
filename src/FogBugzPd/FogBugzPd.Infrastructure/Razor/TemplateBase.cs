using System.ComponentModel;
using System.IO;
using System.Text;

namespace FogBugzPd.Infrastructure.Razor
{
	public abstract class TemplateBase
	{
		[Browsable(false)]
		public StringBuilder Buffer { get; set; }

		[Browsable(false)]
		public StringWriter Writer { get; set; }

		protected TemplateBase()
		{
			Buffer = new StringBuilder();
			Writer = new StringWriter(Buffer);
		}

		public abstract void Execute();

		// Writes the results of expressions like: "@foo.Bar"
		public virtual void Write(object value)
		{
			// Don't need to do anything special
			// Razor for ASP.Net does HTML encoding here.
			WriteLiteral(value);
		}

		// Writes literals like markup: "<p>Foo</p>"
		public virtual void WriteLiteral(object value)
		{
			Buffer.Append(value);
		}
	}
}
