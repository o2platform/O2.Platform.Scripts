// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using Mono.Cecil;
using Mono.Cecil.Cil;
using ICSharpCode.NRefactory.TypeSystem;

//O2File:CecilUtils.cs

namespace O2.External.O2Mono.MonoCecil
{
    public class MethodCalled
    {
        public IMemberReference memberReference { get; set;}
        public SequencePoint sequencePoint { get; set; }

        public MethodCalled(IMemberReference _memberReference, SequencePoint _sequencePoint)
        {
            memberReference = _memberReference;
            sequencePoint = _sequencePoint;
        }
    }
}
