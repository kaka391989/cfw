using CFW.Core.Services.Validations;

namespace CFW.Core.Services.Validations
{
    public interface IRuleDefinition<TValidationModel>
    {
        void DefineRules(TValidationModel model);

        List<Func<IServiceProvider, IValidationRule>> RuleFactories { get; }

    }

    public interface IValidationRule
    {
        public Task<bool> ValidateAsync();

        public string ErrorMessage { get; }
    }
}
