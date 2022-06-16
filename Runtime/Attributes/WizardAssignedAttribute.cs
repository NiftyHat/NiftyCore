using UnityEngine;

namespace NiftyFramework.Core
{
    public class WizardAssignedAttribute : PropertyAttribute
    {
        private System.Type _wizardType;

        public WizardAssignedAttribute(System.Type wizardType)
        {
            _wizardType = wizardType;
        }
    }
}