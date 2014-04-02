namespace FogBugzPd.Infrastructure.Razor
{
	public abstract class ModelTemplateBase<TModel>:TemplateBase where TModel:class
	{
		public TModel Model { get; set; }
	}
}
