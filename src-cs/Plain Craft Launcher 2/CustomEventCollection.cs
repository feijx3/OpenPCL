using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Markup;

namespace PCL
{
    [ContentProperty("Events")]
    public class CustomEventCollection : IEnumerable<CustomEvent>
    {
        public CustomEventCollection()
        {
            this._Events = new List<CustomEvent>();
        }

        public List<CustomEvent> Events
        {
            get
            {
                return this._Events;
            }
        }

        public IEnumerator<CustomEvent> GetEnumerator()
        {
            return ((IEnumerable<CustomEvent>)this.Events).GetEnumerator();
        }

        IEnumerator IEnumerable.IEnumerable_GetEnumerator()
        {
            return ((IEnumerable)this.Events).GetEnumerator();
        }

        private List<CustomEvent> _Events;
    }
}
