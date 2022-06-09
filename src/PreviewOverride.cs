using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TModel
{
    // Should only be applied to derived classes of FPropertyTagType
    public interface IPreviewOverride
    {
        public PreviewOverrideData GetCustomData(object data);
    }

    // Data about overriding the way values are shown in the ObjectViewer
    public struct PreviewOverrideData
    {
        public string? OverrideTypeName;
        public FrameworkElement? OverrideElement;
    }
}
