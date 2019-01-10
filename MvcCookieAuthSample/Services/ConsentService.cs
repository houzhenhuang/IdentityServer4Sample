using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using MvcCookieAuthSample.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCookieAuthSample.Services
{
    public class ConsentService
    {
        private readonly IIdentityServerInteractionService _identityServerInteractionService;
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        public ConsentService(IIdentityServerInteractionService identityServerInteractionService,
            IClientStore clientStore, IResourceStore resourceStore)
        {
            this._identityServerInteractionService = identityServerInteractionService;
            this._clientStore = clientStore;
            this._resourceStore = resourceStore;
        }

        #region Private Methods
        private ConsentViewModel CreateConsentViewModel(AuthorizationRequest request, Client client, Resources resources, InputConsentViewModel model)
        {
            var rememberConsent = model?.RememberConsent ?? true;
            var selectedScope = model?.ScopesConsented ?? Enumerable.Empty<string>();
            var vm = new ConsentViewModel();
            vm.ClientId = client.ClientId;
            vm.ClientName = client.ClientName;
            vm.ClientLogoUrl = client.LogoUri;
            vm.ClientUrl = client.ClientUri;
            vm.RememberConsent = rememberConsent;//client.AllowRememberConsent;
            vm.IdentityScopes = resources.IdentityResources.Select(p => CreateScopeViewModel(p, (selectedScope.Contains(p.Name) || model == null)));
            vm.ResourceScopes = resources.ApiResources.SelectMany(i => i.Scopes).Select(p => CreateScopeViewModel(p, (selectedScope.Contains(p.Name) || model == null)));

            return vm;
        }
        private ScopeViewModel CreateScopeViewModel(IdentityResource identityResource, bool ckeck)
        {
            return new ScopeViewModel
            {
                Name = identityResource.Name,
                DisplayName = identityResource.DisplayName,
                Description = identityResource.Description,
                Checked = ckeck || identityResource.Required,
                Emphasize = identityResource.Emphasize,
                Required = identityResource.Required
            };
        }
        private ScopeViewModel CreateScopeViewModel(Scope scope, bool check)
        {
            return new ScopeViewModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                Checked = check || scope.Required,
                Emphasize = scope.Emphasize,
                Required = scope.Required
            };
        }
        #endregion
        public async Task<ConsentViewModel> BuildConsentViewModel(string returnUrl, InputConsentViewModel model = null)
        {
            var request = await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);
            if (request is null) return null;
            var client = await _clientStore.FindClientByIdAsync(request.ClientId);
            var resource = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);
            var vm = CreateConsentViewModel(request, client, resource, model);
            vm.ReturnUrl = returnUrl;
            return vm;
        }

        public async Task<ProcessConsentResult> ProcessConsent(InputConsentViewModel viewModel)
        {
            ConsentResponse consentResponse = null;
            var result = new ProcessConsentResult();
            if (viewModel.Button == "no")
            {
                consentResponse = ConsentResponse.Denied;
            }
            else if (viewModel.Button == "yes")
            {
                if (viewModel.ScopesConsented != null && viewModel.ScopesConsented.Any())
                {
                    consentResponse = new ConsentResponse
                    {
                        RememberConsent = viewModel.RememberConsent,
                        ScopesConsented = viewModel.ScopesConsented
                    };
                }
            }

            if (consentResponse != null)
            {
                var request = await _identityServerInteractionService.GetAuthorizationContextAsync(viewModel.ReturnUrl);
                await _identityServerInteractionService.GrantConsentAsync(request, consentResponse);

                result.ReturnUrl = viewModel.ReturnUrl;
            }
            else
            {
                result.ValidationError = "请至少选择一个权限";
                var consentViewModel = await BuildConsentViewModel(viewModel.ReturnUrl, viewModel);
                result.ConsentViewModel = consentViewModel;
            }
            return result;
        }
    }
}
