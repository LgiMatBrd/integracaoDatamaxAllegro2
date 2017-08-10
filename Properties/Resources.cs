using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace VNJIngressos.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (VNJIngressos.Properties.Resources.resourceMan == null)
          VNJIngressos.Properties.Resources.resourceMan = new ResourceManager("VNJIngressos.Properties.Resources", typeof (VNJIngressos.Properties.Resources).Assembly);
        return VNJIngressos.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get
      {
        return VNJIngressos.Properties.Resources.resourceCulture;
      }
      set
      {
        VNJIngressos.Properties.Resources.resourceCulture = value;
      }
    }

    internal Resources()
    {
    }
  }
}
