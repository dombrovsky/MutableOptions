namespace Microsoft.Extensions.Options.Mutable.Internal
{
    internal interface IOptionsMutatorChangeTokenSource<out TOptions> : IOptionsChangeTokenSource<TOptions>
    {
        void OnMutated();
    }
}