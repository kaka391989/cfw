using CFW.Core.DomainResults;
using CFW.Core.Services.Validations;

namespace CFW.Core.Services.Validations
{
    public class ValidationService
    {
        private readonly IServiceProvider _serviceProvider;
        public ValidationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<DomainResult?> Validate<TModel>(IRuleDefinition<TModel> ruleDefinition, TModel request)
        {
            ruleDefinition.DefineRules(request);
            if (!ruleDefinition.RuleFactories.Any())
            {
                return DomainResult.Success();
            }

            var rules = ruleDefinition.RuleFactories.Select(x => x(_serviceProvider)).ToList();
            return await ExecuteAndStoreRules<IValidationRule, DomainResult>(rules);
        }

        public async Task<TResponse?> ExecuteAndStoreRules<TRequest, TResponse>(IEnumerable<IValidationRule> rules)
        {
            if (!rules.Any())
            {
                return default;
            }

            foreach (var rule in rules)
            {
                var isValid = await rule.ValidateAsync();
                if (!isValid)
                {
                    return CreateFailedResponse<TResponse>(rule.ErrorMessage);
                }
            }

            return default;
        }

        private TResponse CreateFailedResponse<TResponse>(string errorMessage)
        {
            throw new NotImplementedException();
        }
    }
}
