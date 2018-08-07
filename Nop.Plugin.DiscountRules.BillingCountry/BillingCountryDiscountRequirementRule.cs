using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;

namespace Nop.Plugin.DiscountRules.BillingCountry
{
    public partial class BillingCountryDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        #region Fields

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IUrlHelperFactory _urlHelperFactory;

        #endregion

        #region Ctor

        public BillingCountryDiscountRequirementRule(IActionContextAccessor actionContextAccessor,
            ISettingService settingService,
            ILocalizationService localizationService,
            IUrlHelperFactory urlHelperFactory)
        {
            this._actionContextAccessor = actionContextAccessor;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._urlHelperFactory = urlHelperFactory;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Check discount requirement
        /// </summary>
        /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
        /// <returns>Result</returns>
        public DiscountRequirementValidationResult CheckRequirement(DiscountRequirementValidationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            //invalid by default
            var result = new DiscountRequirementValidationResult();

            if (request.Customer == null)
                return result;

            if (request.Customer.BillingAddress == null)
                return result;

            var billingCountryId = _settingService.GetSettingByKey<int>($"DiscountRequirement.BillingCountry-{request.DiscountRequirementId}");

            if (billingCountryId == 0)
                return result;

            result.IsValid = request.Customer.BillingAddress.CountryId == billingCountryId;

            return result;
        }

        /// <summary>
        /// Get URL for rule configuration
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <param name="discountRequirementId">Discount requirement identifier (if editing)</param>
        /// <returns>URL</returns>        
        public string GetConfigurationUrl(int discountId, int? discountRequirementId)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            return urlHelper.Action("Configure", "DiscountRulesBillingCountry",
                new { discountId = discountId, discountRequirementId = discountRequirementId }).TrimStart('/');
        }

        public override void Install()
        {
            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.BillingCountry.Fields.SelectCountry", "Select country");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.BillingCountry.Fields.Country", "Billing country");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.BillingCountry.Fields.Country.Hint", "Select required billing country.");
            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.DiscountRules.BillingCountry.Fields.SelectCountry");
            _localizationService.DeletePluginLocaleResource("Plugins.DiscountRules.BillingCountry.Fields.Country");
            _localizationService.DeletePluginLocaleResource("Plugins.DiscountRules.BillingCountry.Fields.Country.Hint");
            base.Uninstall();
        }

        #endregion
    }
}