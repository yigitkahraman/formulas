namespace Archmage.Common {
    public interface IContext {
        float ResolveVariable(string name);
        float CallFunction(string name, float[] arguments);
    }
}
