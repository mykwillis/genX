using System;
using System.Collections;

namespace genX
{
	/// <summary>
	/// Summary description for Designer.
	/// </summary>
	internal class GADesigner : System.ComponentModel.Design.ComponentDesigner
	{        
        
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);
#if NOTDEF
            // We add a design-time property called TrackSelection that is used to track
            // the active selection.  If the user sets this to true (the default), then
            // we will listen to selection change events and update the control's active
            // control to point to the current primary selection.
            GA ga = (GA) Component;

            if ( ga.EncodingType == EncodingType.Custom )
            {
                properties.Remove("ChromosomeLength");
            }
            if ( ga.EncodingType != EncodingType.Integer )
            {
                properties.Remove("MaxIntValue");
                properties.Remove("MinIntValue");
            }
            if ( ga.EncodingType != EncodingType.Real )
            {
                properties.Remove("MaxDoubleValue");
                properties.Remove("MinDoubleValue");
            }
//            properties["TrackSelection"] = TypeDescriptor.CreateProperty(
//                this.GetType(),   // the type this property is defined on
//                "TrackSelection", // the name of the property
//                typeof(bool),   // the type of the property
//                new Attribute[] {CategoryAttribute.Design});  // attributes
#endif
        }
	}

}
