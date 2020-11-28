using System;

namespace Archmage.Common {
    public abstract class Node {
        public abstract float Evaluate(IContext context);
    }

    public class NodeNumber : Node {
        private float _number;

        public NodeNumber(float number) {
            _number = number;
        }

        public override float Evaluate(IContext context) {
            return _number;
        }
    }

    public class NodeBinary : Node {
        private Node _left;
        private Node _right;
        private Func<float, float, float> _operation;

        public NodeBinary(Node left, Node right, Func<float, float, float> operation) {
            _left = left;
            _right = right;
            _operation = operation;
        }

        public override float Evaluate(IContext context) {
            return _operation(_left.Evaluate(context), _right.Evaluate(context));
        }
    }

    public class NodeUnary : Node {
        private Node _right;
        private Func<float, float> _operation;

        public NodeUnary(Node right, Func<float, float> operation) {
            _right = right;
            _operation = operation;
        }

        public override float Evaluate(IContext context) {
            return _operation(_right.Evaluate(context));
        }
    }

    public class NodeVariable : Node {
        private string _variableName;

        public NodeVariable(string variableName) {
            _variableName = variableName;
        }

        public override float Evaluate(IContext context) {
            return context.ResolveVariable(_variableName);
        }
    }

    public class NodeFunctionCall : Node {
        private string _functionName;
        private Node[] _arguments;

        public NodeFunctionCall(string functionName, Node[] arguments) {
            _functionName = functionName;
            _arguments = arguments;
        }

        public override float Evaluate(IContext context) {
            var argValues = new float[_arguments.Length];
            for(int i = 0; i < argValues.Length; i++) {
                argValues[i] = _arguments[i].Evaluate(context);
            }
            return context.CallFunction(_functionName, argValues);
        }
    }
}
