using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Proteca.Silverlight.Helpers
{
    public class InlineEqualityComparer<T> : IEqualityComparer<T>
    {
        public InlineEqualityComparer(Func<T, T, bool> cmp)
        {
            this.cmp = cmp;
        }
        public bool Equals(T x, T y)
        {
            return cmp(x, y);
        }

        public int GetHashCode(T obj)
        {
            return 0;
        }

        public Func<T, T, bool> cmp { get; set; }
    }
}
