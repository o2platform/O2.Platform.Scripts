// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using O2.Interfaces.FrameworkSupport.J2EE;

namespace O2.Core.FileViewers.J2EE
{
    [Serializable]
    class KValidationXml : IValidationXml
    {
        public List<IValidation_Form> forms { get; set; }

        public KValidationXml()
        {
            forms = new List<IValidation_Form>();
        }
    }

    [Serializable]
    class KValidation_Form : IValidation_Form
    {
        public string name { get; set; }
        public Dictionary<string, IValidation_Form_Field> fields { get; set; }
        public KValidation_Form()
        {
            fields = new Dictionary<string, IValidation_Form_Field>();
        }
        public override string ToString()
        {
            return name;
        }
    }

    [Serializable]
    class KValidation_Form_Field : IValidation_Form_Field
    {
        public string property { get; set; }
        public string depends { get; set; }
        public Dictionary<string, string> vars { get; set; }
        public KValidation_Form_Field()
        {
            vars = new Dictionary<string, string>();
        }
    }
}
