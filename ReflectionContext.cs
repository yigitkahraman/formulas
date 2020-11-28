using System.IO;
using System.Linq;

namespace Archmage.Common {
    public class ReflectionContext : IContext {
        private object _target;

        public ReflectionContext(object target) {
            _target = target;
        }

        public float ResolveVariable(string name) {
            var property = _target.GetType().GetProperty(name);
            if(property == null) throw new InvalidDataException($"Unknown variable '{name}'");
            return (float)property.GetValue(_target);
        }

        public float CallFunction(string name, float[] args) {
            var function = _target.GetType().GetMethod(name);
            if(function == null) throw new InvalidDataException($"Unknown function '{name}'");
            var arguments = args.Select(x => (object)x).ToArray();
            return (float)function.Invoke(_target, arguments);
        }
    }
}
