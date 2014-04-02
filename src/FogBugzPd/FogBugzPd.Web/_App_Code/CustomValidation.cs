using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using FogBugzPd.Web.Models.FbAccount;

namespace FogBugzPd.Web
{
	public class CustomValidation
	{
		public class UrlAttribute : RegularExpressionAttribute, IClientValidatable
		{
			public UrlAttribute()
				: base(@"(^https:\/\/([\da-zA-Z-]+)\.fogbugz\.com(\/.*|$))") { }

			public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
			{
				var rule = new ModelClientValidationRule
				{
					ValidationType = "customurl",
					ErrorMessage = "Url must point to a valid \"FogBugs On-Demand\" account"
				};
				yield return rule;

			}
		}

		public class GenericUrlAttribute : RegularExpressionAttribute, IClientValidatable
		{
			public GenericUrlAttribute()
				: base(@"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-?&]*)*\/?$") 
			{ }

			protected override ValidationResult IsValid(object value, ValidationContext context)
			{
				var viewModel = (SettingViewModel)context.ObjectInstance;

				if (!viewModel.AllowTestRail) return ValidationResult.Success;

				if (value != null) 
					return base.IsValid(value, context);

				return new ValidationResult("Field is required");
			}

			public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
			{
				var rule = new ModelClientValidationRule
				{
					ValidationType = "customurl",
					ErrorMessage = "Url should be valid"
				};
				yield return rule;

			}
		}

		public class TestRailConfigAttribute : ValidationAttribute, IClientValidatable
		{
			protected override ValidationResult IsValid(object value, ValidationContext context)
			{
				var viewModel = (SettingViewModel)context.ObjectInstance;

				if (!viewModel.AllowTestRail) return ValidationResult.Success;

				if (value != null) return ValidationResult.Success;

				return new ValidationResult("Field is required");
			}

			public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
			{
				var rule = new ModelClientValidationRule
				{
					ValidationType = "testrailconfig",
					ErrorMessage = ErrorMessage
				};

				yield return rule;
			}
		}

		[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
		public class PercentageValidationAttribute : ValidationAttribute, IClientValidatable
		{
			private const int MinPercentage = 0;
			private const int MaxPercentage = 100;

			public PercentageValidationAttribute()
				: base("Percentage is not valid.")
			{
			}

			internal class ModelClientPercentageRangeValidationRule : ModelClientValidationRule
			{
				public ModelClientPercentageRangeValidationRule(string errorMessage, int minPercentage, int maxPercentage)
				{
					ErrorMessage = errorMessage;

					ValidationType = "percentagerange";

					ValidationParameters.Add("minpercentage", minPercentage);
					ValidationParameters.Add("maxpercentage", maxPercentage);
				}

			}

			/// <summary>
			/// This is to handle the code in the server side. We can keep this because if for some scenarious
			/// the client side fails, this will be called for the validation.
			/// </summary>
			/// <param name="value"></param>
			/// <returns></returns>
			public override bool IsValid(object value)
			{
				int percentage = (int)value;
				if (percentage < MinPercentage || percentage > MaxPercentage)
					return false;
				return true;
			}


			#region IClientValidatable Members
			public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
			{
				var rule = new ModelClientPercentageRangeValidationRule
					("Percentage should be between 0 and 100.", MinPercentage, MaxPercentage);
				yield return rule;
			}
			#endregion
		}

		public class AllowParentCaseAttribute : ValidationAttribute, IClientValidatable
		{
			protected override ValidationResult IsValid(object value, ValidationContext context)
			{
				var viewModel = (SettingViewModel)context.ObjectInstance;

				if (!viewModel.AllowSubProjects) return ValidationResult.Success;

				if (value != null) return ValidationResult.Success;

				return new ValidationResult("Field is required");
			}

			public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
			                                                                       ControllerContext context)
			{
				var rule = new ModelClientValidationRule
					{
						ValidationType = "allowparentcase",
						ErrorMessage = ErrorMessage
					};

				yield return rule;
			}
		}

		public class QaPercentageAttribute : ValidationAttribute, IClientValidatable
		{
			private const string DefaultPattern = @"^\d{1,3}?$";
			private const string MessageRequired = "QA Percentage is required";
			private const string MessageIncorrectFormat = "QA Percentage has incorrect format";
			private const string MessageOutOfRange = "QA Percentage is out of valid range";
			private const int MinValue = 0;
			private const int MaxValue = 100;

			private readonly Regex _regExpression;



			public QaPercentageAttribute(string pattern = DefaultPattern)
			{
				_regExpression = new Regex(pattern, RegexOptions.Compiled);
			}

			public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
			{
				var rule = new ModelClientValidationRule
				{
					ValidationType = "qapercentage",
					ErrorMessage = ErrorMessage
				};

				yield return rule;
			}

			protected override ValidationResult IsValid(object value, ValidationContext context)
			{
				var viewModel = (SettingViewModel)context.ObjectInstance;

				if (viewModel.AllowQaEstimates) return ValidationResult.Success;

				if (value == null)
					return new ValidationResult(MessageRequired);

				if (!_regExpression.IsMatch(value.ToString()))
					return new ValidationResult(MessageIncorrectFormat);

				var intVal = 0;

				if (int.TryParse(value.ToString(), out intVal))
				{
					if (intVal > MaxValue || intVal < MinValue)
						return new ValidationResult(MessageOutOfRange);
				}

				return ValidationResult.Success;
			}

		}

		public class QaCustomField : RequiredAttribute, IClientValidatable
		{
			public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
			{
				var rule = new ModelClientValidationRule
				{
					ValidationType = "qacustomfield",
					ErrorMessage = ErrorMessage
				};

				yield return rule;
			}

			protected override ValidationResult IsValid(object value, ValidationContext context)
			{
				var viewModel = (SettingViewModel)context.ObjectInstance;

				if (!viewModel.AllowQaEstimates)
					return ValidationResult.Success;

				return base.IsValid(value, context);
			}

		}

		public class EmailsList : RegularExpressionAttribute, IClientValidatable
		{
			public EmailsList()
				: base(@"(([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6})(,(([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}))*")
			{ }

			protected override ValidationResult IsValid(object value, ValidationContext context)
			{
				var viewModel = (SettingViewModel)context.ObjectInstance;

				if (!viewModel.AllowSendDailyDigestEmails) return ValidationResult.Success;

				if (value != null)
					return base.IsValid(value, context);

				return new ValidationResult("Field is required");
			}

			public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
			{
				var rule = new ModelClientValidationRule
				{
					ValidationType = "emailslist",
					ErrorMessage = "Enter valid emails comma separated"
				};
				yield return rule;

			}
		}
	}
}