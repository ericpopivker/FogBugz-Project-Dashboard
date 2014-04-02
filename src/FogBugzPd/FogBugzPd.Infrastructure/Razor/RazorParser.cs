using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Web.Razor;
using Microsoft.CSharp;

namespace FogBugzPd.Infrastructure.Razor
{
	public class RazorParser
	{
		private static RazorEngineHost GetHost<T>() where T:TemplateBase 
		{
			var host = new RazorEngineHost(new CSharpRazorCodeLanguage());
			host.DefaultBaseClass = typeof(T).FullName;
			host.DefaultNamespace = "RazorOutput";
			host.DefaultClassName = "Template";
			host.NamespaceImports.Add("System");
			host.NamespaceImports.Add("System.IO");
			return host;
		}

		private static RazorTemplateEngine GetTemplateEngine<T>() where T:TemplateBase
		{
			var host = GetHost<T>();
			return new RazorTemplateEngine(host);
		}

		public static string Parse(string razor)
		{
			var engine = GetTemplateEngine<TemplateBase>();

			return Parse(engine, razor);
		}

		public static string Parse<T>(string razor)where T:TemplateBase
		{
			var engine = GetTemplateEngine<T>();

			return Parse(engine, razor);
		}

		public static string Parse<TModel>(TModel model,string razor) where TModel:class
		{
			var engine = GetTemplateEngine<ModelTemplateBase<TModel>>();

			return Parse(engine, razor, model);
		}

		private static string Parse(RazorTemplateEngine engine, string razor)
		{
			var type = GenerateTemplateType(engine, razor);

			var newTemplate = Activator.CreateInstance(type) as TemplateBase;

			if (newTemplate == null)
			{
				throw new Exception("Error loading type");
			}

			newTemplate.Execute();

			var html = newTemplate.Buffer.ToString();

			newTemplate.Buffer.Clear();

			return html;
		}

		private static string Parse<TModel>(RazorTemplateEngine engine, string razor, TModel model) where TModel : class
		{
			var type = GenerateTemplateType(engine, razor);

			var newTemplate = Activator.CreateInstance(type) as ModelTemplateBase<TModel>;

			if (newTemplate == null)
			{
				throw new Exception("Error loading type");
			}

			newTemplate.Model = model;

			newTemplate.Execute();

			var html = newTemplate.Buffer.ToString();

			newTemplate.Buffer.Clear();

			return html;
		}

		private static Type GenerateTemplateType(RazorTemplateEngine engine,string razor)
		{
			GeneratorResults razorResult;
			using (TextReader rdr = new StringReader(razor))
			{
				razorResult = engine.GenerateCode(rdr);
			}

			var codeProvider = new CSharpCodeProvider();

			var parameters = new CompilerParameters();
			parameters.GenerateInMemory = true;

			var assemblies = AppDomain.CurrentDomain
							.GetAssemblies()
							.Where(a => !a.IsDynamic)
							.Select(a => a.Location);   

			parameters.ReferencedAssemblies.AddRange(assemblies.ToArray());

			//if (!String.IsNullOrEmpty(requiredAssemblyFileName))
			//{
			//    parameters.ReferencedAssemblies.Add(requiredAssemblyFileName);
			//}

			var result = codeProvider.CompileAssemblyFromDom(parameters, razorResult.GeneratedCode);

			if (result.Errors.HasErrors)
			{
				var message = "";
				var count = result.Errors.Count;
				for (var i = 0; i < count;i++ )
				{
					message += result.Errors[i].ErrorText + "\r\n";
				}
					throw new Exception("Compilation Error",new Exception(message));
			}

			var assembly = result.CompiledAssembly;

			var type = assembly.GetType("RazorOutput.Template");

			return type;
		}
	}
}
